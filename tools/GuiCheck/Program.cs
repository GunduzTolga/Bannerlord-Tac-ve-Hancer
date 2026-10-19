using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

// Arayüz dosyalarının oyunsuz denetimi (CI'da çalışır). Oyunu burada açamadığımız için Gauntlet prefab'larını
// oyunun referans DLL'lerine ve modun ViewModel'lerine karşı kontrol eder:
//   - her etiket ya bir oyun parçası (Widget sınıfı) ya da bizim prefab'larımızdan biri mi
//   - her özellik o parçada gerçekten var mı, sabit değeri türüne uyuyor mu (enum, sayı, renk, bool)
//   - "@Ad" bağlantıları ViewModel'de [DataSourceProperty] olarak var mı, türleri uyuyor mu
//   - "Command.Click" metotları ve "{Ad}" alt veri kaynakları var mı; liste şablonları doğru türle mi
//   - fırçalar fırça dosyamızda, sprite'lar izinli listede mi; parça yolları (ClipRect, Handle...) bir Id'ye çıkıyor mu
// Kullanım: GuiCheck <GUI klasörü> <CheatMenu.dll> <referans klasörü>...
static class Program
{
    // Hangi kök prefab hangi ViewModel ile açılır (UI/*.cs içindeki Movie sabitleriyle aynı).
    static readonly Dictionary<string, string> Roots = new Dictionary<string, string>
    {
        { "TvhIntrigueScreen", "CheatMenu.UI.ViewModels.IntrigueScreenVM" },
        { "TvhEventCard", "CheatMenu.UI.ViewModels.EventCardVM" },
        { "TvhMapButton", "CheatMenu.UI.ViewModels.MapButtonVM" },
    };

    // Yalnızca her oyun sürümünde bulunduğundan emin olduğumuz sprite'lar.
    static readonly HashSet<string> AllowedSprites = new HashSet<string> { "BlankWhiteSquare_9" };

    static readonly List<string> Errors = new List<string>();
    static readonly Dictionary<string, Type> WidgetTypes = new Dictionary<string, Type>();
    static readonly Dictionary<string, XmlElement> Prefabs = new Dictionary<string, XmlElement>();
    static readonly Dictionary<string, string> PrefabFiles = new Dictionary<string, string>();
    static readonly HashSet<string> Brushes = new HashSet<string>();
    static readonly HashSet<string> Checked = new HashSet<string>();
    static readonly HashSet<string> UsedPrefabs = new HashSet<string>();
    static Type WidgetBase, BindingListType, ViewModelType, DataSourceAttr;

    static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Kullanım: GuiCheck <GUI klasörü> <CheatMenu.dll> <referans klasörü>...");
            return 2;
        }
        string gui = args[0];
        var dirs = args.Skip(2).ToList();
        var paths = dirs.SelectMany(d => Directory.GetFiles(d, "*.dll")).ToList();
        paths.Add(args[1]);
        paths.AddRange(Directory.GetFiles(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "*.dll"));
        var resolver = new PathAssemblyResolver(paths.GroupBy(Path.GetFileName).Select(g => g.First()));
        using var mlc = new MetadataLoadContext(resolver, "netstandard");

        var assemblies = new List<Assembly>();
        foreach (string p in dirs.SelectMany(d => Directory.GetFiles(d, "*.dll")).Append(args[1]))
            try { assemblies.Add(mlc.LoadFromAssemblyPath(p)); } catch { }
        WidgetBase = Find(assemblies, "TaleWorlds.GauntletUI.BaseTypes.Widget");
        BindingListType = Find(assemblies, "TaleWorlds.Library.MBBindingList`1");
        ViewModelType = Find(assemblies, "TaleWorlds.Library.ViewModel");
        DataSourceAttr = Find(assemblies, "TaleWorlds.Library.DataSourceProperty");
        foreach (Assembly a in assemblies)
            foreach (Type t in SafeTypes(a))
                if (!t.IsAbstract && IsWidget(t) && !WidgetTypes.ContainsKey(t.Name)) WidgetTypes[t.Name] = t;
        Console.WriteLine(WidgetTypes.Count + " parça türü yüklendi");

        foreach (string file in Directory.GetFiles(Path.Combine(gui, "Brushes"), "*.xml", SearchOption.AllDirectories))
        {
            var doc = new XmlDocument();
            doc.Load(file);
            foreach (XmlElement b in doc.SelectNodes("//Brush")) Brushes.Add(b.GetAttribute("Name"));
            foreach (XmlElement layer in doc.SelectNodes("//BrushLayer[@Sprite]"))
                if (!AllowedSprites.Contains(layer.GetAttribute("Sprite"))) Error(file, "izinsiz sprite: " + layer.GetAttribute("Sprite"));
        }
        foreach (string file in Directory.GetFiles(Path.Combine(gui, "Prefabs"), "*.xml", SearchOption.AllDirectories))
        {
            var doc = new XmlDocument();
            doc.Load(file);
            var root = doc.SelectSingleNode("/Prefab/Window/*") as XmlElement;
            string name = Path.GetFileNameWithoutExtension(file);
            if (root == null) { Error(file, "Prefab/Window altında kök parça yok"); continue; }
            if (WidgetTypes.ContainsKey(name)) Error(file, "prefab adı bir oyun parçasıyla aynı: " + name);
            Prefabs[name] = root;
            PrefabFiles[name] = file;
        }

        foreach (var pair in Roots)
        {
            if (!Prefabs.ContainsKey(pair.Key)) { Error(pair.Key, "kök prefab bulunamadı"); continue; }
            Type vm = Find(assemblies, pair.Value);
            if (vm == null) { Error(pair.Key, "ViewModel bulunamadı: " + pair.Value); continue; }
            UsedPrefabs.Add(pair.Key);
            CheckPrefab(pair.Key, vm);
        }
        foreach (string unused in Prefabs.Keys.Where(k => !UsedPrefabs.Contains(k))) Error(PrefabFiles[unused], "hiçbir yerde kullanılmayan prefab");

        foreach (string e in Errors) Console.WriteLine("HATA " + e);
        Console.WriteLine(Errors.Count == 0 ? "ARAYÜZ DENETİMİ GEÇTİ (" + Prefabs.Count + " prefab, " + Brushes.Count + " fırça)" : Errors.Count + " hata");
        return Errors.Count == 0 ? 0 : 1;
    }

    static void CheckPrefab(string name, Type vm)
    {
        if (!Checked.Add(name + "|" + vm.FullName)) return;
        XmlElement root = Prefabs[name];
        var ids = new HashSet<string>(root.SelectNodes("descendant-or-self::*[@Id]").Cast<XmlElement>().Select(e => e.GetAttribute("Id")));
        CheckElement(PrefabFiles[name], root, vm, ids);
    }

    /// <returns>Parçanın türü (özel prefab ise kökünün türü).</returns>
    static Type ResolveType(string file, string tag)
    {
        if (WidgetTypes.TryGetValue(tag, out Type t)) return t;
        if (Prefabs.TryGetValue(tag, out XmlElement root)) return ResolveType(file, root.Name);
        Error(file, "bilinmeyen parça ya da prefab: <" + tag + ">");
        return null;
    }

    static void CheckElement(string file, XmlElement el, Type vm, HashSet<string> ids)
    {
        string where = file + " <" + el.Name + (el.HasAttribute("Id") ? " Id=" + el.GetAttribute("Id") : "") + ">";
        Type widget = ResolveType(file, el.Name);
        bool isCustom = Prefabs.ContainsKey(el.Name);

        Type contextVm = vm;
        Type listItem = null;
        if (el.HasAttribute("DataSource"))
        {
            string ds = el.GetAttribute("DataSource");
            if (!ds.StartsWith("{") || !ds.EndsWith("}")) Error(where, "DataSource {Ad} biçiminde olmalı: " + ds);
            else
            {
                PropertyInfo p = VmProperty(vm, ds.Substring(1, ds.Length - 2));
                if (p == null) Error(where, vm.Name + " içinde [DataSourceProperty] yok: " + ds);
                else if (IsBindingList(p.PropertyType)) { listItem = p.PropertyType.GetGenericArguments()[0]; contextVm = null; }
                else if (!IsViewModel(p.PropertyType)) Error(where, ds + " bir ViewModel değil (" + p.PropertyType.Name + ")");
                else contextVm = p.PropertyType;
            }
        }

        if (widget != null)
            foreach (XmlAttribute attr in el.Attributes)
            {
                string name = attr.Name, value = attr.Value;
                if (name == "DataSource" || name == "Id") continue;
                if (name.StartsWith("Command."))
                {
                    if (contextVm == null) { Error(where, name + " liste veri kaynağının üstünde kullanılamaz"); continue; }
                    if (!contextVm.GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.Name == value))
                        Error(where, contextVm.Name + " içinde komut yok: " + value);
                    continue;
                }
                if (name.StartsWith("Parameter.")) { Error(where, "parametreli prefab kullanılmıyor: " + name); continue; }
                Type propType = WidgetPropertyType(widget, name, out string problem);
                if (propType == null) { Error(where, problem); continue; }
                if (value.StartsWith("@"))
                {
                    if (contextVm == null) { Error(where, name + "=" + value + " liste veri kaynağının üstünde bağlanamaz"); continue; }
                    PropertyInfo vp = VmProperty(contextVm, value.Substring(1));
                    if (vp == null) { Error(where, contextVm.Name + " içinde [DataSourceProperty] yok: " + value); continue; }
                    if (!Compatible(propType, vp.PropertyType))
                        Error(where, name + " (" + propType.Name + ") ile " + value + " (" + vp.PropertyType.Name + ") uyumsuz");
                    continue;
                }
                CheckLiteral(where, name, propType, value, ids);
            }

        foreach (XmlNode node in el.ChildNodes)
        {
            if (!(node is XmlElement child)) continue;
            if (child.Name == "Children")
            {
                if (isCustom) Error(where, "özel prefab'a çocuk verilemez");
                foreach (XmlElement c in child.ChildNodes.OfType<XmlElement>()) CheckElement(file, c, contextVm ?? vm, ids);
            }
            else if (child.Name == "ItemTemplate")
            {
                if (listItem == null) { Error(where, "ItemTemplate için DataSource bir MBBindingList olmalı"); continue; }
                foreach (XmlElement c in child.ChildNodes.OfType<XmlElement>()) CheckElement(file, c, listItem, ids);
            }
            else Error(where, "beklenmeyen alt öğe: " + child.Name);
        }
        if (listItem != null && el.SelectSingleNode("ItemTemplate") == null) Error(where, "liste veri kaynağı var ama ItemTemplate yok");

        if (isCustom)
        {
            UsedPrefabs.Add(el.Name);
            CheckPrefab(el.Name, contextVm ?? vm);
        }
    }

    static void CheckLiteral(string where, string name, Type type, string value, HashSet<string> ids)
    {
        if (type.IsEnum)
        {
            if (!type.GetFields(BindingFlags.Public | BindingFlags.Static).Any(f => f.Name == value))
                Error(where, name + "=" + value + ": geçersiz değer (" + string.Join("/", type.GetFields(BindingFlags.Public | BindingFlags.Static).Select(f => f.Name)) + ")");
        }
        else if (type.FullName == "System.Boolean")
        {
            if (value != "true" && value != "false") Error(where, name + "=" + value + ": true/false olmalı");
        }
        else if (type.FullName == "System.Single" || type.FullName == "System.Int32")
        {
            if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _))
                Error(where, name + "=" + value + ": sayı olmalı");
        }
        else if (type.Name == "Color")
        {
            if (value.Length != 9 || value[0] != '#' || !value.Skip(1).All(Uri.IsHexDigit)) Error(where, name + "=" + value + ": #RRGGBBAA olmalı");
        }
        else if (type.Name == "Brush")
        {
            if (!Brushes.Contains(value)) Error(where, "fırça yok: " + value);
        }
        else if (type.Name == "Sprite")
        {
            if (!AllowedSprites.Contains(value)) Error(where, "izinsiz sprite: " + value);
        }
        else if (IsWidget(type))
        {
            string last = value.Split('\\').Last();
            if (!ids.Contains(last)) Error(where, name + "=" + value + ": bu Id prefab'da yok");
        }
        else if (type.FullName != "System.String")
            Error(where, name + "=" + value + ": sabit değer denetlenemeyen tür " + type.Name);
    }

    /// <summary>"StackLayout.LayoutMethod" gibi noktalı adları da çözer; yazılabilir özellik değilse null.</summary>
    static Type WidgetPropertyType(Type widget, string path, out string problem)
    {
        problem = null;
        Type current = widget;
        string[] parts = path.Split('.');
        for (int i = 0; i < parts.Length; i++)
        {
            PropertyInfo p = Property(current, parts[i]);
            if (p == null) { problem = current.Name + " içinde özellik yok: " + path; return null; }
            bool last = i == parts.Length - 1;
            if (last && (p.SetMethod == null || !p.SetMethod.IsPublic)) { problem = current.Name + "." + parts[i] + " yazılamaz"; return null; }
            current = p.PropertyType;
        }
        return current;
    }

    static PropertyInfo Property(Type type, string name)
    {
        for (Type t = type; t != null; t = t.BaseType)
        {
            PropertyInfo p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (p != null) return p;
        }
        return null;
    }

    static PropertyInfo VmProperty(Type vm, string name)
    {
        PropertyInfo p = Property(vm, name);
        if (p == null || p.GetMethod == null) return null;
        return p.GetCustomAttributesData().Any(a => a.AttributeType == DataSourceAttr) ? p : null;
    }

    static bool Compatible(Type widgetProp, Type vmProp)
    {
        string w = widgetProp.FullName, v = vmProp.FullName;
        if (w == v) return true;
        if (w == "System.Single" && (v == "System.Int32" || v == "System.Double")) return true;
        return false;
    }

    // Bazı oyun türlerinin üst türü burada olmayan DLL'lerde (platform SDK'ları): çözülemezse parça değildir.
    static bool IsWidget(Type t) => DerivesFrom(t, WidgetBase);

    static bool IsViewModel(Type t) => DerivesFrom(t, ViewModelType);

    static bool DerivesFrom(Type t, Type baseType)
    {
        try
        {
            for (Type c = t; c != null; c = c.BaseType) if (c == baseType) return true;
        }
        catch (FileNotFoundException) { }
        return false;
    }

    static bool IsBindingList(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == BindingListType;

    static Type Find(IEnumerable<Assembly> assemblies, string fullName)
    {
        foreach (Assembly a in assemblies)
        {
            Type t = a.GetType(fullName);
            if (t != null) return t;
        }
        return null;
    }

    static IEnumerable<Type> SafeTypes(Assembly a)
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
        catch { return Enumerable.Empty<Type>(); }
    }

    static void Error(string where, string message) => Errors.Add(where + ": " + message);
}

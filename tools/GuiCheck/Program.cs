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
    };

    // Kullandığımız oyun sprite'ları: yalnızca HER ZAMAN YÜKLÜ kategorilerden (ui_group1, ui_textures). Başka bir ekranın
    // kategorisindeki sprite (ör. ui_clan) o ekran kapalıyken boş çizilir. "--game" verilirse her biri oyunda doğrulanır.
    static readonly HashSet<string> AllowedSprites = new HashSet<string>
    {
        "BlankWhiteSquare_9",
        @"StdAssets\Popup\canvas", @"StdAssets\Popup\canvas_gradient", @"StdAssets\Popup\divider",
        @"SPGeneral\SPOptions\options_divider_flat",
        "button_default_9", "button_default_hover_9", "done_button_9", "done_button_hover_9",
        "rounded_canvas_9", "rounded_frame_9", "stone_texture_continuous",
        "Scrollbar.Vertical.Handle",
    };

    // Prefab'larımızda doğrudan kullandığımız oyun fırçaları (sprite'ları da her zaman yüklü olmalı).
    static readonly HashSet<string> GameBrushes = new HashSet<string> { "Popup.Frame" };

    // Oyunun her zaman yüklediği sprite kategorileri (SpriteData'da AlwaysLoad).
    static readonly HashSet<string> AlwaysLoadedCategories = new HashSet<string> { "ui_group1", "ui_textures", "ui_fonts", "ui_fullbackgrounds" };

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
        string game = null;
        int gameArg = Array.IndexOf(args, "--game");
        if (gameArg >= 0 && gameArg + 1 < args.Length)
        {
            game = args[gameArg + 1];
            args = args.Where((_, i) => i != gameArg && i != gameArg + 1).ToArray();
        }
        if (args.Length < 3)
        {
            Console.WriteLine("Kullanım: GuiCheck <GUI klasörü> <CheatMenu.dll> <referans klasörü>... [--game <oyun klasörü>]");
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
            foreach (XmlElement layer in doc.SelectNodes("//BrushLayer[@Sprite or @OverlaySprite]"))
                foreach (string attr in new[] { "Sprite", "OverlaySprite" })
                    if (layer.HasAttribute(attr) && !AllowedSprites.Contains(layer.GetAttribute(attr))) Error(file, "izinsiz sprite: " + layer.GetAttribute(attr));
        }
        Brushes.UnionWith(GameBrushes);
        if (game != null) CheckGameAssets(game);
        else Console.WriteLine("Oyun klasörü verilmedi: oyun sprite ve fırçaları yalnızca izinli listeye göre denetlendi");
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

    /// <summary>
    /// İzinli oyun sprite'ları ve fırçaları bu oyun sürümünde gerçekten var mı, sprite'ları her zaman yüklü bir kategoride mi.
    /// Oyunun SpriteData ve fırça dosyalarından okunur (yalnızca adlar ve kategoriler).
    /// </summary>
    static void CheckGameAssets(string game)
    {
        string modules = Path.Combine(game, "Modules");
        if (!Directory.Exists(modules)) { Console.WriteLine("Oyun klasörü bulunamadı (" + game + "): oyun varlıkları doğrulanmadı"); return; }
        var category = new Dictionary<string, string>();
        // Yalnızca modüllerin oyun içi arayüzü (Modules\X\GUI); LauncherGUI'nin kendi kategorileri var (ui_launcher).
        IEnumerable<string> spriteFiles = Directory.GetDirectories(modules)
            .Select(m => Path.Combine(m, "GUI")).Where(Directory.Exists)
            .SelectMany(g => Directory.GetFiles(g, "*SpriteData.xml", SearchOption.TopDirectoryOnly));
        foreach (string file in spriteFiles)
        {
            var doc = new XmlDocument();
            doc.Load(file);
            foreach (XmlElement part in doc.SelectNodes("//SpritePart"))
                category[part.SelectSingleNode("Name")?.InnerText ?? ""] = part.SelectSingleNode("CategoryName")?.InnerText;
            foreach (XmlElement nine in doc.SelectNodes("//NineRegionSprite"))
            {
                string partName = nine.SelectSingleNode("SpritePartName")?.InnerText ?? "";
                category[nine.SelectSingleNode("Name")?.InnerText ?? ""] = category.TryGetValue(partName, out string c) ? c : null;
            }
        }
        void CheckSprite(string where, string sprite)
        {
            if (!category.TryGetValue(sprite, out string c)) Error(where, "oyunda böyle bir sprite yok: " + sprite);
            else if (c == null || !AlwaysLoadedCategories.Contains(c)) Error(where, "sprite her zaman yüklü değil (" + c + "): " + sprite);
        }
        foreach (string sprite in AllowedSprites) CheckSprite("izinli sprite", sprite);

        var gameBrushLayers = new Dictionary<string, List<string>>();
        IEnumerable<string> brushFiles = Directory.GetDirectories(modules)
            .Select(m => Path.Combine(m, "GUI", "Brushes")).Where(Directory.Exists)
            .SelectMany(b => Directory.GetFiles(b, "*.xml", SearchOption.AllDirectories));
        foreach (string file in brushFiles)
        {
            var doc = new XmlDocument();
            try { doc.Load(file); } catch { continue; }
            foreach (XmlElement b in doc.SelectNodes("//Brush"))
                gameBrushLayers[b.GetAttribute("Name")] = b.SelectNodes(".//BrushLayer").Cast<XmlElement>()
                    .SelectMany(l => new[] { l.GetAttribute("Sprite"), l.GetAttribute("OverlaySprite") }).Where(s => s.Length > 0).ToList();
        }
        foreach (string brush in GameBrushes)
        {
            if (!gameBrushLayers.TryGetValue(brush, out List<string> sprites)) { Error("izinli oyun fırçası", "oyunda böyle bir fırça yok: " + brush); continue; }
            foreach (string sprite in sprites) CheckSprite("oyun fırçası " + brush, sprite);
        }
        Console.WriteLine("Oyun varlıkları doğrulandı: " + AllowedSprites.Count + " sprite, " + GameBrushes.Count + " fırça (" + category.Count + " oyun sprite'ı okundu)");
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

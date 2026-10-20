using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

// Paketlenen modların DLL'leri oyunda yüklenebilir mi. Oyun bir alt modülün DLL'ini açarken bütün türlerini okur; bir tür,
// bulunamayan bir DLL'e ya da o DLL'de olmayan bir türe dayanıyorsa "submodule could not be loaded correctly due to a
// dependency conflict" der. Burada aynısı oyunu açmadan yapılır: modülümüzün DLL'leri, oyunun DLL'leri ve bağımlı olduğumuz
// modüllerin (Harmony, ButterLib, UIExtenderEx, MCM) DLL'leriyle birlikte okunur, yüklenemeyen her tür ve eksik her DLL yazılır.
// Kullanım: RefCheck <oyun klasörü> <.NET Framework 4.7.2 referans klasörü>... [--module <Id>]
static class Program
{
    static readonly string Bin = Path.Combine("bin", "Win64_Shipping_Client");

    static int Main(string[] args)
    {
        string moduleId = "CheatMenu";
        int at = Array.IndexOf(args, "--module");
        if (at >= 0 && at + 1 < args.Length)
        {
            moduleId = args[at + 1];
            args = args.Where((_, i) => i != at && i != at + 1).ToArray();
        }
        if (args.Length < 2)
        {
            Console.WriteLine("Kullanım: RefCheck <oyun klasörü> <.NET Framework 4.7.2 referans klasörü>... [--module <Id>]");
            return 2;
        }
        string game = args[0];

        // Modül klasörleri: oyunun Modules'ü ve Steam atölyesi (steamapps\workshop\content\261550).
        var modules = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var roots = new List<string> { Path.Combine(game, "Modules") };
        DirectoryInfo steamapps = Directory.GetParent(game)?.Parent;
        if (steamapps != null) roots.Add(Path.Combine(steamapps.FullName, "workshop", "content", "261550"));
        foreach (string root in roots.Where(Directory.Exists))
            foreach (string dir in Directory.GetDirectories(root))
            {
                XmlDocument xml = Load(Path.Combine(dir, "SubModule.xml"));
                string id = xml?.SelectSingleNode("/Module/Id/@value")?.Value;
                if (id != null && !modules.ContainsKey(id)) modules[id] = dir;
            }
        if (!modules.TryGetValue(moduleId, out string ours)) { Console.WriteLine("Modül bulunamadı: " + moduleId); return 2; }
        XmlDocument manifest = Load(Path.Combine(ours, "SubModule.xml"));
        List<string> depends = manifest.SelectNodes("/Module/DependedModules/DependedModule/@Id").Cast<XmlNode>().Select(n => n.Value).ToList();
        // Paketlenen bir modun isteğe bağlı ortakları (ör. Regard and Standing için Calradia Remembers): kurulu değilse o modun
        // onlara dayanan birkaç türü yüklenemez. Oyun açılışta uyarı penceresi gösterir, OK ile devam eder; mod onları atlar.
        var optional = new HashSet<string>(manifest.SelectNodes("/Module/DependedModules/DependedModule[@Optional='true']/@Id").Cast<XmlNode>()
            .Select(n => n.Value), StringComparer.OrdinalIgnoreCase);
        var subModuleDlls = new HashSet<string>(manifest.SelectNodes("/Module/SubModules/SubModule/DLLName/@value").Cast<XmlNode>()
            .Select(n => n.Value), StringComparer.OrdinalIgnoreCase);

        // Oyunun açılışta göreceği DLL'ler: oyunun kendisi, bizim modül ve bağımlı olduklarımız. Önce gelen kazanır.
        var available = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        void AddDir(string dir)
        {
            if (!Directory.Exists(dir)) return;
            foreach (string dll in Directory.GetFiles(dir, "*.dll"))
                if (!available.ContainsKey(Path.GetFileNameWithoutExtension(dll))) available[Path.GetFileNameWithoutExtension(dll)] = dll;
        }
        foreach (string reference in args.Skip(1)) { AddDir(reference); AddDir(Path.Combine(reference, "Facades")); }
        AddDir(Path.Combine(game, Bin));
        AddDir(Path.Combine(ours, Bin));
        foreach (string id in depends)
            if (modules.TryGetValue(id, out string dir)) AddDir(Path.Combine(dir, Bin));
            else if (!IsOfficial(id) && !optional.Contains(id)) Console.WriteLine("UYARI bağımlı modül kurulu değil: " + id);
        foreach (string official in new[] { "Native", "SandBoxCore", "Sandbox", "StoryMode", "CustomBattle", "NavalDLC" })
            if (modules.TryGetValue(official, out string dir)) AddDir(Path.Combine(dir, Bin));

        // Başka (yüklenmeyen) modüllerde bulunan DLL'ler: eksik bir DLL'in nerede olduğunu söylemek için.
        var elsewhere = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in modules)
        {
            string bin = Path.Combine(pair.Value, Bin);
            if (!Directory.Exists(bin)) continue;
            foreach (string dll in Directory.GetFiles(bin, "*.dll"))
            {
                string name = Path.GetFileNameWithoutExtension(dll);
                if (!elsewhere.TryGetValue(name, out List<string> list)) elsewhere[name] = list = new List<string>();
                list.Add(pair.Key);
            }
        }

        var resolver = new PathAssemblyResolver(available.Values);
        using var mlc = new MetadataLoadContext(resolver, "mscorlib");
        int errors = 0, warnings = 0;
        foreach (string dll in Directory.GetFiles(Path.Combine(ours, Bin), "*.dll").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            string file = Path.GetFileName(dll);
            bool subModule = subModuleDlls.Contains(file);
            Assembly assembly;
            try { assembly = mlc.LoadFromAssemblyPath(dll); }
            catch (BadImageFormatException) { continue; } // .NET olmayan (yerel) DLL
            catch (Exception ex) { Report(subModule, file, "açılamadı: " + ex.Message); continue; }

            // Eksik DLL tek başına sorun değil (yalnızca kullanılırsa); oyunun uyarısı aşağıdaki tür hatalarından gelir.
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (available.ContainsKey(reference.Name)) continue;
                string where = elsewhere.TryGetValue(reference.Name, out List<string> mods)
                    ? " (şu modüllerde var ama oyun onları yüklemiyor: " + string.Join(", ", mods) + ")" : "";
                string owner = OptionalOwner(reference.Name);
                Console.WriteLine("BİLGİ " + file + ": eksik DLL: " + reference.Name + " " + reference.Version + where
                    + (owner != null ? " (isteğe bağlı mod " + owner + " kurulu değil)" : ""));
            }
            // Oyun türleri yüklerken üst türleri ve arayüzleri de çözer; burada ayrıca istenmezse tembel kalırlar.
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
                foreach (string message in ex.LoaderExceptions.Where(e => e != null).Select(e => e.Message).Distinct().Take(15))
                    Report(subModule, file, "yüklenemeyen tür: " + message, message);
            }
            catch (Exception ex) { Report(subModule, file, "türleri okunamadı: " + ex.Message); continue; }
            var broken = new List<string>();
            foreach (Type type in types)
            {
                try
                {
                    for (Type t = type.BaseType; t != null; t = t.BaseType) { }
                    type.GetInterfaces();
                }
                catch (Exception ex) { broken.Add(type.FullName + " (" + ex.Message + ")"); }
            }
            foreach (string b in broken.Take(15)) Report(subModule, file, "yüklenemeyen tür: " + b, b);
            if (broken.Count > 15) Report(subModule, file, "... ve " + (broken.Count - 15) + " tür daha");
        }

        // Bir eksik DLL'in hangi isteğe bağlı modüle ait olduğu (adı modül adıyla başlıyorsa).
        string OptionalOwner(string assemblyName) =>
            optional.FirstOrDefault(id => assemblyName.Equals(id, StringComparison.OrdinalIgnoreCase)
                || assemblyName.StartsWith(id + ".", StringComparison.OrdinalIgnoreCase));

        void Report(bool subModule, string file, string message, string detail = null)
        {
            // Alt modül DLL'i: oyun açılışta tamamını okur, hata uyarı penceresi çıkarır. Kütüphane: yalnızca kullanılırsa sorun olur.
            // Hata, kurulu olmayan isteğe bağlı bir modülden geliyorsa: pencere çıkar ama OK ile devam edilir; derlemeyi durdurmaz.
            string owner = detail == null ? null
                : optional.FirstOrDefault(id => detail.Contains("'" + id + ",") || detail.Contains("'" + id + "."));
            if (subModule && owner == null) errors++; else warnings++;
            Console.WriteLine((subModule && owner == null ? "HATA " : "UYARI ") + file + ": " + message
                + (owner != null ? Environment.NewLine + "      -> isteğe bağlı mod " + owner + " kurulu değil: oyun açılışta \"dependency conflict\" "
                    + "penceresi gösterir, OK ile devam eder; o modla ortak özellik kapalı kalır" : ""));
        }

        Console.WriteLine(errors == 0
            ? "DLL DENETİMİ GEÇTİ (" + subModuleDlls.Count + " alt modül DLL'i" + (warnings > 0 ? ", " + warnings + " uyarı" : "") + ")"
            : errors + " hata: oyun bu alt modüller için \"dependency conflict\" uyarısı verir");
        return errors == 0 ? 0 : 1;
    }

    static bool IsOfficial(string id) =>
        new[] { "Native", "SandBoxCore", "Sandbox", "StoryMode", "CustomBattle", "Multiplayer", "BirthAndDeath", "NavalDLC" }
            .Contains(id, StringComparer.OrdinalIgnoreCase);

    static XmlDocument Load(string path)
    {
        if (!File.Exists(path)) return null;
        var doc = new XmlDocument();
        try { doc.Load(path); return doc; }
        catch { return null; }
    }
}

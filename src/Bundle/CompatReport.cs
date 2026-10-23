using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Bundle
{
    /// <summary>
    /// Kampanya her açıldığında paketlenen modların (ve bizim) birbirine çarpıp çarpmadığını denetler: aynı oyun metodunu yamalayan
    /// ve aynı oyun modelini ekleyen modlar (değerlendirme: CompatRules). Rapor: Belgeler\Mount and Blade II Bannerlord\Configs\
    /// TacVeHancer\uyum_raporu.txt; ciddi çakışma varsa ekranda kırmızı mesaj. Oyunu hiçbir şekilde değiştirmez.
    /// </summary>
    internal static class CompatReport
    {
        private const string FileName = "uyum_raporu.txt";
        private static readonly string[] GamePrefixes = { "TaleWorlds.", "SandBox", "StoryMode", "NavalDLC", "Bannerlord.ReferenceAssemblies" };

        internal static void Run(CampaignGameStarter starter)
        {
            try
            {
                List<CompatFinding> patches = CompatRules.PatchFindings(Patches());
                List<CompatFinding> models = CompatRules.ModelFindings(Models(starter));
                // Modlar: Modules klasöründen yüklenen, oyunun kendisi olmayan DLL'ler.
                IEnumerable<string> mods = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && Location(a).IndexOf(Path.DirectorySeparatorChar + "Modules" + Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(Name).Where(n => n != null && !IsGame(n))
                    .Distinct().OrderBy(n => n, StringComparer.OrdinalIgnoreCase);
                string text = CompatRules.Format(patches, models, mods, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                List<string> hair = BundleShim.HairAssetsReport(out bool hairMissing);
                if (hair.Count > 0) text += Environment.NewLine + string.Join(Environment.NewLine, hair) + Environment.NewLine;
                Directory.CreateDirectory(CrashLog.Folder);
                File.WriteAllText(Path.Combine(CrashLog.Folder, FileName), text);

                int high = CompatRules.Count(patches, CompatRisk.High) + CompatRules.Count(models, CompatRisk.High);
                int medium = CompatRules.Count(patches, CompatRisk.Medium) + CompatRules.Count(models, CompatRisk.Medium);
                if (high + medium > 0)
                    InformationManager.DisplayMessage(new InformationMessage("[" + ModInfo.DisplayName + "] Mod uyumu: " + high + " ciddi, "
                        + medium + " dikkat gerektiren çakışma. Ayrıntı: Belgeler\\Mount and Blade II Bannerlord\\Configs\\TacVeHancer\\" + FileName,
                        high > 0 ? Color.FromUint(0xFFFF6B6B) : Color.FromUint(0xFFE4C59B)));
                else
                    Debug.Print("[CheatMenu] compatibility report: no conflicts");
                if (hairMissing)
                    InformationManager.DisplayMessage(new InformationMessage("[" + ModInfo.DisplayName + "] DLX Hair Layers: saç modelleri oyuna "
                        + "yüklenmemiş, saç seçmek bir şey yapmaz. Ayrıntı: Belgeler\\Mount and Blade II Bannerlord\\Configs\\TacVeHancer\\" + FileName,
                        Color.FromUint(0xFFFF6B6B)));
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] compatibility report failed: " + ex);
            }
        }

        private static IEnumerable<PatchEntry> Patches()
        {
            foreach (MethodBase original in Harmony.GetAllPatchedMethods().ToList())
            {
                HarmonyLib.Patches info;
                try { info = Harmony.GetPatchInfo(original); }
                catch { continue; }
                if (info == null) continue;
                string target = (original.DeclaringType?.FullName ?? "?") + "." + original.Name;
                foreach (Patch p in info.Prefixes) yield return Entry(target, p, PatchKind.Prefix);
                foreach (Patch p in info.Postfixes) yield return Entry(target, p, PatchKind.Postfix);
                foreach (Patch p in info.Transpilers) yield return Entry(target, p, PatchKind.Transpiler);
                foreach (Patch p in info.Finalizers) yield return Entry(target, p, PatchKind.Finalizer);
            }
        }

        private static PatchEntry Entry(string target, Patch patch, PatchKind kind)
        {
            MethodInfo method = null;
            try { method = patch.PatchMethod; } catch { }
            return new PatchEntry
            {
                Target = target,
                Mod = Name(method?.DeclaringType?.Assembly) ?? patch.owner,
                Kind = kind,
                CanSkipOriginal = kind == PatchKind.Prefix && method?.ReturnType == typeof(bool),
            };
        }

        private static IEnumerable<ModelEntry> Models(CampaignGameStarter starter)
        {
            if (starter == null) yield break;
            foreach (GameModel model in starter.Models.ToList())
            {
                if (model == null) continue;
                Type kind = KindOf(model.GetType());
                if (kind == null) continue;
                string mod = Name(model.GetType().Assembly);
                bool game = IsGame(mod);
                yield return new ModelEntry { Kind = kind.Name, Mod = mod, IsGame = game, CallsBase = game || CallsBaseModel(model.GetType()) };
            }
        }

        /// <summary>Modelin türü: MBGameModel&lt;T&gt;'deki T (ör. DefaultPregnancyModel -> PregnancyModel).</summary>
        private static Type KindOf(Type type)
        {
            for (Type t = type; t != null && t.BaseType != null; t = t.BaseType)
                if (t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(MBGameModel<>))
                    return t.BaseType.GetGenericArguments()[0];
            return null;
        }

        /// <summary>
        /// Modelin kendi kodu (oyunun sınıflarına kadar) bir yerde BaseModel'i okuyor mu: okuyorsa önceki modele devrediyor demektir.
        /// IL'deki call/callvirt hedefleri çözülür; çözülemeyen bayt dizileri yok sayılır.
        /// </summary>
        private static bool CallsBaseModel(Type type)
        {
            for (Type t = type; t != null && !IsGame(Name(t.Assembly)); t = t.BaseType)
            {
                foreach (MethodInfo method in t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                    | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    byte[] il;
                    try { il = method.GetMethodBody()?.GetILAsByteArray(); }
                    catch { continue; }
                    if (il == null) continue;
                    for (int i = 0; i + 4 < il.Length; i++)
                    {
                        if (il[i] != 0x28 && il[i] != 0x6F) continue; // call, callvirt
                        int token = BitConverter.ToInt32(il, i + 1);
                        int table = token >> 24;
                        if (table != 0x06 && table != 0x0A && table != 0x2B) continue; // MethodDef, MemberRef, MethodSpec
                        try
                        {
                            MethodBase called = t.Module.ResolveMethod(token, t.IsGenericType ? t.GetGenericArguments() : null,
                                method.IsGenericMethod ? method.GetGenericArguments() : null);
                            if (called?.Name == "get_BaseModel") return true;
                        }
                        catch { }
                    }
                }
            }
            return false;
        }

        private static bool IsGame(string assembly) =>
            assembly != null && GamePrefixes.Any(p => assembly.StartsWith(p, StringComparison.Ordinal));

        private static string Location(Assembly assembly)
        {
            try { return assembly.Location ?? ""; }
            catch { return ""; }
        }

        private static string Name(Assembly assembly)
        {
            try { return assembly?.GetName().Name; }
            catch { return null; }
        }
    }
}

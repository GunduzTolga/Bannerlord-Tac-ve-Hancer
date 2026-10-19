using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Menu;
using HarmonyLib;

namespace CheatMenu.Patches
{
    /// <summary>
    /// Harmony yamaları (yol haritası P0). Yalnızca oyun davranışını başka yolla değiştiremediğimizde yama yazılır.
    /// Her yama sınıfı ayrı uygulanır: oyun güncellemesi bir yamayı bozarsa o özellik kapanır, oyun ve diğer yamalar çalışır;
    /// tutmayanlar kampanya açılınca adıyla bildirilir.
    /// </summary>
    public static class HarmonyPatches
    {
        private const string HarmonyId = "tr.tacvehancer.cheatmenu";
        private static readonly List<string> Failed = new List<string>();

        public static void Apply()
        {
            var harmony = new Harmony(HarmonyId);
            IEnumerable<Type> patchClasses = typeof(HarmonyPatches).Assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(HarmonyPatch), true).Length > 0);
            foreach (Type type in patchClasses)
            {
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch (Exception ex)
                {
                    Failed.Add(type.Name);
                    TaleWorlds.Library.Debug.Print("[CheatMenu] Harmony patch failed: " + type.FullName + " " + ex);
                }
            }
        }

        /// <summary>Kampanya açılınca bir kez: tutmayan yamaları oyuncuya söyler.</summary>
        public static void ReportFailures()
        {
            if (Failed.Count == 0) return;
            MenuUI.Error("Oyun sürümüyle uyuşmayan " + Failed.Count + " yama kapatıldı: " + string.Join(", ", Failed)
                + ". İlgili özellikler çalışmayabilir.");
            Failed.Clear();
        }
    }
}

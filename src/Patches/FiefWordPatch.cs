using System;
using System.Collections.Generic;
using CheatMenu.Intrigue.Core;
using HarmonyLib;
using TaleWorlds.Localization;

namespace CheatMenu.Patches
{
    /// <summary>
    /// Oyunun Türkçesinde fief "tımar"; kullanıcı "toprak" istedi (CLAUDE.md > Terimler). Oyunun çevirdiği her metin buradan geçer
    /// (Klan > Tımarlar sekmesi, Krallık ekranı, ansiklopedi, ipuçları): içinde "tımar" geçen metin ekleriyle "toprak"a çevrilir
    /// (Tr.TimarToToprak). Öteki diller ve tımar geçmeyen metinler dokunulmadan geçer; çevrilenler önbellekte tutulur.
    /// </summary>
    [HarmonyPatch(typeof(LocalizedTextManager), nameof(LocalizedTextManager.GetTranslatedText))]
    internal static class FiefWordPatch
    {
        private static readonly Dictionary<string, string> Cache = new Dictionary<string, string>();

        private static void Postfix(ref string __result)
        {
            if (__result == null || (__result.IndexOf("ımar", StringComparison.Ordinal) < 0 && __result.IndexOf("IMAR", StringComparison.Ordinal) < 0))
                return;
            lock (Cache)
            {
                if (!Cache.TryGetValue(__result, out string replaced))
                {
                    replaced = Tr.TimarToToprak(__result);
                    if (Cache.Count < 4096) Cache[__result] = replaced;
                }
                __result = replaced;
            }
        }
    }
}

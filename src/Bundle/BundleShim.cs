using System;
using System.IO;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Bundle
{
    /// <summary>
    /// Taç ve Hançer'in içine paketlenen modların (third_party\paketler.txt, tools\bundle.ps1) uyum yamaları.
    /// SubModule.xml'de ilk alt modül: oyun önce bütün DLL'leri yükler, sonra OnSubModuleLoad'u listedeki sırayla çağırır;
    /// yani paketlenen modların türleri burada yüklüdür ama onların OnSubModuleLoad'u henüz çalışmamıştır.
    /// Paketlenen mod yoksa hiçbir şey yapmaz.
    /// </summary>
    public sealed class BundleShim : MBSubModuleBase
    {
        /// <summary>Paketlenen modların kök klasördeki dosyaları (ör. ayar dosyaları) Modules\CheatMenu\Bundled\&lt;mod&gt;\ altındadır.</summary>
        internal static string BundledFile(string modId, string fileName)
        {
            string bin = Path.GetDirectoryName(typeof(BundleShim).Assembly.Location);
            return Path.GetFullPath(Path.Combine(bin, "..", "..", "Bundled", modId, fileName));
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                Apply();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] bundle shim failed: " + ex);
            }
        }

        // Ayrı metot: Harmony'nin türleri yalnızca bu çağrıda yüklenir, eksikse istisna yukarıda yakalanır.
        private static void Apply()
        {
            var harmony = new Harmony(ModInfo.Id + ".bundle");
            // Women in Calradia ayarlarını sabit "Modules\WomeninCalradia\config.xml" yolundan okur; dosya yoksa açılışta çöker.
            var wicSettings = AccessTools.Method("Women_In_Calradia.SettingsLoader:LoadSettings", new[] { typeof(string) });
            if (wicSettings != null)
                harmony.Patch(wicSettings, prefix: new HarmonyMethod(typeof(BundleShim), nameof(WomenInCalradiaConfig)));

            // Companion Autonomy, Klan > Birlikler'de kendi bütçe bölümü görünürken "Birlik Lideri Rol Etkileri" listesini 230'dan
            // 36 piksele indiriyor (tek satır: liste görünmez oluyor). Liste kaydırılabilir; birkaç satır görünecek kadar yükseklik verilir.
            var roleEffectsHeight = AccessTools.PropertyGetter("CompanionAutonomy.ClanPartyBudgetMixin:CARoleEffectsHeight");
            if (roleEffectsHeight != null && roleEffectsHeight.ReturnType == typeof(float))
                harmony.Patch(roleEffectsHeight, postfix: new HarmonyMethod(typeof(BundleShim), nameof(RoleEffectsHeight)));
        }

        /// <summary>Companion Autonomy açıkken rol etkileri listesinin en az yüksekliği (oyunun kendisi 230 kullanır).</summary>
        internal const float MinRoleEffectsHeight = 130f;

        private static void RoleEffectsHeight(ref float __result)
        {
            if (__result < MinRoleEffectsHeight) __result = MinRoleEffectsHeight;
        }

        private static void WomenInCalradiaConfig(ref string __0)
        {
            // Ayrı kurulmuşsa kendi dosyası kullanılır.
            if (File.Exists(__0)) return;
            string bundled = BundledFile("WomeninCalradia", "config.xml");
            if (File.Exists(bundled)) __0 = bundled;
        }
    }
}

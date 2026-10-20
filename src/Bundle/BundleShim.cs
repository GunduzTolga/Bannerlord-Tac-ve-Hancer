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

        /// <summary>
        /// Diplomacy'nin asıl DLL'ini kendi yükleyicisi OnSubModuleLoad'da yükler (bizden sonra): onun türlerine dokunan yamalar
        /// bütün modüller yüklendikten sonra uygulanır.
        /// </summary>
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_lateApplied) return;
            _lateApplied = true;
            try
            {
                ApplyLate();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] bundle shim (late) failed: " + ex);
            }
        }

        private static bool _lateApplied;

        private static void ApplyLate()
        {
            var harmony = new Harmony(ModInfo.Id + ".bundle.late");
            // Savaş Raporu (Diplomacy): prefab'ı büyütüldü (third_party\Bannerlord.Diplomacy\GUI\Prefabs\WarExhaustion\DetailWarView.xml),
            // yüksekliği de aynı oranda; kapatma düğmesinin yeri (CloseButtonYOffset) bu yükseklikten hesaplanır.
            var reportHeight = AccessTools.PropertyGetter("Diplomacy.ViewModel.DetailWarVM:ReportHeight");
            if (reportHeight != null && reportHeight.ReturnType == typeof(int))
                harmony.Patch(reportHeight, postfix: new HarmonyMethod(typeof(BundleShim), nameof(WarReportHeight)));
        }

        /// <summary>Savaş Raporu penceresinin büyütme oranı (yazılar 16'dan 20 puntoya).</summary>
        internal const float WarReportScale = 1.25f;

        private static void WarReportHeight(ref int __result) => __result = (int)Math.Round(__result * WarReportScale);

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

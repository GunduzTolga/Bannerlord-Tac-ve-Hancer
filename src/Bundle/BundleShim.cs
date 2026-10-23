using System;
using System.IO;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
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

            // DLX Hair Layers'ın savaş davranışında hata yakalama yok: savaşın ortasındaki bir istisna oyunu masaüstüne atar (22 Ekim deniz
            // savaşı çökmesinin adayı). Hata verirse o savaşta saç katmanları durur, oyun sürer; hata çökme kaydına yazılır.
            foreach (string name in new[] { "OnAgentBuild", "OnAgentDeleted", "OnMissionTick" })
            {
                var hairMission = AccessTools.Method("HairAddons.HairAddonsMissionBehavior:" + name);
                if (hairMission != null)
                    harmony.Patch(hairMission, prefix: new HarmonyMethod(typeof(BundleShim), nameof(HairMissionPrefix)),
                        finalizer: new HarmonyMethod(typeof(BundleShim), nameof(HairMissionFinalizer)));
            }
        }

        /// <summary>Hata veren saç katmanı davranışı: o savaşta bir daha çalışmaz (yeni savaşta yenisi kurulur).</summary>
        private static readonly WeakReference BrokenHairMission = new WeakReference(null);

        private static bool HairMissionPrefix(object __instance) => !ReferenceEquals(BrokenHairMission.Target, __instance);

        /// <summary>null: istisna yutulur, savaş sürer.</summary>
        private static Exception HairMissionFinalizer(object __instance, Exception __exception)
        {
            if (__exception == null) return null;
            BrokenHairMission.Target = __instance;
            CrashLog.Note("DLX Hair Layers savaşta hata verdi (oyun sürdü, saç katmanları bu savaş için durdu)", __exception.ToString());
            Debug.Print("[CheatMenu] DLX Hair Layers mission error: " + __exception);
            try
            {
                Menu.MenuUI.Error("DLX Hair Layers bu savaşta hata verdi; saç katmanları bu savaş için kapatıldı.");
            }
            catch
            {
                // Mesaj gösterilemezse de savaş sürsün.
            }
            return null;
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
            // Savaş Raporu: Diplomacy'nin penceresi yerine bizimki (üst kısmı yine onun ViewModel'iyle; UI.WarReportScreen).
            var warReport = AccessTools.Method("Diplomacy.GauntletInterfaces.DetailWarViewInterface:ShowInterface");
            if (warReport != null)
                harmony.Patch(warReport, prefix: new HarmonyMethod(typeof(BundleShim), nameof(WarReportWindow)));
            // İsyanlar ve iç savaş: Diplomacy'nin grup penceresi yerine Entrika ekranının İsyanlar sekmesi (kuralları ve düğmeleri yine onun).
            var factions = AccessTools.Method("Diplomacy.GauntletInterfaces.RebelFactionsInterface:ShowInterface");
            if (factions != null)
                harmony.Patch(factions, prefix: new HarmonyMethod(typeof(BundleShim), nameof(RebellionsWindow)));
            // Altın bağışı, fief verme, fief iadesi: Diplomacy'nin pencereleri yerine Entrika ekranının Krallık sekmesi (işlemler yine onun).
            Type screen = typeof(TaleWorlds.ScreenSystem.ScreenBase);
            var donate = AccessTools.Method("Diplomacy.GauntletInterfaces.DonateGoldInterface:ShowInterface", new[] { screen, typeof(Clan) });
            if (donate != null)
                harmony.Patch(donate, prefix: new HarmonyMethod(typeof(BundleShim), nameof(DonateGoldWindow)));
            var grant = AccessTools.Method("Diplomacy.GauntletInterfaces.GrantFiefInterface:ShowFiefInterface", new[] { screen, typeof(Hero), typeof(Action) });
            if (grant != null)
                harmony.Patch(grant, prefix: new HarmonyMethod(typeof(BundleShim), nameof(GrantFiefWindow)));
            var giveBack = AccessTools.Method("Diplomacy.GauntletInterfaces.ReturnFiefInterface:ShowReturnFiefInterface", new[] { screen, typeof(Kingdom), typeof(Action) });
            if (giveBack != null)
                harmony.Patch(giveBack, prefix: new HarmonyMethod(typeof(BundleShim), nameof(ReturnFiefWindow)));
            // AutoTrader'ın ayar penceresi (Alt+A) yerine Entrika ekranının Saray > Ticaret sekmesi (ayarlar yine onun dosyasında).
            var autoTrader = AccessTools.Method("AutoTrader.AutoTraderSubModule:OpenSettingsMenu");
            if (autoTrader != null)
                harmony.Patch(autoTrader, prefix: new HarmonyMethod(typeof(BundleShim), nameof(AutoTraderWindow)));
        }

        private static DateTime _autoTraderOpened = DateTime.MinValue;

        /// <summary>
        /// false: AutoTrader'ın penceresi açılmaz, Ticaret sekmesi açılır. AutoTrader tuşları basılı kaldıkça her karede çağırır; ekran açılana
        /// kadar (birkaç kare) yeniden açmayalım. Kampanya durumu yoksa onunki açılır.
        /// </summary>
        private static bool AutoTraderWindow()
        {
            try
            {
                if ((DateTime.UtcNow - _autoTraderOpened).TotalMilliseconds < 1500) return false;
                if (!UI.IntrigueScreen.CanRedirect()) return true;
                _autoTraderOpened = DateTime.UtcNow;
                UI.IntrigueScreen.OpenTab(UI.ViewModels.IntrigueScreenVM.TabTrade);
                return false;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] AutoTrader window hook failed: " + ex);
                return true;
            }
        }

        /// <summary>false: Diplomacy'nin penceresi açılmaz, Krallık sekmesi açılır (bu klan seçili). Krallıkta değilsen onunki açılır.</summary>
        private static bool RealmTab(Clan clan, string what)
        {
            try
            {
                if (Clan.PlayerClan?.Kingdom == null || !UI.IntrigueScreen.CanRedirect()) return true;
                UI.IntrigueScreen.OpenRealm(clan != null && clan.Kingdom == Clan.PlayerClan.Kingdom ? clan : null);
                return false;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] " + what + " window hook failed: " + ex);
                return true;
            }
        }

        private static bool DonateGoldWindow(TaleWorlds.ScreenSystem.ScreenBase screenBase, Clan clan) => RealmTab(clan, "donate gold");

        private static bool GrantFiefWindow(TaleWorlds.ScreenSystem.ScreenBase screenBase, Hero hero, Action refreshAction) => RealmTab(hero?.Clan, "grant fief");

        private static bool ReturnFiefWindow(TaleWorlds.ScreenSystem.ScreenBase screenBase, Kingdom targetKingdom, Action refreshAction) => RealmTab(null, "return fief");

        /// <summary>false: Diplomacy'nin penceresi açılmaz, Entrika ekranının İsyanlar sekmesi açılır (Krallık ekranı ya da ansiklopedi kapanır).</summary>
        private static bool RebellionsWindow(TaleWorlds.ScreenSystem.ScreenBase screenBase, Kingdom kingdom)
        {
            try
            {
                if (kingdom == null || !UI.IntrigueScreen.CanRedirect()) return true;
                UI.IntrigueScreen.OpenRebellions(kingdom == Clan.PlayerClan.Kingdom ? null : kingdom);
                return false;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] rebellions window hook failed: " + ex);
                return true;
            }
        }

        /// <summary>false: Diplomacy'nin raporu açılmaz, bizimki açıldı. Bizimki açılamazsa onunki açılır.</summary>
        private static bool WarReportWindow(TaleWorlds.ScreenSystem.ScreenBase screenBase, Kingdom opposingKingdom)
        {
            try
            {
                return !UI.WarReportScreen.Open(opposingKingdom, screenBase);
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] war report window hook failed: " + ex);
                return true;
            }
        }

        /// <summary>Diplomacy'nin kendi Savaş Raporu penceresinin büyütme oranı (yazılar 16'dan 20 puntoya; bizimki açılamazsa o açılır).</summary>
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

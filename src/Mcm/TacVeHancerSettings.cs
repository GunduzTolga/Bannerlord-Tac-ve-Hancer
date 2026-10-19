using System;
using System.ComponentModel;
using CheatMenu.Cheats;
using CheatMenu.Intrigue.Core;
using MCM.Abstractions;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace CheatMenu.Mcm
{
    /// <summary>
    /// MCM ayar ekranı: Esc > Mod Seçenekleri > Taç ve Hançer. Çalışma zamanındaki tek kaynak ModSettings (SettingsStore);
    /// bu sınıf ona ayna tutar. MCM'de kaydedilince ve yüklenince değerler ModSettings'e aktarılır; Insert menüsünden
    /// değişince (SettingsStore.Saved) buraya kopyalanıp MCM dosyasına yazılır. İlk açılışta eski CheatMenu.txt içe aktarılır.
    /// </summary>
    public sealed class TacVeHancerSettings : AttributeGlobalSettings<TacVeHancerSettings>
    {
        private const string RulesGroup = "Kurallar";
        private const string IntrigueGroup = "Entrika";
        private const string MapGroup = "Harita";
        private const string UiGroup = "Arayüz";
        private const string RulesHint = " (yalnızca senin klanın/partin; kayıt dosyasına bir şey yazılmaz)";

        private static readonly string[] DifficultyNames = { "Kolay", "Normal", "Zor" };
        private static readonly string[] AiNames = { "Kapalı", "Az", "Normal" };
        private static bool _syncing;

        public override string Id => "TacVeHancer";
        public override string DisplayName => ModInfo.DisplayName;
        public override string FolderName => "TacVeHancer";
        public override string FormatType => "json2";

        // ---- Kurallar ----

        [SettingPropertyDropdown("Yetenek XP çarpanı", Order = 0, RequireRestart = false,
            HintText = "Kahramanlarının yetenek XP'si bu kadar katlanır" + RulesHint)]
        [SettingPropertyGroup(RulesGroup, GroupOrder = 0)]
        public Dropdown<string> SkillXp { get; set; } = Choices(ModSettings.AllowedXpMultipliers, v => v + "x");

        [SettingPropertyDropdown("Asker XP çarpanı", Order = 1, RequireRestart = false,
            HintText = "Partindeki askerlerin vuruşlardan kazandığı XP bu kadar katlanır" + RulesHint)]
        [SettingPropertyGroup(RulesGroup, GroupOrder = 0)]
        public Dropdown<string> TroopXp { get; set; } = Choices(ModSettings.AllowedXpMultipliers, v => v + "x");

        [SettingPropertyDropdown("Parti boyutu bonusu", Order = 2, RequireRestart = false,
            HintText = "Partinin asker ve esir sınırına eklenir" + RulesHint)]
        [SettingPropertyGroup(RulesGroup, GroupOrder = 0)]
        public Dropdown<string> PartySize { get; set; } = Choices(ModSettings.AllowedPartySizeBonuses, v => "+" + v);

        [SettingPropertyBool("Öğrenme sınırını kaldır", Order = 3, RequireRestart = false,
            HintText = "Öğrenme sınırına +1000: yetenekler sınırsız gelişir" + RulesHint)]
        [SettingPropertyGroup(RulesGroup, GroupOrder = 0)]
        public bool NoLearningLimit { get; set; }

        [SettingPropertyBool("Yoldaş sınırını kaldır", Order = 4, RequireRestart = false,
            HintText = "Klanının yoldaş sınırı 999 olur" + RulesHint)]
        [SettingPropertyGroup(RulesGroup, GroupOrder = 0)]
        public bool NoCompanionLimit { get; set; }

        [SettingPropertyInteger("Parti sayısı sınırına ekle", 0, ModSettings.MaxPartyLimitBonus, "0", Order = 5, RequireRestart = false,
            HintText = "Klanının aynı anda kurabileceği parti sayısına eklenir" + RulesHint)]
        [SettingPropertyGroup(RulesGroup, GroupOrder = 0)]
        public int PartyLimitBonus { get; set; }

        [SettingPropertyBool("Parti sayısı sınırını kaldır", Order = 6, RequireRestart = false,
            HintText = "Parti sayısı sınırı 999 olur; açıkken yukarıdaki ekleme yok sayılır" + RulesHint)]
        [SettingPropertyGroup(RulesGroup, GroupOrder = 0)]
        public bool NoPartyLimit { get; set; }

        // ---- Entrika ----

        [SettingPropertyDropdown("Entrika zorluğu", Order = 0, RequireRestart = false,
            HintText = "Kolay: başarı +10, yakalanma -3. Zor: başarı -10, yakalanma +3. Yapay zekânın sana kurduğu entrikalarda tersine işler.")]
        [SettingPropertyGroup(IntrigueGroup, GroupOrder = 1)]
        public Dropdown<string> IntrigueDifficulty { get; set; } = new Dropdown<string>(DifficultyNames, 1);

        [SettingPropertyDropdown("Yapay zekâ sana entrika kursun", Order = 1, RequireRestart = false,
            HintText = "Senden nefret eden lordlar ve kan davalılar klanına suikast/kaçırma planlar. Az: aday başına haftalık %3, aynı anda 1. Normal: %6, aynı anda 2.")]
        [SettingPropertyGroup(IntrigueGroup, GroupOrder = 1)]
        public Dropdown<string> AiIntrigue { get; set; } = new Dropdown<string>(AiNames, 1);

        [SettingPropertyBool("Yapay zekâ senin kendine de suikast kurabilsin", Order = 2, RequireRestart = false,
            HintText = "Kapalıyken yalnızca ailen ve yoldaşların hedef olur. Açıkken sen de ölebilirsin: varisin yoksa kampanya biter.")]
        [SettingPropertyGroup(IntrigueGroup, GroupOrder = 1)]
        public bool PlayerCanBeTargeted { get; set; }

        // ---- Harita ----

        [SettingPropertyDropdown("İleri sarma hızı", Order = 0, RequireRestart = false,
            HintText = "Haritadaki >> düğmesinin hızı (oyunun varsayılanı 4x)")]
        [SettingPropertyGroup(MapGroup, GroupOrder = 2)]
        public Dropdown<string> FastForward { get; set; } = Choices(ModSettings.AllowedSpeeds, v => v + "x");

        // ---- Arayüz ----

        [SettingPropertyDropdown("Entrika ekranı kısayolu", Order = 0, RequireRestart = false,
            HintText = "Haritadayken entrika ekranını açar/kapatır. Oyunun kendi harita tuşlarıyla çakışmayanlar listelenir. Insert menüsü her zaman çalışır.")]
        [SettingPropertyGroup(UiGroup, GroupOrder = 3)]
        public Dropdown<string> ScreenHotkey { get; set; } = new Dropdown<string>(ModSettings.ScreenHotkeyNames, ModSettings.DefaultScreenHotkey);

        [SettingPropertyDropdown("Haritadaki Entrika düğmesi", Order = 1, RequireRestart = false,
            HintText = "Haritanın üst kenarındaki düğmenin yeri. Rozet: harekete geçmeye hazır entrika ve sezilen tehdit sayısı.")]
        [SettingPropertyGroup(UiGroup, GroupOrder = 3)]
        public Dropdown<string> MapButtonPosition { get; set; } = new Dropdown<string>(ModSettings.MapButtonPositionNames, ModSettings.DefaultMapButtonPosition);

        [SettingPropertyBool("Resimli olay kartları", Order = 2, RequireRestart = false,
            HintText = "Entrika olayları, kararları ve sonuçları resimli kartla gösterilir (resim yoksa kişinin portresi). Kapalıyken oyunun düz seçim penceresi kullanılır.")]
        [SettingPropertyGroup(UiGroup, GroupOrder = 3)]
        public bool EventCards { get; set; } = true;

        public TacVeHancerSettings()
        {
            CopyFrom(SettingsStore.Current);
            PropertyChanged += OnSettingsEvent;
        }

        /// <summary>SubModule yüklenirken bir kez: Insert menüsünden yapılan değişiklikler MCM'e de yansısın.</summary>
        public static void Initialize() => SettingsStore.Saved += OnStoreSaved;

        private void OnSettingsEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != SaveTriggered && e.PropertyName != LoadingComplete) return;
            if (_syncing) return;
            _syncing = true;
            try
            {
                ApplyTo(SettingsStore.Current);
                SettingsStore.Save();
                TimeSpeed.MarkChanged();
            }
            finally
            {
                _syncing = false;
            }
        }

        private static void OnStoreSaved()
        {
            TacVeHancerSettings instance = Instance;
            if (_syncing || instance == null) return;
            _syncing = true;
            try
            {
                instance.CopyFrom(SettingsStore.Current);
                BaseSettingsProvider.Instance?.SaveSettings(instance);
            }
            finally
            {
                _syncing = false;
            }
        }

        private void CopyFrom(ModSettings s)
        {
            SkillXp.SelectedIndex = IndexOf(ModSettings.AllowedXpMultipliers, s.SkillXpMultiplier);
            TroopXp.SelectedIndex = IndexOf(ModSettings.AllowedXpMultipliers, s.TroopXpMultiplier);
            PartySize.SelectedIndex = IndexOf(ModSettings.AllowedPartySizeBonuses, s.PartySizeBonus);
            NoLearningLimit = s.NoLearningLimit;
            NoCompanionLimit = s.NoCompanionLimit;
            PartyLimitBonus = s.PartyLimitBonus;
            NoPartyLimit = s.NoPartyLimit;
            IntrigueDifficulty.SelectedIndex = SchemeMath.Clamp(s.IntrigueDifficulty, 0, DifficultyNames.Length - 1);
            AiIntrigue.SelectedIndex = SchemeMath.Clamp(s.AiIntrigue, 0, AiNames.Length - 1);
            PlayerCanBeTargeted = s.PlayerCanBeTargeted;
            FastForward.SelectedIndex = IndexOf(ModSettings.AllowedSpeeds, s.FastForwardSpeed);
            ScreenHotkey.SelectedIndex = s.ScreenHotkey;
            MapButtonPosition.SelectedIndex = s.MapButtonPosition;
            EventCards = s.EventCards;
        }

        private void ApplyTo(ModSettings s)
        {
            s.SetSkillXpMultiplier(At(ModSettings.AllowedXpMultipliers, SkillXp.SelectedIndex));
            s.SetTroopXpMultiplier(At(ModSettings.AllowedXpMultipliers, TroopXp.SelectedIndex));
            s.SetPartySizeBonus(At(ModSettings.AllowedPartySizeBonuses, PartySize.SelectedIndex));
            s.NoLearningLimit = NoLearningLimit;
            s.NoCompanionLimit = NoCompanionLimit;
            s.SetPartyLimitBonus(PartyLimitBonus);
            s.NoPartyLimit = NoPartyLimit;
            s.SetIntrigueDifficulty(IntrigueDifficulty.SelectedIndex);
            s.SetAiIntrigue(AiIntrigue.SelectedIndex);
            s.PlayerCanBeTargeted = PlayerCanBeTargeted;
            s.SetFastForwardSpeed(At(ModSettings.AllowedSpeeds, FastForward.SelectedIndex));
            s.SetScreenHotkey(ScreenHotkey.SelectedIndex);
            s.SetMapButtonPosition(MapButtonPosition.SelectedIndex);
            s.EventCards = EventCards;
        }

        private static Dropdown<string> Choices(int[] values, Func<int, string> label) =>
            new Dropdown<string>(Array.ConvertAll(values, v => label(v)), 0);

        private static int IndexOf(int[] values, int value) => Math.Max(0, Array.IndexOf(values, value));

        private static int At(int[] values, int index) => values[SchemeMath.Clamp(index, 0, values.Length - 1)];
    }
}

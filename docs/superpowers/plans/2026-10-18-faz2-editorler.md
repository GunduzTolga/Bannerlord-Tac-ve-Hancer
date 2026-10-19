# Faz 2 — Editörler Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** CheatMenu'ya Kahraman editörü, İlişkiler, Yerleşim editörü, Eşya ekle kategorilerini ve hatırlanan ileri sarma hızını (4x/8x/16x) eklemek.

**Architecture:** Faz 1'in `Picker` + popup altyapısı üstüne; her editör `Cheats/` altında oyunun eylemlerini çağıran küçük bir statik sınıf, menüleri `Categories` partial sınıfının ayrı dosyalarında. Saf mantık (aralıklı sayı, ayar dosyası) ayrı ve test edilir. Harmony yok, özel kayıt tipi yok, konsol hilesi çağrısı yok.

**Tech Stack:** C# 9, .NET Framework 4.8.1, VS 2026 MSBuild, Bannerlord v1.4.8 + War Sails v1.2.8.

**Spec:** `docs/superpowers/specs/2026-10-18-faz2-editorler-design.md`

**Git yok:** commit adımı yok; her görev "derleme yeşil" ile biter. Komut: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`

---

## Dosya yapısı

| Dosya | Durum | Sorumluluk |
|---|---|---|
| `src/AmountParser.cs` | değişir | `TryParseInRange` (eksi destekli) |
| `src/ModSettings.cs` | yeni | ayar metni okuma/yazma, izinli hızlar |
| `tests/CheatMenu.Tests/*` | değişir | yeni testler + ModSettings.cs bağlantısı |
| `src/SettingsStore.cs` | yeni | `Belgeler\...\Configs\CheatMenu.txt` okuma/yazma |
| `src/Cheats/TimeSpeed.cs` | yeni | ileri sarma hızını uygula + kaydet |
| `src/Cheats/HeroEditor.cs` | yeni | yaş, yetenek, nitelik, odak, perk, kişilik, berber |
| `src/Cheats/Relations.cs` | yeni | ilişki oku/ayarla |
| `src/Cheats/SettlementEditor.cs` | yeni | sahiplik, istatistikler, projeler |
| `src/Cheats/ItemSpawner.cs` | yeni | eşya türleri, liste, ekleme |
| `src/Menu/MenuUI.cs` | değişir | `AskNumberInRange` |
| `src/Menu/MenuEntry.cs` | değişir | `Range` satırı |
| `src/Menu/Picker.cs` | değişir | isteğe bağlı ikon + sıralama |
| `src/Menu/Categories.cs` | değişir | `partial`; kök + hileler (Harita/Adlandır taşınır) |
| `src/Menu/Categories.World.cs` | yeni | Harita (+ hız) ve Adlandır (Faz 1 kodu taşınır) |
| `src/Menu/Categories.Editors.cs` | yeni | Faz 2 menüleri |
| `src/SubModule.cs` | değişir | `TimeSpeed.Tick()` |

---

### Task 1: Saf mantık (TDD) — TryParseInRange + ModSettings

- [ ] **Step 1: Test bağlantısı** — `CheatMenu.Tests.csproj`, NameRules satırının altına:
  `<Compile Include="..\..\src\ModSettings.cs"><Link>ModSettings.cs</Link></Compile>`

- [ ] **Step 2: Başarısız testler** — `Program.cs` `Main` içinde özet satırından önce:

```csharp
            // AmountParser.TryParseInRange: eksi destekli, [min, max] dahil.
            ExpectRange("-100", -100, 100, true, -100);
            ExpectRange("100", -100, 100, true, 100);
            ExpectRange("101", -100, 100, false);
            ExpectRange("-101", -100, 100, false);
            ExpectRange(" 5 ", 0, 10, true, 5);
            ExpectRange("0", 0, 10, true, 0);
            ExpectRange("+2", -2, 2, true, 2);
            ExpectRange("abc", 0, 10, false);
            ExpectRange("", 0, 10, false);
            ExpectRange(null, 0, 10, false);

            // ModSettings: okuma/yazma, izinli hızlar.
            Check("settings: null -> 4", ModSettings.Parse(null).FastForwardSpeed == 4);
            Check("settings: 8", ModSettings.Parse("FastForwardSpeed=8").FastForwardSpeed == 8);
            Check("settings: CRLF 16", ModSettings.Parse("FastForwardSpeed=16\r\n").FastForwardSpeed == 16);
            Check("settings: 32 not allowed -> 4", ModSettings.Parse("FastForwardSpeed=32").FastForwardSpeed == 4);
            Check("settings: garbage -> 4", ModSettings.Parse("FastForwardSpeed=abc").FastForwardSpeed == 4);
            Check("settings: comments/unknown keys", ModSettings.Parse("# yorum\nFoo=1\nFastForwardSpeed=8").FastForwardSpeed == 8);
            var roundTrip = new ModSettings();
            roundTrip.SetFastForwardSpeed(16);
            Check("settings: round trip", ModSettings.Parse(roundTrip.Format()).FastForwardSpeed == 16);
```

ve yardımcılar:

```csharp
        private static void ExpectRange(string input, int min, int max, bool expectedOk, int expectedValue = 0)
        {
            bool ok = AmountParser.TryParseInRange(input, min, max, out int value);
            Check("range \"" + input + "\" [" + min + "," + max + "] -> ok=" + ok + ", value=" + value,
                ok == expectedOk && (!expectedOk || value == expectedValue));
        }

        private static void Check(string label, bool pass)
        {
            if (!pass) _failures++;
            Console.WriteLine((pass ? "PASS " : "FAIL ") + label);
        }
```

- [ ] **Step 3: Kırmızı** — build.ps1 → Expected: `CS2001 ... ModSettings.cs could not be found`.

- [ ] **Step 4: Uygula** — `AmountParser`'a:

```csharp
        /// <summary>Eksi değerlere izin veren, [min, max] aralığına sınırlı okuma (ilişki, kişilik, yaş vb.).</summary>
        public static bool TryParseInRange(string input, int min, int max, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            if (!int.TryParse(input.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int parsed)) return false;
            if (parsed < min || parsed > max) return false;
            value = parsed;
            return true;
        }
```

`src/ModSettings.cs`:

```csharp
using System;
using System.Globalization;

namespace CheatMenu
{
    /// <summary>
    /// Kalıcı mod ayarları ("Anahtar=Değer" satırları). Saf: dosya işi SettingsStore'da.
    /// Oyunsuz test edilir (tests/CheatMenu.Tests).
    /// </summary>
    public sealed class ModSettings
    {
        public static readonly int[] AllowedSpeeds = { 4, 8, 16 };
        public const int DefaultSpeed = 4;

        private const string SpeedKey = "FastForwardSpeed";

        public int FastForwardSpeed { get; private set; } = DefaultSpeed;

        /// <summary>İzinli listede olmayan değer varsayılana döner.</summary>
        public void SetFastForwardSpeed(int speed) =>
            FastForwardSpeed = Array.IndexOf(AllowedSpeeds, speed) >= 0 ? speed : DefaultSpeed;

        /// <summary>Bilinmeyen anahtarlar, yorumlar (#) ve bozuk satırlar yok sayılır.</summary>
        public static ModSettings Parse(string text)
        {
            var settings = new ModSettings();
            if (string.IsNullOrEmpty(text)) return settings;

            foreach (string rawLine in text.Split('\n'))
            {
                string line = rawLine.Trim();
                int separator = line.IndexOf('=');
                if (line.StartsWith("#") || separator <= 0) continue;

                string key = line.Substring(0, separator).Trim();
                string value = line.Substring(separator + 1).Trim();
                if (key == SpeedKey && int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int speed))
                    settings.SetFastForwardSpeed(speed);
            }
            return settings;
        }

        public string Format() =>
            "# CheatMenu ayarlari\n" + SpeedKey + "=" + FastForwardSpeed.ToString(CultureInfo.InvariantCulture) + "\n";
    }
}
```

- [ ] **Step 5: Yeşil** — build.ps1 → Expected: 35 `PASS`, `ALL TESTS PASSED`.

---

### Task 2: Altyapı — Picker ikon/sıralama, AskNumberInRange, Range satırı

- [ ] **Step 1: Picker** — `using TaleWorlds.Core.ImageIdentifiers;` ekle; imza ve eleman kurulumu:

```csharp
        public static void Pick<T>(string title, IEnumerable<T> items, Func<T, string> label, Action<T> onPicked, Action onBack,
            Func<T, ImageIdentifier> image = null, bool sort = true)
            where T : class
        {
            IEnumerable<InquiryElement> query = items.Select(item => new InquiryElement(item, label(item), image?.Invoke(item)));
            if (sort) query = query.OrderBy(element => element.Title, StringComparer.CurrentCultureIgnoreCase);
            List<InquiryElement> elements = query.ToList();
```

(gövdenin geri kalanı aynı.)

- [ ] **Step 2: MenuUI.AskNumberInRange** — `using System.Globalization;` ekle; `AskNumber`'ın altına:

```csharp
        /// <summary>[min, max] aralığında sayı sorar (eksi olabilir); kutu mevcut değerle dolu açılır.</summary>
        public static void AskNumberInRange(string title, int min, int max, int current, Action<int> apply, Action onDone)
        {
            string hint = min + " ile " + max + " arasında bir sayı yaz:";
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    title, hint, true, true, "Uygula", "İptal",
                    text =>
                    {
                        if (AmountParser.TryParseInRange(text, min, max, out int value)) SafeRun(title, () => apply(value));
                        else Error("Geçersiz değer: " + text);
                        Defer(onDone);
                    },
                    () => Defer(onDone),
                    false,
                    text => AmountParser.TryParseInRange(text, min, max, out _)
                        ? new Tuple<bool, string>(true, string.Empty)
                        : new Tuple<bool, string>(false, hint),
                    "",
                    current.ToString(CultureInfo.InvariantCulture)),
                true, false);
        }
```

- [ ] **Step 3: MenuEntry.Range** — `Text`'in altına:

```csharp
        /// <summary>Aralıklı sayı soran eylem; kutu mevcut değerle dolu açılır.</summary>
        public static MenuEntry Range(string label, int min, int max, Func<int> current, Action<int> apply) =>
            new MenuEntry(label, reopen => MenuUI.AskNumberInRange(label, min, max, current(), apply, reopen));
```

- [ ] **Step 4: Derle** — build.ps1 → hata yok.

---

### Task 3: Zaman hızı (SettingsStore + TimeSpeed)

- [ ] **Step 1:** `src/SettingsStore.cs`:

```csharp
using System;
using System.IO;

namespace CheatMenu
{
    /// <summary>
    /// Ayarları Belgeler\Mount and Blade II Bannerlord\Configs\CheatMenu.txt dosyasında tutar.
    /// Kayıt (save) dosyalarına hiçbir şey yazmaz; dosya okunamazsa varsayılanlarla devam eder.
    /// </summary>
    public static class SettingsStore
    {
        private static ModSettings _current;

        public static ModSettings Current => _current ?? (_current = Load());

        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord", "Configs", "CheatMenu.txt");

        /// <returns>Yazma başarılıysa true.</returns>
        public static bool Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, Current.Format());
                return true;
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[CheatMenu] settings save failed: " + ex);
                return false;
            }
        }

        private static ModSettings Load()
        {
            try
            {
                return File.Exists(FilePath) ? ModSettings.Parse(File.ReadAllText(FilePath)) : new ModSettings();
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[CheatMenu] settings load failed: " + ex);
                return new ModSettings();
            }
        }
    }
}
```

- [ ] **Step 2:** `src/Cheats/TimeSpeed.cs`:

```csharp
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// İleri sarma (">>") hızı. Oyun Campaign.SpeedUpMultiplier'ı kaydetmiyor ve her yüklemede 4'e sıfırlıyor;
    /// bu yüzden seçili değer her karede yeniden uygulanır. Seçili değer varsayılanken (4x) dokunulmaz,
    /// böylece Better Time gibi modlarla çakışılmaz.
    /// </summary>
    public static class TimeSpeed
    {
        private static bool _resetPending;

        public static int Current => SettingsStore.Current.FastForwardSpeed;

        public static void Set(int speed)
        {
            SettingsStore.Current.SetFastForwardSpeed(speed);
            _resetPending = true;
            Tick();
            if (SettingsStore.Save()) MenuUI.Success("İleri sarma hızı: " + Current + "x");
            else MenuUI.Error("İleri sarma hızı " + Current + "x oldu ama ayar dosyasına yazılamadı");
        }

        public static void Tick()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null) return;
            int wanted = Current;
            if (wanted == ModSettings.DefaultSpeed && !_resetPending) return;
            if (campaign.SpeedUpMultiplier != wanted) campaign.SpeedUpMultiplier = wanted;
            _resetPending = false;
        }
    }
}
```

- [ ] **Step 3: SubModule** — `OnApplicationTick`'te `if (Campaign.Current == null) return;` satırının hemen altına `TimeSpeed.Tick();`.

- [ ] **Step 4: Derle** — hata yok.

---

### Task 4: Editör eylemleri (HeroEditor, Relations, SettlementEditor, ItemSpawner)

- [ ] **Step 1:** `src/Cheats/HeroEditor.cs`:

```csharp
using System;
using CheatMenu.Menu;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Kahraman düzenleme. Oyunun konsol hilelerinin içinde kullandığı eylemler doğrudan çağrılır
    /// (konsol hileleri değil; onlar kayda "hile kullanıldı" işareti koyar). Hepsi oyunun kendi kayıt alanlarına yazar.
    /// </summary>
    public static class HeroEditor
    {
        public const int MaxSkill = 330;

        public static int MinAge => Campaign.Current.Models.AgeModel.HeroComesOfAge;
        public static int MaxAge => Campaign.Current.Models.AgeModel.MaxAge;
        public static int MaxAttribute => Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute;
        public static int MaxFocus => Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill;

        public static void SetAge(Hero hero, int age)
        {
            hero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge(age));
            MenuUI.Success(hero.Name + " artık " + (int)hero.Age + " yaşında");
        }

        // campaign.set_skill_player ile aynı iki adım.
        public static void SetSkill(Hero hero, SkillObject skill, int value)
        {
            hero.HeroDeveloper.SetInitialSkillLevel(skill, value);
            hero.HeroDeveloper.InitializeSkillXp(skill);
            MenuUI.Success(hero.Name + ": " + skill.Name + " = " + hero.GetSkillValue(skill));
        }

        public static void SetAllSkills(Hero hero, int value)
        {
            foreach (SkillObject skill in Skills.All)
            {
                hero.HeroDeveloper.SetInitialSkillLevel(skill, value);
                hero.HeroDeveloper.InitializeSkillXp(skill);
            }
            MenuUI.Success(hero.Name + ": tüm yetenekler " + value);
        }

        public static void SetAttribute(Hero hero, CharacterAttribute attribute, int value)
        {
            int delta = value - hero.GetAttributeValue(attribute);
            if (delta > 0) hero.HeroDeveloper.AddAttribute(attribute, delta, false);
            else if (delta < 0) hero.HeroDeveloper.RemoveAttribute(attribute, -delta);
            MenuUI.Success(hero.Name + ": " + attribute.Name + " = " + hero.GetAttributeValue(attribute));
        }

        public static void SetFocus(Hero hero, SkillObject skill, int value)
        {
            int delta = value - hero.HeroDeveloper.GetFocus(skill);
            if (delta > 0) hero.HeroDeveloper.AddFocus(skill, delta, false);
            else if (delta < 0) hero.HeroDeveloper.RemoveFocus(skill, -delta);
            MenuUI.Success(hero.Name + ": " + skill.Name + " odağı = " + hero.HeroDeveloper.GetFocus(skill));
        }

        public static void AddPerk(Hero hero, PerkObject perk)
        {
            hero.HeroDeveloper.AddPerk(perk);
            MenuUI.Success(hero.Name + " perk kazandı: " + perk.Name);
        }

        // campaign.set_player_trait ile aynı: ana kahramanda trait XP'si de güncellenir, yoksa seviye geri kayabilir.
        public static void SetTrait(Hero hero, TraitObject trait, int value)
        {
            int previous = hero.GetTraitLevel(trait);
            hero.SetTraitLevel(trait, value);
            if (hero == Hero.MainHero)
            {
                TraitLevelingHelper.UpdateTraitXPAccordingToTraitLevels();
                CampaignEventDispatcher.Instance.OnPlayerTraitChanged(trait, previous);
            }
            MenuUI.Success(hero.Name + ": " + trait.Name + " = " + hero.GetTraitLevel(trait));
        }

        /// <summary>Oyunun berber ekranı; yalnızca ana kahraman (ekran Hero.MainHero'ya bağlı).</summary>
        public static void OpenBarber()
        {
            if (Hero.MainHero == null) throw new InvalidOperationException("Ana karakter bulunamadı");
            if (Mission.Current != null) throw new InvalidOperationException("Sahne içindeyken berber ekranı açılamaz");
            GameStateManager states = Game.Current.GameStateManager;
            states.PushState(states.CreateState<BarberState>(Hero.MainHero.CharacterObject, CharacterHelper.GetFaceGeneratorFilter()), 0);
        }
    }
}
```

- [ ] **Step 2:** `src/Cheats/Relations.cs`:

```csharp
using System;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Cheats
{
    /// <summary>Oyuncunun bir kahramanla ilişkisi. Tam değer Hero.SetPersonalRelation ile (oyun sınırlara kırpar).</summary>
    public static class Relations
    {
        public static int Min => Campaign.Current.Models.DiplomacyModel.MinRelationLimit;
        public static int Max => Campaign.Current.Models.DiplomacyModel.MaxRelationLimit;

        public static int Get(Hero hero) => Hero.MainHero.GetRelation(hero);

        public static void Set(Hero hero, int value)
        {
            Hero.MainHero.SetPersonalRelation(hero, Math.Max(Min, Math.Min(Max, value)));
            MenuUI.Success(hero.Name + " ile ilişki: " + Get(hero));
        }

        public static void Add(Hero hero, int delta) => Set(hero, Get(hero) + delta);
    }
}
```

- [ ] **Step 3:** `src/Cheats/SettlementEditor.cs`:

```csharp
using System;
using CheatMenu.Menu;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Yerleşim düzenleme. Kuşatma, yağma ve savaş sırasında oyunun kendi hilesi gibi reddeder.
    /// Sahiplik oyunun eylemiyle değişir (vali ve krallık olayları doğru tetiklenir).
    /// </summary>
    public static class SettlementEditor
    {
        public static string GetBlocker(Settlement settlement)
        {
            if (settlement.IsUnderSiege) return settlement.Name + " kuşatma altında";
            if (settlement.IsUnderRaid) return settlement.Name + " yağmalanıyor";
            if (settlement.Party.MapEvent != null) return settlement.Name + " şu an bir savaşın içinde";
            return null;
        }

        public static void ChangeOwner(Settlement settlement, Clan clan)
        {
            Guard(settlement);
            if (settlement.IsVillage) throw new InvalidOperationException("Köyler bağlı oldukları şehirle birlikte el değiştirir");
            if (clan == Clan.PlayerClan && clan.IsUnderMercenaryService)
                throw new InvalidOperationException("Paralı askerken yerleşim sahibi olamazsın");
            Hero newOwner = clan.Leader ?? throw new InvalidOperationException(clan.Name + " klanının lideri yok");
            ChangeOwnerOfSettlementAction.ApplyByDefault(newOwner, settlement);
            MenuUI.Success(settlement.Name + " artık " + clan.Name + " klanının");
        }

        public static void AddProsperity(Settlement settlement, int amount)
        {
            Town town = RequireTown(settlement);
            town.Prosperity += amount;
            MenuUI.Success(settlement.Name + " refahı: " + town.Prosperity.ToString("N0"));
        }

        public static void MaxLoyalty(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            town.Loyalty = 100f;
            MenuUI.Success(settlement.Name + " sadakati: " + town.Loyalty.ToString("0"));
        }

        public static void MaxSecurity(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            town.Security = 100f;
            MenuUI.Success(settlement.Name + " güvenliği: " + town.Security.ToString("0"));
        }

        public static void FillFood(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            town.FoodStocks = town.FoodStocksUpperLimit();
            MenuUI.Success(settlement.Name + " yiyecek deposu: " + town.FoodStocks.ToString("0"));
        }

        /// <summary>
        /// CheckIfBuildingIsComplete hangi binayı verirsen ver kuyruğun BAŞINI siler; bu yüzden her turda Peek().
        /// </summary>
        public static void FinishAllProjects(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            int finished = 0;
            while (town.BuildingsInProgress.Count > 0 && finished < 100)
            {
                Building building = town.BuildingsInProgress.Peek();
                building.BuildingProgress = building.GetConstructionCost();
                BuildingHelper.CheckIfBuildingIsComplete(building);
                finished++;
            }
            MenuUI.Success(settlement.Name + ": " + finished + " proje tamamlandı");
        }

        public static void AddHearth(Settlement settlement, int amount)
        {
            Guard(settlement);
            Village village = settlement.Village ?? throw new InvalidOperationException(settlement.Name + " bir köy değil");
            village.Hearth += amount;
            MenuUI.Success(settlement.Name + " hane: " + village.Hearth.ToString("0"));
        }

        public static void AddMilitia(Settlement settlement, int amount)
        {
            Guard(settlement);
            settlement.Militia += amount;
            MenuUI.Success(settlement.Name + " milis: " + settlement.Militia.ToString("0"));
        }

        private static Town RequireTown(Settlement settlement)
        {
            Guard(settlement);
            return settlement.Town ?? throw new InvalidOperationException(settlement.Name + " bir şehir ya da kale değil");
        }

        private static void Guard(Settlement settlement)
        {
            string blocker = GetBlocker(settlement);
            if (blocker != null) throw new InvalidOperationException(blocker);
        }
    }
}
```

- [ ] **Step 4:** `src/Cheats/ItemSpawner.cs`:

```csharp
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.ObjectSystem;

namespace CheatMenu.Cheats
{
    /// <summary>Envantere eşya ekleme. Satılık olmayan özel eşyalar (NotMerchandise) listelenmez.</summary>
    public static class ItemSpawner
    {
        public sealed class Category
        {
            public Category(string label, ItemObject.ItemTypeEnum type)
            {
                Label = label;
                Type = type;
            }

            public string Label { get; }
            public ItemObject.ItemTypeEnum Type { get; }
        }

        public static readonly Category[] Categories =
        {
            new Category("Tek elli silah", ItemObject.ItemTypeEnum.OneHandedWeapon),
            new Category("İki elli silah", ItemObject.ItemTypeEnum.TwoHandedWeapon),
            new Category("Mızrak / uzun silah", ItemObject.ItemTypeEnum.Polearm),
            new Category("Yay", ItemObject.ItemTypeEnum.Bow),
            new Category("Arbalet", ItemObject.ItemTypeEnum.Crossbow),
            new Category("Ok", ItemObject.ItemTypeEnum.Arrows),
            new Category("Cıvata", ItemObject.ItemTypeEnum.Bolts),
            new Category("Fırlatma silahı", ItemObject.ItemTypeEnum.Thrown),
            new Category("Kalkan", ItemObject.ItemTypeEnum.Shield),
            new Category("Kafa zırhı", ItemObject.ItemTypeEnum.HeadArmor),
            new Category("Gövde zırhı", ItemObject.ItemTypeEnum.BodyArmor),
            new Category("Bacak zırhı", ItemObject.ItemTypeEnum.LegArmor),
            new Category("Eldiven", ItemObject.ItemTypeEnum.HandArmor),
            new Category("Pelerin", ItemObject.ItemTypeEnum.Cape),
            new Category("At", ItemObject.ItemTypeEnum.Horse),
            new Category("At koşumu", ItemObject.ItemTypeEnum.HorseHarness),
            new Category("Ticaret malı", ItemObject.ItemTypeEnum.Goods),
            new Category("Hayvan", ItemObject.ItemTypeEnum.Animal),
        };

        public static IEnumerable<ItemObject> ItemsOfType(ItemObject.ItemTypeEnum type) =>
            MBObjectManager.Instance.GetObjectTypeList<ItemObject>()
                .Where(item => item.ItemType == type && !item.NotMerchandise);

        public static string Label(ItemObject item) =>
            item.Name + " (T" + ((int)item.Tier + 1) + ", " + item.Value.ToString("N0") + " dinar)";

        public static ImageIdentifier Image(ItemObject item) => new ItemImageIdentifier(item, null);

        public static void Add(ItemObject item, int count)
        {
            MobileParty.MainParty.ItemRoster.AddToCounts(item, count);
            MenuUI.Success(count + " x " + item.Name + " envantere eklendi");
        }
    }
}
```

- [ ] **Step 5: Derle** — hata yok.

---

### Task 5: Menüler — Categories'i partial'a böl, Faz 2 menülerini ekle

- [ ] **Step 1: Categories.cs** — `public static class Categories` → `public static partial class Categories`; using'ler
  `System`, `System.Collections.Generic`, `CheatMenu.Cheats` ve `CampaignCheats` alias'ı kalır (TaleWorlds using'leri
  World dosyasına taşınır); `ShowMap` … `PickAndRename` arası (Faz 1 kodu) silinir; kök listeye eklenir:

```csharp
                MenuEntry.Submenu("Kahraman editörü", ShowHeroEditorPick),
                MenuEntry.Submenu("İlişkiler", ShowRelationPick),
                MenuEntry.Submenu("Yerleşim editörü", ShowSettlementPick),
                MenuEntry.Submenu("Eşya ekle", ShowItemCategories),
```

- [ ] **Step 2: Categories.World.cs** — Faz 1 kodu birebir taşınır; `ShowMap` hız satırlarıyla:

```csharp
        private static void ShowMap()
        {
            var entries = new List<MenuEntry>
            {
                MenuEntry.Submenu("Yerleşime ışınlan...", PickSettlementToTeleport),
                MenuEntry.Submenu("Bir lordun yanına ışınlan...", PickHeroToTeleport),
            };
            foreach (int speed in ModSettings.AllowedSpeeds)
                entries.Add(MenuEntry.Simple(SpeedLabel(speed), () => TimeSpeed.Set(speed)));

            ShowCategory("Harita", entries, ShowMap,
                "İpucu: Ctrl+Alt + Sol tık ile haritada tıkladığın yere ışınlanırsın. "
                + "İleri sarma hızı yalnızca >> düğmesini etkiler.");
        }

        private static string SpeedLabel(int speed) =>
            "İleri sarma hızı " + speed + "x" + (speed == ModSettings.DefaultSpeed ? " (normal)" : "")
            + (TimeSpeed.Current == speed ? "  [SEÇİLİ]" : "");
```

- [ ] **Step 3: Categories.Editors.cs** — yeni dosya:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace CheatMenu.Menu
{
    /// <summary>Faz 2: kahraman editörü, ilişkiler, yerleşim editörü, eşya ekleme.</summary>
    public static partial class Categories
    {
        // ---- Kahraman editörü ----

        private static void ShowHeroEditorPick()
        {
            var heroes = new List<Hero> { Hero.MainHero };
            heroes.AddRange(Lists.LordsAndCompanions(true).OrderBy(h => h.Name.ToString(), StringComparer.CurrentCultureIgnoreCase));
            Picker.Pick("Kahraman editörü", heroes,
                h => h == Hero.MainHero ? "[Ben] " + h.Name : Lists.HeroLabel(h),
                ShowHero, ShowRoot, sort: false);
        }

        private static void ShowHero(Hero hero)
        {
            var entries = new List<MenuEntry>
            {
                MenuEntry.Range("Yaş: " + (int)hero.Age, HeroEditor.MinAge, HeroEditor.MaxAge,
                    () => (int)hero.Age, age => HeroEditor.SetAge(hero, age)),
                MenuEntry.Submenu("Bir yeteneği ayarla...", () => PickSkill(hero, "Yetenek",
                    skill => hero.GetSkillValue(skill), HeroEditor.MaxSkill, (skill, v) => HeroEditor.SetSkill(hero, skill, v))),
                MenuEntry.Range("Tüm yetenekleri ayarla...", 0, HeroEditor.MaxSkill, () => 100,
                    v => HeroEditor.SetAllSkills(hero, v)),
                MenuEntry.Submenu("Bir niteliği ayarla...", () => PickAttribute(hero)),
                MenuEntry.Submenu("Bir yeteneğin odağını ayarla...", () => PickSkill(hero, "Odak",
                    skill => hero.HeroDeveloper.GetFocus(skill), HeroEditor.MaxFocus, (skill, v) => HeroEditor.SetFocus(hero, skill, v))),
                MenuEntry.Submenu("Perk ekle...", () => PickPerk(hero)),
                MenuEntry.Submenu("Kişilik özelliğini ayarla...", () => PickTrait(hero)),
            };
            if (hero == Hero.MainHero)
                entries.Add(MenuEntry.Submenu("Görünümümü düzenle (berber ekranı)", HeroEditor.OpenBarber));

            MenuUI.ShowList("Kahraman: " + hero.Name, entries, () => ShowHero(hero), "Geri", ShowHeroEditorPick);
        }

        private static void PickSkill(Hero hero, string what, Func<SkillObject, int> current, int max,
            Action<SkillObject, int> apply)
        {
            Picker.Pick(hero.Name + " - " + what, Skills.All, skill => skill.Name + ": " + current(skill),
                skill => MenuUI.AskNumberInRange(what + ": " + skill.Name, 0, max, current(skill),
                    v => apply(skill, v), () => ShowHero(hero)),
                () => ShowHero(hero), sort: false);
        }

        private static void PickAttribute(Hero hero)
        {
            Picker.Pick(hero.Name + " - Nitelik", Attributes.All, a => a.Name + ": " + hero.GetAttributeValue(a),
                a => MenuUI.AskNumberInRange("Nitelik: " + a.Name, 0, HeroEditor.MaxAttribute, hero.GetAttributeValue(a),
                    v => HeroEditor.SetAttribute(hero, a, v), () => ShowHero(hero)),
                () => ShowHero(hero), sort: false);
        }

        private static void PickPerk(Hero hero)
        {
            Picker.Pick(hero.Name + " - Perk ekle", PerkObject.All.Where(p => !hero.GetPerkValue(p)),
                p => p.Name + " (" + p.Skill?.Name + " " + (int)p.RequiredSkillValue + ")",
                p =>
                {
                    MenuUI.SafeRun("Perk ekle", () => HeroEditor.AddPerk(hero, p));
                    MenuUI.Defer(() => PickPerk(hero));
                },
                () => ShowHero(hero));
        }

        private static void PickTrait(Hero hero)
        {
            Picker.Pick(hero.Name + " - Kişilik", DefaultTraits.Personality, t => t.Name + ": " + hero.GetTraitLevel(t),
                t => MenuUI.AskNumberInRange("Kişilik: " + t.Name, t.MinValue, t.MaxValue, hero.GetTraitLevel(t),
                    v => HeroEditor.SetTrait(hero, t, v), () => ShowHero(hero)),
                () => ShowHero(hero), sort: false);
        }

        // ---- İlişkiler ----

        private static void ShowRelationPick()
        {
            Picker.Pick("İlişkiler", Lists.LordsAndCompanions(false).Concat(Lists.Notables()),
                h => Lists.HeroLabel(h) + " [" + Relations.Get(h) + "]", ShowRelation, ShowRoot);
        }

        private static void ShowRelation(Hero hero)
        {
            MenuUI.ShowList(hero.Name + " ile ilişki: " + Relations.Get(hero), new List<MenuEntry>
            {
                MenuEntry.Simple("+10", () => Relations.Add(hero, 10)),
                MenuEntry.Simple("+50", () => Relations.Add(hero, 50)),
                MenuEntry.Simple("-10", () => Relations.Add(hero, -10)),
                MenuEntry.Simple("Tam dost (" + Relations.Max + ")", () => Relations.Set(hero, Relations.Max)),
                MenuEntry.Simple("Tam düşman (" + Relations.Min + ")", () => Relations.Set(hero, Relations.Min)),
                MenuEntry.Range("Özel değer...", Relations.Min, Relations.Max, () => Relations.Get(hero),
                    v => Relations.Set(hero, v)),
            }, () => ShowRelation(hero), "Geri", ShowRelationPick,
                "Not: oyun bazı ilişkileri kişinin klan liderine yönlendirir.");
        }

        // ---- Yerleşim editörü ----

        private static void ShowSettlementPick() =>
            Picker.Pick("Yerleşim editörü", Lists.Settlements(), Lists.SettlementLabel, ShowSettlement, ShowRoot);

        private static void ShowSettlement(Settlement s)
        {
            var entries = new List<MenuEntry>();
            if (!s.IsVillage)
                entries.Add(MenuEntry.Submenu("Sahibini değiştir... (şu an: " + s.OwnerClan?.Name + ")", () => PickNewOwner(s)));
            if (s.Town != null)
            {
                Town town = s.Town;
                entries.Add(MenuEntry.Simple("Refah +1000 (şu an " + town.Prosperity.ToString("N0") + ")",
                    () => SettlementEditor.AddProsperity(s, 1000)));
                entries.Add(MenuEntry.Custom("Özel miktar refah...", v => SettlementEditor.AddProsperity(s, v)));
                entries.Add(MenuEntry.Simple("Sadakati tam yap (şu an " + town.Loyalty.ToString("0") + ")",
                    () => SettlementEditor.MaxLoyalty(s)));
                entries.Add(MenuEntry.Simple("Güvenliği tam yap (şu an " + town.Security.ToString("0") + ")",
                    () => SettlementEditor.MaxSecurity(s)));
                entries.Add(MenuEntry.Simple("Yiyecek deposunu doldur (şu an " + town.FoodStocks.ToString("0") + ")",
                    () => SettlementEditor.FillFood(s)));
                entries.Add(MenuEntry.Simple("Tüm inşaat projelerini bitir (" + town.BuildingsInProgress.Count + " proje)",
                    () => SettlementEditor.FinishAllProjects(s)));
            }
            if (s.Village != null)
            {
                entries.Add(MenuEntry.Simple("Hane +100 (şu an " + s.Village.Hearth.ToString("0") + ")",
                    () => SettlementEditor.AddHearth(s, 100)));
                entries.Add(MenuEntry.Custom("Özel miktar hane...", v => SettlementEditor.AddHearth(s, v)));
            }
            entries.Add(MenuEntry.Simple("Milis +50 (şu an " + s.Militia.ToString("0") + ")",
                () => SettlementEditor.AddMilitia(s, 50)));

            MenuUI.ShowList(s.Name.ToString(), entries, () => ShowSettlement(s), "Geri", ShowSettlementPick);
        }

        private static void PickNewOwner(Settlement s)
        {
            var clans = new List<Clan> { Clan.PlayerClan };
            clans.AddRange(Lists.Clans()
                .Where(c => c != Clan.PlayerClan && c.Leader != null)
                .OrderBy(c => c.Name.ToString(), StringComparer.CurrentCultureIgnoreCase));
            Picker.Pick(s.Name + " - yeni sahip", clans,
                c => c == Clan.PlayerClan ? "[Senin klanın] " + c.Name : Lists.ClanLabel(c),
                c =>
                {
                    MenuUI.SafeRun("Sahiplik", () => SettlementEditor.ChangeOwner(s, c));
                    MenuUI.Defer(() => ShowSettlement(s));
                },
                () => ShowSettlement(s), sort: false);
        }

        // ---- Eşya ekle ----

        private static void ShowItemCategories()
        {
            List<MenuEntry> entries = ItemSpawner.Categories
                .Select(category => MenuEntry.Submenu(category.Label + "...", () => PickItem(category)))
                .ToList();
            ShowCategory("Eşya ekle", entries, ShowItemCategories);
        }

        private static void PickItem(ItemSpawner.Category category)
        {
            Picker.Pick(category.Label, ItemSpawner.ItemsOfType(category.Type), ItemSpawner.Label,
                item => MenuUI.AskNumberInRange(item.Name + " - adet", 1, 9999, 1,
                    count => ItemSpawner.Add(item, count), () => PickItem(category)),
                ShowItemCategories, ItemSpawner.Image);
        }
    }
}
```

- [ ] **Step 4: Derle + doğrula** — build.ps1 → hata yok; `scratchpad/verify_mod.ps1` → tüm tipler yüklenir,
  `references to TaleWorlds.CampaignSystem.CampaignCheats: 0`, `unknown-opcode lines: 0`.

---

### Task 6: Oyun içi kontrol listesi (kullanıcı)

- [ ] Kahraman editörü → [Ben] → Yaş 25 → ansiklopedide yaş 25.
- [ ] Bir yetenek (ör. Tek El) = 200; tüm yetenekler = 150; karakter ekranında görünür.
- [ ] Nitelik ve odak ayarı; perk ekle (alternatif perk dahil) → karakter ekranında iki perk birden.
- [ ] Kişilik (Merhamet +2) → ansiklopedide görünür; birkaç gün bekle → geri kaymıyor.
- [ ] Görünümümü düzenle → berber ekranı açılır, değişiklik kaydedilir.
- [ ] Bir yoldaşın yaşını/yeteneğini değiştir.
- [ ] İlişkiler → bir lord +50, Tam düşman, Özel değer -30.
- [ ] Yerleşim editörü → bir şehri kendi klanına ver; başka bir klana ver; refah, sadakat, güvenlik, yiyecek, milis;
  tüm projeleri bitir → şehir yönetim ekranında binalar bir üst seviyede.
- [ ] Köy → hane +100; kuşatma altındaki bir şehirde değişiklik → kırmızı ret mesajı.
- [ ] Eşya ekle → Tek elli silah → ikonlu liste, arama → 3 adet → envanterde.
- [ ] Harita → İleri sarma 16x [SEÇİLİ] → >> ile hız belirgin artar; kaydet-çık-yükle → hâlâ 16x;
  4x'e dön → normal.

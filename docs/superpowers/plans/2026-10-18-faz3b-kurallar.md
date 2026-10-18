# Faz 3B — Kural Çarpanları Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Kalıcı "Kurallar" kategorisi (yetenek XP, asker XP, parti boyutu, öğrenme sınırı) ve savaş hilelerine müttefik kapsamı.

**Architecture:** Dört oyun modeli `AddModel<T>` ile zincirlenir (BaseModel'e tam delege + tek üyede değişiklik); modeller ayarları `SettingsStore.Current`'tan canlı okur. Ayarlar `ModSettings` (saf, test edilir) ile config dosyasında.

**Tech Stack:** C# 9, .NET Framework 4.8.1, VS 2026 MSBuild, Bannerlord v1.4.8 + War Sails v1.2.8.

**Spec:** `docs/superpowers/specs/2026-10-18-faz3b-kurallar-design.md` · **Git yok.** Derleme: `build.ps1`.

---

## Dosya yapısı

| Dosya | Durum |
|---|---|
| `src/ModSettings.cs` | yeniden yazılır (yeni alanlar) |
| `tests/CheatMenu.Tests/Program.cs` | +9 test |
| `src/Cheats/Rules.cs` | yeni |
| `src/Cheats/Models/CheatGenericXpModel.cs`, `CheatCombatXpModel.cs`, `CheatPartySizeLimitModel.cs`, `CheatCharacterDevelopmentModel.cs`, `RuleModels.cs` | yeni |
| `src/CheatState.cs`, `src/Cheats/CheatTargets.cs` | `ArmyIncludesAllies` |
| `src/Menu/Categories.Rules.cs` | yeni |
| `src/Menu/Categories.cs` | kök + Savaş içi toggle |
| `src/SubModule.cs` | `RuleModels.Register` |

---

### Task 1: ModSettings (TDD)

- [ ] **Step 1: Başarısız testler** — `Program.cs`, "settings: round trip" satırının altına:

```csharp
            Check("settings: skill xp 5", ModSettings.Parse("SkillXpMultiplier=5").SkillXpMultiplier == 5);
            Check("settings: skill xp 4 not allowed -> 1", ModSettings.Parse("SkillXpMultiplier=4").SkillXpMultiplier == 1);
            Check("settings: troop xp 10", ModSettings.Parse("TroopXpMultiplier=10").TroopXpMultiplier == 10);
            Check("settings: party size 200", ModSettings.Parse("PartySizeBonus=200").PartySizeBonus == 200);
            Check("settings: party size -50 -> 0", ModSettings.Parse("PartySizeBonus=-50").PartySizeBonus == 0);
            Check("settings: no learning limit 1", ModSettings.Parse("NoLearningLimit=1").NoLearningLimit);
            Check("settings: no learning limit true", ModSettings.Parse("NoLearningLimit=true").NoLearningLimit);
            Check("settings: no learning limit 0", !ModSettings.Parse("NoLearningLimit=0").NoLearningLimit);
            var full = new ModSettings();
            full.SetFastForwardSpeed(8);
            full.SetSkillXpMultiplier(3);
            full.SetTroopXpMultiplier(5);
            full.SetPartySizeBonus(100);
            full.NoLearningLimit = true;
            ModSettings back = ModSettings.Parse(full.Format());
            Check("settings: full round trip", back.FastForwardSpeed == 8 && back.SkillXpMultiplier == 3
                && back.TroopXpMultiplier == 5 && back.PartySizeBonus == 100 && back.NoLearningLimit);
```

- [ ] **Step 2: Kırmızı** — build.ps1 → `CS1061: 'ModSettings' does not contain a definition for 'SkillXpMultiplier'`.

- [ ] **Step 3: Uygula** — `src/ModSettings.cs`:

```csharp
using System;
using System.Globalization;

namespace CheatMenu
{
    /// <summary>
    /// Kalıcı mod ayarları ("Anahtar=Değer" satırları). Saf: dosya işi SettingsStore'da.
    /// Oyunsuz test edilir (tests/CheatMenu.Tests). İzinli listede olmayan değerler varsayılana döner.
    /// </summary>
    public sealed class ModSettings
    {
        public static readonly int[] AllowedSpeeds = { 4, 8, 16 };
        public static readonly int[] AllowedXpMultipliers = { 1, 2, 3, 5, 10 };
        public static readonly int[] AllowedPartySizeBonuses = { 0, 50, 100, 200, 500 };
        public const int DefaultSpeed = 4;

        public int FastForwardSpeed { get; private set; } = DefaultSpeed;
        public int SkillXpMultiplier { get; private set; } = 1;
        public int TroopXpMultiplier { get; private set; } = 1;
        public int PartySizeBonus { get; private set; }
        public bool NoLearningLimit { get; set; }

        public void SetFastForwardSpeed(int speed) => FastForwardSpeed = Allowed(AllowedSpeeds, speed, DefaultSpeed);
        public void SetSkillXpMultiplier(int value) => SkillXpMultiplier = Allowed(AllowedXpMultipliers, value, 1);
        public void SetTroopXpMultiplier(int value) => TroopXpMultiplier = Allowed(AllowedXpMultipliers, value, 1);
        public void SetPartySizeBonus(int value) => PartySizeBonus = Allowed(AllowedPartySizeBonuses, value, 0);

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
                bool isNumber = int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int number);
                switch (key)
                {
                    case "FastForwardSpeed": if (isNumber) settings.SetFastForwardSpeed(number); break;
                    case "SkillXpMultiplier": if (isNumber) settings.SetSkillXpMultiplier(number); break;
                    case "TroopXpMultiplier": if (isNumber) settings.SetTroopXpMultiplier(number); break;
                    case "PartySizeBonus": if (isNumber) settings.SetPartySizeBonus(number); break;
                    case "NoLearningLimit":
                        settings.NoLearningLimit = value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                }
            }
            return settings;
        }

        public string Format() =>
            "# CheatMenu ayarlari\n"
            + "FastForwardSpeed=" + Invariant(FastForwardSpeed) + "\n"
            + "SkillXpMultiplier=" + Invariant(SkillXpMultiplier) + "\n"
            + "TroopXpMultiplier=" + Invariant(TroopXpMultiplier) + "\n"
            + "PartySizeBonus=" + Invariant(PartySizeBonus) + "\n"
            + "NoLearningLimit=" + (NoLearningLimit ? "1" : "0") + "\n";

        private static int Allowed(int[] allowed, int value, int fallback) =>
            Array.IndexOf(allowed, value) >= 0 ? value : fallback;

        private static string Invariant(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
```

- [ ] **Step 4: Yeşil** — 44 PASS.

---

### Task 2: Rules + zincirlenen modeller

- [ ] **Step 1:** `src/Cheats/Rules.cs`:

```csharp
using System;
using CheatMenu.Menu;

namespace CheatMenu.Cheats
{
    /// <summary>Kural ayarlarını değiştirir ve kaydeder. Modeller ayarları canlı okuduğu için değişiklik hemen etkilidir.</summary>
    public static class Rules
    {
        public static ModSettings Settings => SettingsStore.Current;

        /// <param name="successMessage">null ise başarı mesajı gösterilmez (aç/kapa satırı zaten gösterir).</param>
        public static void Change(Action<ModSettings> change, string successMessage)
        {
            change(SettingsStore.Current);
            if (!SettingsStore.Save()) MenuUI.Error("Ayar değişti ama ayar dosyasına yazılamadı");
            else if (successMessage != null) MenuUI.Success(successMessage);
        }
    }
}
```

- [ ] **Step 2:** `src/Cheats/Models/CheatGenericXpModel.cs`:

```csharp
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace CheatMenu.Cheats.Models
{
    /// <summary>Yetenek XP çarpanı (oyuncu klanı). HeroDeveloper.AddSkillXp her XP kazancında bunu çağırır.</summary>
    public sealed class CheatGenericXpModel : GenericXpModel
    {
        public override float GetXpMultiplier(Hero hero)
        {
            float value = BaseModel.GetXpMultiplier(hero);
            int multiplier = SettingsStore.Current.SkillXpMultiplier;
            return multiplier > 1 && hero != null && hero.Clan == Clan.PlayerClan ? value * multiplier : value;
        }
    }
}
```

- [ ] **Step 3:** `src/Cheats/Models/CheatCombatXpModel.cs`:

```csharp
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// Asker XP çarpanı: oyuncu partisindeki kahraman olmayan askerlerin vuruşlardan kazandığı XP.
    /// Kahramanların XP'si zaten GenericXpModel'den geçtiği için burada çarpılmaz (çift sayılmasın).
    /// </summary>
    public sealed class CheatCombatXpModel : CombatXpModel
    {
        private static readonly TextObject BonusText = new TextObject("CheatMenu: asker XP çarpanı", null);

        public override float CaptainRadius => BaseModel.CaptainRadius;

        public override SkillObject GetSkillForWeapon(WeaponComponentData weapon, bool isSiegeEngineHit) =>
            BaseModel.GetSkillForWeapon(weapon, isSiegeEngineHit);

        public override ExplainedNumber GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain,
            CharacterObject attackedTroop, PartyBase attackerParty, int damage, bool isFatal, MissionTypeEnum missionType)
        {
            ExplainedNumber result = BaseModel.GetXpFromHit(attackerTroop, captain, attackedTroop, attackerParty, damage, isFatal, missionType);
            int multiplier = SettingsStore.Current.TroopXpMultiplier;
            if (multiplier > 1 && attackerParty == PartyBase.MainParty && attackerTroop != null && !attackerTroop.IsHero)
                result.AddFactor(multiplier - 1, BonusText);
            return result;
        }

        public override float GetXpMultiplierFromShotDifficulty(float shotDifficulty) =>
            BaseModel.GetXpMultiplierFromShotDifficulty(shotDifficulty);
    }
}
```

- [ ] **Step 4:** `src/Cheats/Models/CheatPartySizeLimitModel.cs`:

```csharp
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>Parti boyutu bonusu: yalnızca oyuncu partisinin asker ve esir sınırı. Diğer üyeler delege edilir.</summary>
    public sealed class CheatPartySizeLimitModel : PartySizeLimitModel
    {
        private static readonly TextObject BonusText = new TextObject("CheatMenu: parti boyutu bonusu", null);

        public override int MinimumNumberOfVillagersAtVillagerParty => BaseModel.MinimumNumberOfVillagersAtVillagerParty;

        public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false) =>
            WithBonus(party, BaseModel.GetPartyMemberSizeLimit(party, includeDescriptions));

        public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false) =>
            WithBonus(party, BaseModel.GetPartyPrisonerSizeLimit(party, includeDescriptions));

        public override ExplainedNumber CalculateGarrisonPartySizeLimit(Settlement settlement, bool includeDescriptions = false) =>
            BaseModel.CalculateGarrisonPartySizeLimit(settlement, includeDescriptions);

        public override int GetClanTierPartySizeEffectForHero(Hero hero) => BaseModel.GetClanTierPartySizeEffectForHero(hero);

        public override int GetNextClanTierPartySizeEffectChangeForHero(Hero hero) =>
            BaseModel.GetNextClanTierPartySizeEffectChangeForHero(hero);

        public override int GetAssumedPartySizeForLordParty(Hero leaderHero, IFaction partyMapFaction, Clan actualClan) =>
            BaseModel.GetAssumedPartySizeForLordParty(leaderHero, partyMapFaction, actualClan);

        public override int GetIdealVillagerPartySize(Village village) => BaseModel.GetIdealVillagerPartySize(village);

        public override TroopRoster FindAppropriateInitialRosterForMobileParty(MobileParty party, PartyTemplateObject partyTemplate) =>
            BaseModel.FindAppropriateInitialRosterForMobileParty(party, partyTemplate);

        public override List<Ship> FindAppropriateInitialShipsForMobileParty(MobileParty party, PartyTemplateObject partyTemplate) =>
            BaseModel.FindAppropriateInitialShipsForMobileParty(party, partyTemplate);

        private static ExplainedNumber WithBonus(PartyBase party, ExplainedNumber result)
        {
            int bonus = SettingsStore.Current.PartySizeBonus;
            if (bonus > 0 && party == PartyBase.MainParty) result.Add(bonus, BonusText, null);
            return result;
        }
    }
}
```

- [ ] **Step 5:** `src/Cheats/Models/CheatCharacterDevelopmentModel.cs`:

```csharp
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// "Öğrenme sınırını kaldır": oyuncu klanının kahramanlarında öğrenme sınırına +1000.
    /// DefaultCharacterDevelopmentModel.CalculateLearningRate sınırı zincirin tepesinden (Campaign.Current.Models)
    /// sorduğu için "sınır aşıldı" cezası da kendiliğinden kalkar. Diğer 19 üye olduğu gibi delege edilir.
    /// </summary>
    public sealed class CheatCharacterDevelopmentModel : CharacterDevelopmentModel
    {
        private const int NoLimitBonus = 1000;
        private static readonly TextObject NoLimitText = new TextObject("CheatMenu: öğrenme sınırı kaldırıldı", null);

        public override int MaxAttribute => BaseModel.MaxAttribute;
        public override int MaxFocusPerSkill => BaseModel.MaxFocusPerSkill;
        public override int MaxSkillRequiredForEpicPerkBonus => BaseModel.MaxSkillRequiredForEpicPerkBonus;
        public override int MinSkillRequiredForEpicPerkBonus => BaseModel.MinSkillRequiredForEpicPerkBonus;
        public override int FocusPointsPerLevel => BaseModel.FocusPointsPerLevel;
        public override int FocusPointsAtStart => BaseModel.FocusPointsAtStart;
        public override int AttributePointsAtStart => BaseModel.AttributePointsAtStart;
        public override int LevelsPerAttributePoint => BaseModel.LevelsPerAttributePoint;

        public override int SkillsRequiredForLevel(int level) => BaseModel.SkillsRequiredForLevel(level);
        public override int GetMaxSkillPoint() => BaseModel.GetMaxSkillPoint();
        public override int GetXpRequiredForSkillLevel(int skillLevel) => BaseModel.GetXpRequiredForSkillLevel(skillLevel);

        public override int GetSkillLevelChange(Hero hero, SkillObject skill, float skillXp) =>
            BaseModel.GetSkillLevelChange(hero, skill, skillXp);

        public override int GetXpAmountForSkillLevelChange(Hero hero, SkillObject skill, int skillLevelChange) =>
            BaseModel.GetXpAmountForSkillLevelChange(hero, skill, skillLevelChange);

        public override void GetTraitLevelForTraitXp(Hero hero, TraitObject trait, int newValue, out int traitLevel, out int traitXp) =>
            BaseModel.GetTraitLevelForTraitXp(hero, trait, newValue, out traitLevel, out traitXp);

        public override int GetTraitXpRequiredForTraitLevel(TraitObject trait, int traitLevel) =>
            BaseModel.GetTraitXpRequiredForTraitLevel(trait, traitLevel);

        public override ExplainedNumber CalculateLearningLimit(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes,
            int focusValue, SkillObject skill, bool includeDescriptions = false)
        {
            ExplainedNumber result = BaseModel.CalculateLearningLimit(characterAttributes, focusValue, skill, includeDescriptions);
            if (SettingsStore.Current.NoLearningLimit && BelongsToPlayerClan(characterAttributes))
                result.Add(NoLimitBonus, NoLimitText, null);
            return result;
        }

        public override ExplainedNumber CalculateLearningRate(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes,
            int focusValue, int skillValue, SkillObject skill, bool includeDescriptions = false) =>
            BaseModel.CalculateLearningRate(characterAttributes, focusValue, skillValue, skill, includeDescriptions);

        public override SkillObject GetNextSkillToAddFocus(Hero hero) => BaseModel.GetNextSkillToAddFocus(hero);
        public override CharacterAttribute GetNextAttributeToUpgrade(Hero hero) => BaseModel.GetNextAttributeToUpgrade(hero);
        public override PerkObject GetNextPerkToChoose(Hero hero, PerkObject perk) => BaseModel.GetNextPerkToChoose(hero, perk);

        // Öznitelik nesnesi kahramana özel; hangi kahramana ait olduğunu referansla buluyoruz (oyuncu klanı küçük bir liste).
        private static bool BelongsToPlayerClan(IReadOnlyPropertyOwner<CharacterAttribute> attributes)
        {
            Clan clan = Clan.PlayerClan;
            if (clan == null || attributes == null) return false;
            foreach (Hero hero in clan.Heroes)
                if (ReferenceEquals(hero.CharacterAttributes, attributes)) return true;
            return false;
        }
    }
}
```

- [ ] **Step 6:** `src/Cheats/Models/RuleModels.cs`:

```csharp
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// Modun zincirlediği tüm oyun modelleri. AddModel&lt;T&gt; önceki modeli (vanilla / NavalDLC) BaseModel olarak verir;
    /// bu modellerde abstract olmayan virtual üye yok, bu yüzden tüm abstract üyeleri delege etmek tam zincirleme için yeterli.
    /// </summary>
    public static class RuleModels
    {
        public static void Register(CampaignGameStarter starter)
        {
            starter.AddModel<PartySpeedModel>(new CheatPartySpeedModel());
            starter.AddModel<GenericXpModel>(new CheatGenericXpModel());
            starter.AddModel<CombatXpModel>(new CheatCombatXpModel());
            starter.AddModel<PartySizeLimitModel>(new CheatPartySizeLimitModel());
            starter.AddModel<CharacterDevelopmentModel>(new CheatCharacterDevelopmentModel());
        }
    }
}
```

- [ ] **Step 7: SubModule** — `using CheatMenu.Cheats.Models;`; `OnGameStart` içinde
  `starter.AddModel<PartySpeedModel>(new CheatPartySpeedModel());` → `RuleModels.Register(starter);`
  (kullanılmayan `TaleWorlds.CampaignSystem.ComponentInterfaces` using'i silinir).

- [ ] **Step 8: Derle** — hata yok.

---

### Task 3: Müttefik kapsamı + menüler

- [ ] **Step 1: CheatState** — `public static bool ArmyIncludesAllies;` + `ResetAll` içinde `ArmyIncludesAllies = false;`.

- [ ] **Step 2: CheatTargets** — son satır ve yorum:

```csharp
            // Oyuncu yalnızca [Ben]'e, atlar binicilerine, diğerleri oyuncunun takımındaysa (ve seçiliyse
            // müttefik takımlardaysa) [Ordu]'ya tabi.
            if (agent.IsMainAgent) return me;
            if (agent.IsMount) return agent.RiderAgent != null && IsInScope(agent.RiderAgent, me, army);
            return army && agent.Team != null
                && (agent.Team.IsPlayerTeam || (CheatState.ArmyIncludesAllies && agent.Team.IsPlayerAlly));
```

- [ ] **Step 3: Categories.cs** — kökte `Bilgi`'nin altına `MenuEntry.Submenu("Kurallar", ShowRules),`; `ShowCombat` listesinin sonuna:

```csharp
                MenuEntry.Toggle("(Ordu) hilelerine müttefikleri de kat", () => CheatState.ArmyIncludesAllies,
                    v => CheatState.ArmyIncludesAllies = v),
```

- [ ] **Step 4:** `src/Menu/Categories.Rules.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;

namespace CheatMenu.Menu
{
    /// <summary>Kurallar: kalıcı oran ayarları (config dosyasında, save'de değil).</summary>
    public static partial class Categories
    {
        private static void ShowRules()
        {
            ShowCategory("Kurallar", new List<MenuEntry>
            {
                Choice("Yetenek XP çarpanı (senin klanın)", ModSettings.AllowedXpMultipliers, () => Rules.Settings.SkillXpMultiplier,
                    v => Rules.Change(s => s.SetSkillXpMultiplier(v), "Yetenek XP çarpanı: " + v + "x"), v => v + "x"),
                Choice("Asker XP çarpanı (senin partin)", ModSettings.AllowedXpMultipliers, () => Rules.Settings.TroopXpMultiplier,
                    v => Rules.Change(s => s.SetTroopXpMultiplier(v), "Asker XP çarpanı: " + v + "x"), v => v + "x"),
                Choice("Parti boyutu bonusu", ModSettings.AllowedPartySizeBonuses, () => Rules.Settings.PartySizeBonus,
                    v => Rules.Change(s => s.SetPartySizeBonus(v), "Parti boyutu bonusu: +" + v), v => "+" + v),
                MenuEntry.Toggle("Öğrenme sınırını kaldır (senin klanın)", () => Rules.Settings.NoLearningLimit,
                    v => Rules.Change(s => s.NoLearningLimit = v, null)),
            }, ShowRules, "Ayarlar kalıcıdır ve anında etkilidir; kayıt dosyasına bir şey yazılmaz.");
        }

        private static MenuEntry Choice(string name, int[] options, Func<int> current, Action<int> set, Func<int, string> format) =>
            MenuEntry.Submenu(name + ": " + format(current()), () => ShowChoice(name, options, current, set, format));

        private static void ShowChoice(string name, int[] options, Func<int> current, Action<int> set, Func<int, string> format)
        {
            List<MenuEntry> entries = options
                .Select(option => MenuEntry.Simple(format(option) + (option == current() ? "  [SEÇİLİ]" : ""), () => set(option)))
                .ToList();
            MenuUI.ShowList(name, entries, () => ShowChoice(name, options, current, set, format), "Geri", ShowRules);
        }
    }
}
```

- [ ] **Step 5: Derle + doğrula** — build.ps1 hata yok; `verify_mod.ps1` tipler yüklenir (modeller dahil), 0 konsol hilesi çağrısı.

---

### Task 4: Oyun içi kontrol listesi (kullanıcı, sonra)

- [ ] Kurallar → Yetenek XP 5x → bir savaş → yetenek çubukları belirgin hızlı dolar.
- [ ] Asker XP 5x → savaş sonrası terfi edilebilir asker sayısı belirgin artar.
- [ ] Parti boyutu +100 → parti ekranında sınır +100 ("CheatMenu: parti boyutu bonusu" ipucunda).
- [ ] Öğrenme sınırını kaldır → karakter ekranında öğrenme sınırı ~1000+, sınırın üstündeki yetenekte öğrenme oranı düşmez.
- [ ] Oyunu kapat-aç → ayarlar duruyor.
- [ ] Savaş içi → (Ordu) + müttefikler → bir müttefik ordusuyla ortak savaşta müttefik askerler de ölümsüz.
- [ ] War Sails: deniz savaşı ve parti boyutu normal çalışıyor (NavalDLC modelleriyle zincir).

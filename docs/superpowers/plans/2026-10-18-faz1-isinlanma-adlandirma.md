# Faz 1 — Işınlanma ve Adlandırma Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** CheatMenu'ya "Harita" (yerleşime / lorda / Ctrl+Alt+tık ile imlece ışınlanma) ve "Adlandır" (kahraman, eşraf, klan, krallık, yerleşim, parti) kategorilerini eklemek.

**Architecture:** Mevcut popup altyapısı üstüne arama kutulu genel bir `Picker` eklenir; ışınlanma oyunun hile-modu Ctrl+tık kodunun adımlarını, adlandırma oyunun kendi klan/krallık ekranlarının metin şablonlarını taklit eder. Harmony yok, özel kayıt tipi yok.

**Tech Stack:** C# 9, .NET Framework 4.8.1, VS 2026 MSBuild (SDK-style olmayan csproj), Bannerlord v1.4.8 + War Sails v1.2.8.

**Spec:** `docs/superpowers/specs/2026-10-18-faz1-isinlanma-adlandirma-design.md`

**Git yok:** commit adımı yok; her görev "derleme yeşil" ile biter. Oyun açıkken derleme DeployToGame'de kilit hatası verir.

---

## Dosya yapısı

| Dosya | Durum | Sorumluluk |
|---|---|---|
| `src/NameRules.cs` | yeni | ad doğrulama (saf, test edilir) |
| `tests/CheatMenu.Tests/Program.cs` | değişir | NameRules testleri |
| `tests/CheatMenu.Tests/CheatMenu.Tests.csproj` | değişir | NameRules.cs bağlantısı |
| `src/CheatMenu.csproj` | değişir | SandBox, SandBox.View, ScreenSystem referansları |
| `src/Menu/MenuUI.cs` | değişir | `AskName`, `ShowList` açıklama parametresi |
| `src/Menu/MenuEntry.cs` | değişir | `Text` satır türü |
| `src/Menu/Picker.cs` | yeni | arama kutulu genel seçici |
| `src/Menu/Lists.cs` | yeni | seçici kaynakları + etiketler |
| `src/Cheats/Teleport.cs` | yeni | engel kontrolü + ışınlama |
| `src/Cheats/CursorTeleport.cs` | yeni | Ctrl+Alt+tık + imleç konumu |
| `src/Cheats/Renamer.cs` | yeni | 5 tür adlandırma |
| `src/Menu/Categories.cs` | değişir | Harita + Adlandır kategorileri |
| `src/SubModule.cs` | değişir | `CursorTeleport.Tick()` |

---

### Task 1: NameRules (TDD)

**Files:** Create `src/NameRules.cs`; Modify `tests/CheatMenu.Tests/Program.cs`, `tests/CheatMenu.Tests/CheatMenu.Tests.csproj`

- [ ] **Step 1: Test projesine NameRules.cs bağlantısı ekle** — `CheatMenu.Tests.csproj` içinde AmountParser satırının altına:

```xml
    <Compile Include="..\..\src\NameRules.cs"><Link>NameRules.cs</Link></Compile>
```

- [ ] **Step 2: Başarısız testleri yaz** — `Program.cs` `Main` içinde `Console.WriteLine(_failures == 0 ...` satırından önce:

```csharp
            // NameRules: kırp, boş yalnızca allowEmpty ile, en fazla 50 karakter, { } yasak.
            ExpectName("Tolga", false, true, "Tolga");
            ExpectName("  Yeni Ad  ", false, true, "Yeni Ad");
            ExpectName("", false, false, "");
            ExpectName("   ", false, false, "");
            ExpectName("", true, true, "");
            ExpectName(null, true, true, "");
            ExpectName(new string('a', 50), false, true, new string('a', 50));
            ExpectName(new string('a', 51), false, false, "");
            ExpectName("Ad{X}", false, false, "");
            ExpectName("Cagri Sukru", false, true, "Cagri Sukru");
```

ve sınıfa yardımcı:

```csharp
        private static void ExpectName(string input, bool allowEmpty, bool expectedOk, string expectedName)
        {
            bool ok = NameRules.TryNormalize(input, allowEmpty, out string name, out string error);
            bool pass = ok == expectedOk && (!expectedOk || name == expectedName);
            if (!pass) _failures++;
            Console.WriteLine((pass ? "PASS " : "FAIL ") + "name \"" + input + "\" (allowEmpty=" + allowEmpty + ") -> ok=" + ok
                + ", name=\"" + name + "\"" + (error != null ? " [error]" : ""));
        }
```

- [ ] **Step 3: Kırmızıyı gör** — Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1` → Expected: `CS2001: Source file '...\src\NameRules.cs' could not be found`.

- [ ] **Step 4: Uygula** — `src/NameRules.cs`:

```csharp
namespace CheatMenu
{
    /// <summary>
    /// Adlandırma kutusuna yazılan adı temizler ve doğrular. Oyun API'sine bağımlı değildir;
    /// tests/CheatMenu.Tests altında oyunsuz test edilir.
    /// </summary>
    public static class NameRules
    {
        public const int MaxLength = 50;

        private static readonly char[] Forbidden = { '{', '}' };

        /// <param name="allowEmpty">Yerleşim ve partilerde boş ad "orijinal ada dön" demektir.</param>
        public static bool TryNormalize(string input, bool allowEmpty, out string name, out string error)
        {
            name = (input ?? string.Empty).Trim();
            error = null;

            if (name.Length == 0)
            {
                if (allowEmpty) return true;
                error = "Ad boş olamaz";
                return false;
            }
            if (name.Length > MaxLength)
            {
                error = "Ad en fazla " + MaxLength + " karakter olabilir";
                return false;
            }
            // TextObject süslü parantezleri değişken sanar; ad bozulur.
            if (name.IndexOfAny(Forbidden) >= 0)
            {
                error = "Ad { veya } içeremez";
                return false;
            }
            return true;
        }
    }
}
```

- [ ] **Step 5: Yeşili gör** — aynı komut → Expected: 18 `PASS`, `ALL TESTS PASSED`, `CheatMenu -> ...`.

---

### Task 2: Menü altyapısı (AskName, açıklama, Text satırı, Picker, Lists, referanslar)

**Files:** Modify `src/CheatMenu.csproj`, `src/Menu/MenuUI.cs`, `src/Menu/MenuEntry.cs`; Create `src/Menu/Picker.cs`, `src/Menu/Lists.cs`

- [ ] **Step 1: csproj referansları** — `<ModuleOut>` satırının altına `<SandBoxBin>$(GameFolder)\Modules\SandBox\bin\Win64_Shipping_Client</SandBoxBin>`; referans grubuna:

```xml
    <Reference Include="SandBox"><HintPath>$(SandBoxBin)\SandBox.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="SandBox.View"><HintPath>$(SandBoxBin)\SandBox.View.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.ScreenSystem"><HintPath>$(GameBin)\TaleWorlds.ScreenSystem.dll</HintPath><Private>False</Private></Reference>
```

- [ ] **Step 2: MenuUI** — `ShowList` imzasına `string description = ""` eklenir ve `string.Empty` yerine `description` geçilir; `AskNumber`'ın altına:

```csharp
        /// <summary>Ad sorar (kutu mevcut adla dolu). Geçerliyse apply çalışır; her durumda sonunda onDone çağrılır.</summary>
        public static void AskName(string title, string currentName, bool allowEmpty, Action<string> apply, Action onDone)
        {
            string hint = allowEmpty ? "Yeni adı yaz (boş bırakırsan orijinal ada döner):" : "Yeni adı yaz:";
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    title, hint, true, true, "Uygula", "İptal",
                    text =>
                    {
                        if (NameRules.TryNormalize(text, allowEmpty, out string name, out string error)) SafeRun(title, () => apply(name));
                        else Error(error);
                        Defer(onDone);
                    },
                    () => Defer(onDone),
                    false,
                    text => NameRules.TryNormalize(text, allowEmpty, out _, out string error)
                        ? new Tuple<bool, string>(true, string.Empty)
                        : new Tuple<bool, string>(false, error),
                    "",
                    currentName ?? string.Empty),
                true, false);
        }
```

- [ ] **Step 3: MenuEntry.Text** — `Custom`'ın altına:

```csharp
        /// <summary>Ad soran eylem; kutu mevcut adla dolu açılır, onaylanınca apply çalışır.</summary>
        public static MenuEntry Text(string label, Func<string> currentName, bool allowEmpty, Action<string> apply) =>
            new MenuEntry(label, reopen => MenuUI.AskName(label, currentName(), allowEmpty, apply, reopen));
```

- [ ] **Step 4: Picker** — `src/Menu/Picker.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace CheatMenu.Menu
{
    /// <summary>Arama kutulu, tek seçimli genel liste. Faz 2 editörleri de bunu kullanır.</summary>
    public static class Picker
    {
        public static void Pick<T>(string title, IEnumerable<T> items, Func<T, string> label, Action<T> onPicked, Action onBack)
            where T : class
        {
            List<InquiryElement> elements = items
                .Select(item => new InquiryElement(item, label(item), null))
                .OrderBy(element => element.Title, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            if (elements.Count == 0)
            {
                MenuUI.Error(title + ": liste boş");
                MenuUI.Defer(onBack);
                return;
            }

            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(
                    title, elements.Count + " kayıt - aramak için yazmaya başla", elements,
                    true, 1, 1, "Seç", "Geri",
                    selected =>
                    {
                        // Bir sonraki karede işlensin: onPicked yeni popup (ad kutusu) açabilir.
                        if (selected != null && selected.Count > 0 && selected[0].Identifier is T item)
                            MenuUI.Defer(() => onPicked(item));
                    },
                    _ => MenuUI.Defer(onBack),
                    "", true),
                true, false);
        }
    }
}
```

- [ ] **Step 5: Lists** — `src/Menu/Lists.cs`:

```csharp
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Menu
{
    /// <summary>Seçici listelerinin kaynakları ve satır etiketleri.</summary>
    public static class Lists
    {
        public static IEnumerable<Settlement> Settlements() =>
            Settlement.All.Where(s => s.IsTown || s.IsCastle || s.IsVillage);

        public static IEnumerable<Hero> LordsAndCompanions(bool includeChildren) =>
            Hero.AllAliveHeroes.Where(h => h != Hero.MainHero
                && (h.IsLord || h.IsWanderer || h.IsPlayerCompanion)
                && (includeChildren || !h.IsChild));

        public static IEnumerable<Hero> Notables() => Hero.AllAliveHeroes.Where(h => h.IsNotable);

        public static IEnumerable<Clan> Clans() => Clan.All.Where(c => !c.IsEliminated && !c.IsBanditFaction);

        public static IEnumerable<Kingdom> Kingdoms() => Kingdom.All.Where(k => !k.IsEliminated);

        public static IEnumerable<MobileParty> Parties() =>
            MobileParty.All.Where(p => p.IsActive && (p.IsMainParty || p.IsLordParty || p.IsCaravan));

        public static string SettlementLabel(Settlement s) =>
            s.Name + " (" + Kind(s) + (s.MapFaction != null ? ", " + s.MapFaction.Name : "") + ")";

        public static string HeroLabel(Hero h) =>
            h.Name + (h.Clan != null ? " (" + h.Clan.Name + ")" : "") + " - " + Where(h);

        public static string ClanLabel(Clan c) =>
            c.Name + " (" + (c.Kingdom != null ? c.Kingdom.Name.ToString() : "bağımsız") + ")";

        public static string PartyLabel(MobileParty p) =>
            p.Name + (p.LeaderHero != null && !p.IsMainParty ? " (" + p.LeaderHero.Name + ")" : "");

        private static string Kind(Settlement s) => s.IsTown ? "Şehir" : s.IsCastle ? "Kale" : "Köy";

        private static string Where(Hero h)
        {
            if (h.IsPrisoner) return "esir";
            MobileParty party = h.PartyBelongedTo;
            if (party?.CurrentSettlement != null) return party.CurrentSettlement.Name.ToString();
            if (party != null) return party.IsCurrentlyAtSea ? "denizde" : "seferde";
            if (h.CurrentSettlement != null) return h.CurrentSettlement.Name.ToString();
            return "bilinmiyor";
        }
    }
}
```

- [ ] **Step 6: Derle** — build.ps1 → Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

---

### Task 3: Işınlanma

**Files:** Create `src/Cheats/Teleport.cs`, `src/Cheats/CursorTeleport.cs`

- [ ] **Step 1: Teleport** — `src/Cheats/Teleport.cs`:

```csharp
using System;
using CheatMenu.Menu;
using Helpers;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Ana partiyi ışınlar. Adımlar oyunun hile modundaki Ctrl+tık ışınlanmasını
    /// (SandBox.View.Map.MapScreen.HandleLeftMouseButtonClick) izler; yalnızca oyunun kendi kaydettiği alanlar değişir.
    /// </summary>
    public static class Teleport
    {
        /// <summary>Işınlanmaya engel bir durum varsa nedenini, yoksa null döner.</summary>
        public static string GetBlocker()
        {
            MobileParty me = MobileParty.MainParty;
            if (me == null || Hero.MainHero == null) return "Ana parti bulunamadı";
            if (Mission.Current != null) return "Sahne veya savaş içindeyken ışınlanamazsın";
            if (me.MapEvent != null) return "Savaş sırasında ışınlanamazsın";
            if (me.SiegeEvent != null || me.BesiegedSettlement != null) return "Kuşatma sırasında ışınlanamazsın";
            if (me.IsInRaftState) return "Sal durumundayken ışınlanamazsın";
            if (me.Army != null && me.Army.LeaderParty != me) return "Başkasının ordusundayken ışınlanamazsın";
            if (Hero.MainHero.IsPrisoner) return "Esirken ışınlanamazsın";
            return null;
        }

        public static void ToSettlement(Settlement settlement)
        {
            MoveMainParty(settlement.GatePosition);
            MenuUI.Success(settlement.Name + " kapısına ışınlandın");
        }

        public static void ToHero(Hero hero)
        {
            if (hero.IsPrisoner) throw new InvalidOperationException(hero.Name + " esir, yanına ışınlanamazsın");

            MobileParty party = hero.PartyBelongedTo;
            if (party != null && party != MobileParty.MainParty && party.IsActive && party.CurrentSettlement == null)
            {
                if (party.IsCurrentlyAtSea && !MobileParty.MainParty.HasNavalNavigationCapability)
                    throw new InvalidOperationException(hero.Name + " denizde ve senin gemin yok");

                // Tam üstüne değil, 1-2 birim yanına: hemen karşılaşma başlamasın.
                MobileParty.NavigationType navigation = party.IsCurrentlyAtSea
                    ? MobileParty.NavigationType.Naval
                    : MobileParty.NavigationType.Default;
                CampaignVec2 near = NavigationHelper.FindPointAroundPosition(party.Position, navigation, 2f, 1f, false, false);
                MoveMainParty(near.IsValid() ? near : party.Position);
                MenuUI.Success(hero.Name + " yanına ışınlandın");
                return;
            }

            Settlement settlement = party?.CurrentSettlement ?? hero.CurrentSettlement;
            if (settlement == null) throw new InvalidOperationException(hero.Name + " şu an haritada değil");
            ToSettlement(settlement);
        }

        public static void ToPosition(CampaignVec2 target)
        {
            if (!target.IsOnLand && !MobileParty.MainParty.HasNavalNavigationCapability)
                throw new InvalidOperationException("Gemin olmadan denize ışınlanamazsın");
            MoveMainParty(target);
        }

        private static void MoveMainParty(CampaignVec2 target)
        {
            string blocker = GetBlocker();
            if (blocker != null) throw new InvalidOperationException(blocker);
            if (!target.IsValid()) throw new InvalidOperationException("Geçersiz hedef konum");

            MobileParty me = MobileParty.MainParty;

            // Oyunun "yerleşimden ayrıl" seçeneğinin yaptığı gibi önce karşılaşmayı kapat.
            if (PlayerEncounter.Current != null)
            {
                if (me.CurrentSettlement != null) PlayerEncounter.LeaveSettlement();
                PlayerEncounter.Finish(true);
            }
            if (me.CurrentSettlement != null) LeaveSettlementAction.ApplyForParty(me);

            me.Position = target;
            me.SetMoveModeHold();

            // Hedef partinin şu anki kara/deniz durumuna uymuyorsa durumu çevir (oyunun kendi kontrolü).
            MobileParty.NavigationType current = me.IsCurrentlyAtSea
                ? MobileParty.NavigationType.Naval
                : MobileParty.NavigationType.Default;
            if (!NavigationHelper.IsPositionValidForNavigationType(target, current)) me.ChangeIsCurrentlyAtSeaCheat();

            if (me.Army != null && me.Army.LeaderParty == me)
                foreach (MobileParty attached in me.AttachedParties)
                    attached.Position = target;

            foreach (MobileParty party in MobileParty.All) party.Party.UpdateVisibilityAndInspected(me.Position);
            foreach (Settlement settlement in Settlement.All) settlement.Party.UpdateVisibilityAndInspected(me.Position);

            MapScreen.Instance?.TeleportCameraToMainParty();
        }
    }
}
```

- [ ] **Step 2: CursorTeleport** — `src/Cheats/CursorTeleport.cs`:

```csharp
using System;
using CheatMenu.Menu;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace CheatMenu.Cheats
{
    /// <summary>Ctrl+Alt + Sol tık: kampanya haritasında imlecin altına ışınlanma.</summary>
    public static class CursorTeleport
    {
        public static void Tick()
        {
            if (!Input.IsKeyPressed(InputKey.LeftMouseButton)) return;
            if (!IsEitherDown(InputKey.LeftControl, InputKey.RightControl)) return;
            if (!IsEitherDown(InputKey.LeftAlt, InputKey.RightAlt)) return;
            if (!(ScreenManager.TopScreen is MapScreen mapScreen)) return;
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState mapState) || mapState.AtMenu) return;
            if (InformationManager.IsAnyInquiryActive()) return;

            MenuUI.SafeRun("Fareyle ışınlanma", () =>
            {
                if (!TryGetCursorPosition(mapScreen, out CampaignVec2 target))
                    throw new InvalidOperationException("İmlecin altı ışınlanılabilir bir yer değil");
                Teleport.ToPosition(target);
                MenuUI.Success("Işınlandın");
            });
        }

        private static bool IsEitherDown(InputKey a, InputKey b) => Input.IsKeyDown(a) || Input.IsKeyDown(b);

        /// <summary>Oyunun MapScreen.HandleMouse içindeki imleç → harita noktası hesabı.</summary>
        private static bool TryGetCursorPosition(MapScreen mapScreen, out CampaignVec2 position)
        {
            Vec3 near = Vec3.Zero;
            Vec3 far = Vec3.Zero;
            mapScreen.SceneLayer.SceneView.TranslateMouse(ref near, ref far, -1f);

            PathFaceRecord face = PathFaceRecord.NullFaceRecord;
            mapScreen.GetCursorIntersectionPoint(ref near, ref far, out float _, out Vec3 point, ref face, out bool isOnLand);

            position = new CampaignVec2(point.AsVec2, isOnLand);
            return face.IsValid();
        }
    }
}
```

- [ ] **Step 3: Derle** — build.ps1 → Expected: hata yok.

---

### Task 4: Adlandırma

**Files:** Create `src/Cheats/Renamer.cs`

- [ ] **Step 1: Renamer** — `src/Cheats/Renamer.cs`:

```csharp
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Adlandırma. Kahraman/klan/krallık için oyunun kendi ekranlarının metin şablonları kullanılır;
    /// yerleşim ve parti için kaydedilen "özel ad" alanı (PartyBase.CustomName).
    /// </summary>
    public static class Renamer
    {
        // ClanLordItemVM.OnNamingHeroOver ile aynı şablonlar.
        public static void RenameHero(Hero hero, string name)
        {
            int gender = hero.IsFemale ? 1 : 0;
            TextObject firstName = GameTexts.FindText("str_generic_character_firstname", null)
                .SetTextVariable("CHARACTER_FIRSTNAME", new TextObject(name, null))
                .SetTextVariable("CHARACTER_GENDER", gender);
            TextObject fullName = GameTexts.FindText("str_generic_character_name", null)
                .SetTextVariable("CHARACTER_NAME", new TextObject(name, null))
                .SetTextVariable("CHARACTER_GENDER", gender);
            hero.SetName(fullName, firstName);

            Army army = hero.PartyBelongedTo?.Army;
            if (army != null && army.LeaderParty?.Owner == hero) army.UpdateName();
            MenuUI.Success("Kahramanın yeni adı: " + name);
        }

        // ClanManagementVM.OnChangeClanNameDone ile aynı.
        public static void RenameClan(Clan clan, string name)
        {
            TextObject text = GameTexts.FindText("str_generic_clan_name", null)
                .SetTextVariable("CLAN_NAME", new TextObject(name, null));
            clan.ChangeClanName(text, text);
            MenuUI.Success("Klanın yeni adı: " + name);
        }

        // KingdomManagementVM.OnChangeKingdomNameDone ile aynı.
        public static void RenameKingdom(Kingdom kingdom, string name)
        {
            TextObject input = new TextObject(name, null);
            TextObject fullName = GameTexts.FindText("str_generic_kingdom_name", null).SetTextVariable("KINGDOM_NAME", input);
            TextObject shortName = GameTexts.FindText("str_generic_kingdom_short_name", null).SetTextVariable("KINGDOM_SHORT_NAME", input);
            kingdom.ChangeKingdomName(fullName, shortName);
            MenuUI.Success("Krallığın yeni adı: " + kingdom.Name);
        }

        /// <summary>Boş ad = orijinal ada dön. Yerleşimin asıl adı save'e yazılmadığı için ona dokunulmaz.</summary>
        public static void RenameSettlement(Settlement settlement, string name)
        {
            settlement.Party.SetCustomName(ToCustomName(name));
            MenuUI.Success(name.Length == 0 ? "Orijinal ada dönüldü: " + settlement.Name : "Yerleşimin yeni adı: " + name);
        }

        /// <summary>Boş ad = orijinal ada dön. Oyun bazı olaylarda (dağılma vb.) parti adını kendisi sıfırlayabilir.</summary>
        public static void RenameParty(MobileParty party, string name)
        {
            party.Party.SetCustomName(ToCustomName(name));
            MenuUI.Success(name.Length == 0 ? "Orijinal ada dönüldü: " + party.Name : "Partinin yeni adı: " + name);
        }

        // Sıfırlamada oyunun kendi yöntemi (DisbandPartyAction.CancelDisband) TextObject.GetEmpty().
        private static TextObject ToCustomName(string name) =>
            name.Length == 0 ? TextObject.GetEmpty() : new TextObject(name, null);
    }
}
```

- [ ] **Step 2: Derle** — build.ps1 → Expected: hata yok.

---

### Task 5: Menü kategorileri ve bağlama

**Files:** Modify `src/Menu/Categories.cs`, `src/SubModule.cs`

- [ ] **Step 1: Categories** — usings'e `System`, `System.Collections.Generic` (var), `TaleWorlds.CampaignSystem`, `TaleWorlds.CampaignSystem.Party`, `TaleWorlds.CampaignSystem.Settlements`; `ShowRoot` listesine `MenuEntry.Submenu("Harita", ShowMap)` ve `MenuEntry.Submenu("Adlandır", ShowRename)`; `ShowCategory`'ye `string description = ""` parametresi; yeni metotlar:

```csharp
        private static void ShowMap()
        {
            ShowCategory("Harita", new List<MenuEntry>
            {
                MenuEntry.Submenu("Yerleşime ışınlan...", PickSettlementToTeleport),
                MenuEntry.Submenu("Bir lordun yanına ışınlan...", PickHeroToTeleport),
            }, ShowMap, "İpucu: Ctrl+Alt + Sol tık ile haritada tıkladığın yere ışınlanırsın.");
        }

        private static void PickSettlementToTeleport()
        {
            if (TeleportBlocked()) return;
            Picker.Pick("Yerleşime ışınlan", Lists.Settlements(), Lists.SettlementLabel,
                s => MenuUI.SafeRun("Işınlanma", () => Teleport.ToSettlement(s)), ShowMap);
        }

        private static void PickHeroToTeleport()
        {
            if (TeleportBlocked()) return;
            Picker.Pick("Bir lordun yanına ışınlan", Lists.LordsAndCompanions(false), Lists.HeroLabel,
                h => MenuUI.SafeRun("Işınlanma", () => Teleport.ToHero(h)), ShowMap);
        }

        private static bool TeleportBlocked()
        {
            string blocker = Teleport.GetBlocker();
            if (blocker == null) return false;
            MenuUI.Error(blocker);
            MenuUI.Defer(ShowMap);
            return true;
        }

        private static void ShowRename()
        {
            ShowCategory("Adlandır", new List<MenuEntry>
            {
                MenuEntry.Text("Kendimi adlandır", () => Hero.MainHero.FirstName.ToString(), false,
                    name => Renamer.RenameHero(Hero.MainHero, name)),
                MenuEntry.Text("Partimi adlandır", () => MobileParty.MainParty.Name.ToString(), true,
                    name => Renamer.RenameParty(MobileParty.MainParty, name)),
                MenuEntry.Text("Bulunduğum yerleşimi adlandır", CurrentSettlementName, true,
                    name => Renamer.RenameSettlement(MobileParty.MainParty.CurrentSettlement, name)),
                MenuEntry.Submenu("Bir lord veya yoldaş...", () => PickAndRename("Lord veya yoldaş",
                    Lists.LordsAndCompanions(true), Lists.HeroLabel, h => h.FirstName.ToString(), false, Renamer.RenameHero)),
                MenuEntry.Submenu("Bir eşraf...", () => PickAndRename("Eşraf",
                    Lists.Notables(), Lists.HeroLabel, h => h.FirstName.ToString(), false, Renamer.RenameHero)),
                MenuEntry.Submenu("Bir klan...", () => PickAndRename("Klan",
                    Lists.Clans(), Lists.ClanLabel, c => c.Name.ToString(), false, Renamer.RenameClan)),
                MenuEntry.Submenu("Bir krallık...", () => PickAndRename("Krallık",
                    Lists.Kingdoms(), k => k.Name.ToString(), k => k.InformalName.ToString(), false, Renamer.RenameKingdom)),
                MenuEntry.Submenu("Bir yerleşim...", () => PickAndRename("Yerleşim",
                    Lists.Settlements(), Lists.SettlementLabel, s => s.Name.ToString(), true, Renamer.RenameSettlement)),
                MenuEntry.Submenu("Bir parti...", () => PickAndRename("Parti",
                    Lists.Parties(), Lists.PartyLabel, p => p.Name.ToString(), true, Renamer.RenameParty)),
            }, ShowRename, "Yerleşim ve partilerde adı boş bırakırsan orijinal ada döner. "
                + "Oyun bazı olaylarda parti adını kendisi sıfırlayabilir.");
        }

        private static string CurrentSettlementName()
        {
            Settlement settlement = MobileParty.MainParty?.CurrentSettlement
                ?? throw new InvalidOperationException("Şu an bir yerleşimde değilsin");
            return settlement.Name.ToString();
        }

        private static void PickAndRename<T>(string kind, IEnumerable<T> items, Func<T, string> label,
            Func<T, string> currentName, bool allowEmpty, Action<T, string> rename) where T : class
        {
            string title = kind + " adlandır";
            Picker.Pick(title, items, label,
                item => MenuUI.AskName(title, currentName(item), allowEmpty, name => rename(item, name), ShowRename),
                ShowRename);
        }
```

ve

```csharp
        private static void ShowCategory(string title, List<MenuEntry> entries, Action reshow, string description = "") =>
            MenuUI.ShowList(title, entries, reshow, "Geri", ShowRoot, description);
```

- [ ] **Step 2: SubModule** — `OnApplicationTick` sonuna `CursorTeleport.Tick();`.

- [ ] **Step 3: Derle + tip yükleme kontrolü** — build.ps1 → Expected: hata yok. Ardından DLL'i oyun assembly'leriyle yükleyip `GetTypes()` → tüm tipler `OK`.

---

### Task 6: Oyun içi kontrol listesi (kullanıcı)

- [ ] Harita → Yerleşime ışınlan → arama kutusuna "Prav" yaz → Pravend → kapının önündesin, kamera orada.
- [ ] Şehrin içindeyken (menüde) Insert → Harita → başka bir şehre → şehirden çıkılıp ışınlanılır.
- [ ] Bir lordun yanına ışınlan → partisinin yanındasın; şehirdeki bir lord → o şehrin kapısı.
- [ ] Ctrl+Alt + Sol tık boş araziye → oraya ışınlanılır; gemisizken denize → kırmızı ret mesajı.
- [ ] Ordu liderken ışınlan → ordu da gelir. Başkasının ordusundayken → ret.
- [ ] Adlandır: kendini, partini, bir şehri, bir klanı, bir krallığı, bir eşrafı → yeni adlar ansiklopedide/haritada.
- [ ] Şehir adını boş bırakıp onayla → orijinal ad geri gelir.
- [ ] Kaydet → ana menü → yükle → yeni adlar duruyor.
- [ ] Harita etiketinde şehir adı hemen güncelleniyor mu (güncellenmiyorsa kaydet-yükle sonrası).

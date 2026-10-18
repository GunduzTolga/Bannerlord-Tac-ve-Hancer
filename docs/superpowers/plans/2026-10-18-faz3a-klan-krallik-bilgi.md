# Faz 3A — Klan & Krallık ve Bilgi Araçları Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** CheatMenu'ya "Klan & krallık" (kendi klanın + herkes için krallık araçları) ve "Bilgi" (lord bulucu, harita sisi aç/kapa) kategorilerini eklemek.

**Architecture:** Mevcut popup + `Picker` altyapısı; eylemler `Cheats/KingdomTools.cs` ve `Cheats/MapInfo.cs` içinde oyunun eylem sınıflarını ön koşul kontrolleriyle çağırır; menüler `Categories.Politics.cs` partial dosyasında. Geri alınamaz işlemler `MenuUI.Confirm` ile onaylanır.

**Tech Stack:** C# 9, .NET Framework 4.8.1, VS 2026 MSBuild, Bannerlord v1.4.8 + War Sails v1.2.8.

**Spec:** `docs/superpowers/specs/2026-10-18-faz3a-klan-krallik-bilgi-design.md`

**Git yok:** commit adımı yok. Derleme: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`

---

## Dosya yapısı

| Dosya | Durum | Sorumluluk |
|---|---|---|
| `src/CheatState.cs` | değişir | `RevealMap` bayrağı |
| `src/Menu/MenuUI.cs` | değişir | `Confirm` |
| `src/Cheats/Renamer.cs` | değişir | `KingdomNameTexts` ortak yardımcı |
| `src/Cheats/KingdomTools.cs` | yeni | klan/krallık listeleri ve eylemleri |
| `src/Cheats/MapInfo.cs` | yeni | lord konumu + kamera, TrueSight |
| `src/Menu/Categories.Politics.cs` | yeni | iki menü |
| `src/Menu/Categories.cs` | değişir | köke 2 satır |
| `src/SubModule.cs` | değişir | `MapInfo.Tick()` |

---

### Task 1: Altyapı (CheatState, Confirm, KingdomNameTexts)

- [ ] **Step 1: CheatState** — alanlara `public static bool RevealMap;`, `ResetAll()` içine `RevealMap = false;`.

- [ ] **Step 2: MenuUI.Confirm** — `SafeRun`'ın üstüne:

```csharp
        /// <summary>Evet/Vazgeç onayı (geri alınamaz işlemler için). Seçim bir sonraki karede işlenir.</summary>
        public static void Confirm(string title, string text, Action onYes, Action onNo)
        {
            InformationManager.ShowInquiry(
                new InquiryData(title, text, true, true, "Evet", "Vazgeç",
                    () => Defer(() => SafeRun(title, onYes)),
                    () => Defer(onNo)),
                true, false);
        }
```

- [ ] **Step 3: Renamer** — `RenameKingdom` gövdesindeki iki şablon satırı yardımcıya taşınır:

```csharp
        // KingdomManagementVM.OnChangeKingdomNameDone ile aynı.
        public static void RenameKingdom(Kingdom kingdom, string name)
        {
            KingdomNameTexts(name, out TextObject fullName, out TextObject shortName);
            kingdom.ChangeKingdomName(fullName, shortName);
            MenuUI.Success("Krallığın yeni adı: " + kingdom.Name);
        }

        /// <summary>Oyunun krallık adı şablonları; krallık kurarken de kullanılır.</summary>
        internal static void KingdomNameTexts(string name, out TextObject fullName, out TextObject shortName)
        {
            TextObject input = new TextObject(name, null);
            fullName = GameTexts.FindText("str_generic_kingdom_name", null).SetTextVariable("KINGDOM_NAME", input);
            shortName = GameTexts.FindText("str_generic_kingdom_short_name", null).SetTextVariable("KINGDOM_SHORT_NAME", input);
        }
```

- [ ] **Step 4: Derle** — hata yok, `ALL TESTS PASSED`.

---

### Task 2: KingdomTools ve MapInfo

- [ ] **Step 1:** `src/Cheats/KingdomTools.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Klan ve krallık araçları. Oyunun eylem sınıfları doğrudan çağrılır; konsol hilelerinin ön koşulları
    /// (ör. hükümdarken başka krallığa katılamama) burada da uygulanır.
    /// </summary>
    public static class KingdomTools
    {
        private static Clan Me => Clan.PlayerClan ?? throw new InvalidOperationException("Oyuncu klanı bulunamadı");

        // ---- Listeler ----

        public static IEnumerable<Kingdom> LiveKingdoms() => Kingdom.All.Where(k => !k.IsEliminated);

        /// <summary>Savaş/barış tarafları: yaşayan krallıklar + (bağımsızsa) oyuncunun klanı.</summary>
        public static IEnumerable<IFaction> Factions()
        {
            foreach (Kingdom kingdom in LiveKingdoms()) yield return kingdom;
            if (Me.Kingdom == null) yield return Me;
        }

        public static IEnumerable<IFaction> PeacefulWith(IFaction a) =>
            Factions().Where(b => b != a && !FactionManager.IsAtWarAgainstFaction(a, b));

        public static IEnumerable<IFaction> AtWarWith(IFaction a) =>
            Factions().Where(b => b != a && FactionManager.IsAtWarAgainstFaction(a, b)
                && !FactionManager.IsAtConstantWarAgainstFaction(a, b));

        public static IEnumerable<Clan> MovableClans() =>
            Clan.All.Where(c => !c.IsEliminated && !c.IsBanditFaction && c != Clan.PlayerClan && c.Leader != null
                && (c.Kingdom == null || c.Kingdom.RulingClan != c));

        public static IEnumerable<Hero> LeaderCandidates(Clan clan) =>
            clan.Heroes.Where(h => h.IsAlive && !h.IsChild && h != clan.Leader);

        public static IEnumerable<Clan> RulerCandidates(Kingdom kingdom) =>
            kingdom.Clans.Where(c => c != kingdom.RulingClan && !c.IsEliminated && !c.IsUnderMercenaryService);

        // ---- Benim klanım ----

        public static void JoinAsVassal(Kingdom kingdom)
        {
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Krallık yönetirken başka krallığa katılamazsın");
            if (Me.Kingdom == kingdom && !Me.IsUnderMercenaryService)
                throw new InvalidOperationException("Zaten " + kingdom.Name + " vasalısın");
            ChangeKingdomAction.ApplyByJoinToKingdom(Me, kingdom, default, true);
            MenuUI.Success(kingdom.Name + " krallığına vasal olarak katıldın");
        }

        public static void JoinAsMercenary(Kingdom kingdom)
        {
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Krallık yönetirken paralı asker olamazsın");
            if (Me.Kingdom != null) throw new InvalidOperationException("Paralı asker olmak için önce krallığından ayrıl");
            ChangeKingdomAction.ApplyByJoinFactionAsMercenary(Me, kingdom, default, 50, true);
            MenuUI.Success(kingdom.Name + " için paralı asker oldun");
        }

        public static void LeaveKingdom()
        {
            Kingdom kingdom = Me.Kingdom ?? throw new InvalidOperationException("Bir krallıkta değilsin");
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Yönettiğin krallıktan ayrılamazsın");
            if (Me.IsUnderMercenaryService) ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(Me, true);
            else ChangeKingdomAction.ApplyByLeaveKingdom(Me, true);
            MenuUI.Success(kingdom.Name + " krallığından ayrıldın");
        }

        public static void CreateKingdom(string name)
        {
            if (Me.Kingdom != null) throw new InvalidOperationException("Krallık kurmak için önce krallığından ayrıl");
            Renamer.KingdomNameTexts(name, out TextObject fullName, out TextObject shortName);
            Campaign.Current.KingdomManager.CreateKingdom(fullName, shortName, Me.Culture, Me, null, null, null, null);
            MenuUI.Success("Krallığın kuruldu: " + (Me.Kingdom != null ? Me.Kingdom.Name : fullName));
        }

        public static void BecomeRuler()
        {
            Kingdom kingdom = Me.Kingdom ?? throw new InvalidOperationException("Bir krallıkta değilsin");
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Zaten krallığın hükümdarısın");
            if (Me.IsUnderMercenaryService) throw new InvalidOperationException("Paralı askerken hükümdar olamazsın");
            ChangeRulingClanAction.Apply(kingdom, Me);
            MenuUI.Success(kingdom.Name + " krallığının hükümdarı oldun");
        }

        public static int MinTier => Campaign.Current.Models.ClanTierModel.MinClanTier;
        public static int MaxTier => Campaign.Current.Models.ClanTierModel.MaxClanTier;

        /// <summary>Tier'ın setter'ı private; kademe AddRenown içinde şöhrete göre yeniden hesaplanır.</summary>
        public static void SetTier(int tier)
        {
            Me.Renown = Campaign.Current.Models.ClanTierModel.GetRequiredRenownForTier(tier);
            Me.AddRenown(0f, true);
            MenuUI.Success("Klan kademesi: " + Me.Tier + " (şöhret " + Me.Renown.ToString("N0") + ")");
        }

        public static void AddRenown(int amount)
        {
            Me.AddRenown(amount, true);
            MenuUI.Success("Şöhret +" + amount + " (kademe " + Me.Tier + ")");
        }

        // ---- Herkes ----

        public static void DeclareWar(IFaction a, IFaction b)
        {
            if (FactionManager.IsAtWarAgainstFaction(a, b)) throw new InvalidOperationException(a.Name + " ile " + b.Name + " zaten savaşta");
            DeclareWarAction.ApplyByDefault(a, b);
            MenuUI.Success(a.Name + " ile " + b.Name + " artık savaşta");
        }

        public static void MakePeace(IFaction a, IFaction b)
        {
            if (!FactionManager.IsAtWarAgainstFaction(a, b)) throw new InvalidOperationException(a.Name + " ile " + b.Name + " savaşta değil");
            if (FactionManager.IsAtConstantWarAgainstFaction(a, b)) throw new InvalidOperationException("Bu iki taraf arasında barış mümkün değil");
            MakePeaceAction.Apply(a, b);
            MenuUI.Success(a.Name + " ile " + b.Name + " barış yaptı");
        }

        public static void MoveClanToKingdom(Clan clan, Kingdom kingdom)
        {
            if (clan == Clan.PlayerClan) throw new InvalidOperationException("Kendi klanın için 'Benim klanım' seçeneklerini kullan");
            if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan)
                throw new InvalidOperationException(clan.Name + " kendi krallığının hükümdar klanı");
            if (clan.Kingdom == kingdom) throw new InvalidOperationException(clan.Name + " zaten " + kingdom.Name + " içinde");
            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, default, true);
            MenuUI.Success(clan.Name + " artık " + kingdom.Name + " krallığında");
        }

        public static void ChangeClanLeader(Clan clan, Hero newLeader)
        {
            if (clan == Clan.PlayerClan)
                throw new InvalidOperationException("Kendi klanının liderini değiştirmek oyuncu karakterini değiştirir; desteklenmiyor");
            ChangeClanLeaderAction.ApplyWithSelectedNewLeader(clan, newLeader);
            MenuUI.Success(clan.Name + " klanının yeni lideri: " + newLeader.Name);
        }

        public static void ChangeRuler(Kingdom kingdom, Clan clan)
        {
            if (kingdom.RulingClan == clan) throw new InvalidOperationException(clan.Name + " zaten hükümdar klan");
            if (clan.IsUnderMercenaryService) throw new InvalidOperationException("Paralı asker klan hükümdar olamaz");
            ChangeRulingClanAction.Apply(kingdom, clan);
            MenuUI.Success(kingdom.Name + " krallığının yeni hükümdar klanı: " + clan.Name);
        }

        public static void DestroyKingdom(Kingdom kingdom)
        {
            if (kingdom == Me.Kingdom) throw new InvalidOperationException("Kendi krallığını yok edemezsin");
            DestroyKingdomAction.Apply(kingdom);
            MenuUI.Success(kingdom.Name + " yok edildi");
        }
    }
}
```

- [ ] **Step 2:** `src/Cheats/MapInfo.cs`:

```csharp
using CheatMenu.Menu;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Bilgi araçları: bir kahramanın gerçek konumu ve harita sisini kaldırma.
    /// Campaign.TrueSight kaydedilmeyen bir çalışma anı bayrağıdır (oyunun Ctrl+Alt+T hilesi de bunu kullanır);
    /// yüklemede sıfırlandığı için seçili durum her karede yeniden uygulanır.
    /// </summary>
    public static class MapInfo
    {
        private static bool _resetPending;

        public static void SetReveal(bool on)
        {
            CheatState.RevealMap = on;
            _resetPending = true;
            Tick();
        }

        public static void Tick()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null) return;
            if (!CheatState.RevealMap && !_resetPending) return;
            if (campaign.TrueSight != CheatState.RevealMap) campaign.TrueSight = CheatState.RevealMap;
            _resetPending = false;
        }

        /// <summary>Kahramanın gerçek konumunu yazar ve kamerayı oraya götürür.</summary>
        public static void FindHero(Hero hero)
        {
            if (!TryLocate(hero, out CampaignVec2 position, out string where))
            {
                MenuUI.Error(hero.Name + ": " + where);
                return;
            }
            MapScreen.Instance?.FastMoveCameraToPosition(position);
            MenuUI.Info(hero.Name + ": " + where);
        }

        private static bool TryLocate(Hero hero, out CampaignVec2 position, out string where)
        {
            position = CampaignVec2.Invalid;

            PartyBase captor = hero.IsPrisoner ? hero.PartyBelongedToAsPrisoner : null;
            if (captor != null)
            {
                position = captor.Position;
                where = captor.IsSettlement ? captor.Name + " zindanında esir" : captor.Name + " elinde esir";
                return true;
            }

            MobileParty party = hero.PartyBelongedTo;
            Settlement settlement = party?.CurrentSettlement ?? hero.CurrentSettlement;
            if (settlement != null)
            {
                position = settlement.GatePosition;
                where = settlement.Name + " içinde";
                return true;
            }
            if (party != null)
            {
                position = party.Position;
                where = (party.IsCurrentlyAtSea ? "denizde, " : "seferde, ") + NearestSettlement(position) + " yakınında";
                return true;
            }

            where = "konumu bilinmiyor (haritada değil)";
            return false;
        }

        private static string NearestSettlement(CampaignVec2 position)
        {
            Vec2 point = position.ToVec2();
            Settlement nearest = null;
            float best = float.MaxValue;
            foreach (Settlement s in Settlement.All)
            {
                if (!s.IsTown && !s.IsCastle && !s.IsVillage) continue;
                float distance = point.DistanceSquared(s.GatePosition.ToVec2());
                if (distance < best)
                {
                    best = distance;
                    nearest = s;
                }
            }
            return nearest != null ? nearest.Name.ToString() : "?";
        }
    }
}
```

- [ ] **Step 3: SubModule** — `TimeSpeed.Tick();` altına `MapInfo.Tick();`.

- [ ] **Step 4: Derle** — hata yok.

---

### Task 3: Menüler

- [ ] **Step 1: Categories.cs** — kök listeye `Eşya ekle`'nin altına:

```csharp
                MenuEntry.Submenu("Klan & krallık", ShowPolitics),
                MenuEntry.Submenu("Bilgi", ShowInfo),
```

- [ ] **Step 2:** `src/Menu/Categories.Politics.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Menu
{
    /// <summary>Klan & krallık araçları ve Bilgi araçları.</summary>
    public static partial class Categories
    {
        private const string IrreversibleWarning =
            "Bu işlem geri alınamaz. Önce oyunu kaydetmeni öneririm. Devam edilsin mi?";

        // ---- Klan & krallık ----

        private static void ShowPolitics()
        {
            Clan me = Clan.PlayerClan;
            ShowCategory("Klan & krallık", new List<MenuEntry>
            {
                MenuEntry.Submenu("Bir krallığa vasal olarak katıl...", () => PickKingdom("Vasal olarak katıl", KingdomTools.JoinAsVassal)),
                MenuEntry.Submenu("Bir krallığa paralı asker olarak katıl...", () => PickKingdom("Paralı asker ol", KingdomTools.JoinAsMercenary)),
                MenuEntry.Simple("Krallığımdan ayrıl" + (me.Kingdom != null ? " (" + me.Kingdom.Name + ")" : ""), KingdomTools.LeaveKingdom),
                MenuEntry.Submenu("Kendi krallığını kur...", AskCreateKingdom),
                MenuEntry.Submenu("Krallığımın hükümdarı ol", () => ConfirmThen("Hükümdar ol", KingdomTools.BecomeRuler, ShowPolitics)),
                MenuEntry.Range("Klan kademesi: " + me.Tier, KingdomTools.MinTier, KingdomTools.MaxTier, () => me.Tier, KingdomTools.SetTier),
                MenuEntry.Simple("+500 şöhret", () => KingdomTools.AddRenown(500)),
                MenuEntry.Submenu("Savaş ilan ettir...", () => PickTwoFactions("Savaş", KingdomTools.PeacefulWith, KingdomTools.DeclareWar)),
                MenuEntry.Submenu("Barış yaptır...", () => PickTwoFactions("Barış", KingdomTools.AtWarWith, KingdomTools.MakePeace)),
                MenuEntry.Submenu("Bir klanı bir krallığa sok...", PickClanToMove),
                MenuEntry.Submenu("Bir klanın liderini değiştir...", PickClanForLeader),
                MenuEntry.Submenu("Bir krallığın hükümdarını değiştir...", PickKingdomForRuler),
                MenuEntry.Submenu("Bir krallığı yok et...", PickKingdomToDestroy),
            }, ShowPolitics);
        }

        private static void PickKingdom(string title, Action<Kingdom> apply) =>
            Picker.Pick(title, KingdomTools.LiveKingdoms(), k => k.Name.ToString(),
                k =>
                {
                    MenuUI.SafeRun(title, () => apply(k));
                    MenuUI.Defer(ShowPolitics);
                },
                ShowPolitics);

        private static void AskCreateKingdom() =>
            MenuUI.Confirm("Krallık kur", IrreversibleWarning,
                () => MenuUI.AskName("Yeni krallığın adı", Clan.PlayerClan.Name.ToString(), false,
                    KingdomTools.CreateKingdom, ShowPolitics),
                ShowPolitics);

        private static void PickTwoFactions(string verb, Func<IFaction, IEnumerable<IFaction>> partners,
            Action<IFaction, IFaction> apply) =>
            Picker.Pick(verb + ": birinci taraf", KingdomTools.Factions(), FactionLabel,
                a => Picker.Pick(verb + ": " + a.Name + " ile kim?", partners(a), FactionLabel,
                    b =>
                    {
                        MenuUI.SafeRun(verb, () => apply(a, b));
                        MenuUI.Defer(ShowPolitics);
                    },
                    ShowPolitics),
                ShowPolitics);

        private static string FactionLabel(IFaction faction) =>
            faction == Clan.PlayerClan ? "[Benim klanım] " + faction.Name : faction.Name.ToString();

        private static void PickClanToMove() =>
            Picker.Pick("Hangi klan?", KingdomTools.MovableClans(), Lists.ClanLabel,
                clan => Picker.Pick(clan.Name + " hangi krallığa?", KingdomTools.LiveKingdoms().Where(k => k != clan.Kingdom),
                    k => k.Name.ToString(),
                    k =>
                    {
                        MenuUI.SafeRun("Klanı taşı", () => KingdomTools.MoveClanToKingdom(clan, k));
                        MenuUI.Defer(ShowPolitics);
                    },
                    ShowPolitics),
                ShowPolitics);

        private static void PickClanForLeader() =>
            Picker.Pick("Hangi klanın lideri?", Lists.Clans().Where(c => c != Clan.PlayerClan), Lists.ClanLabel,
                clan => Picker.Pick(clan.Name + " - yeni lider", KingdomTools.LeaderCandidates(clan), Lists.HeroLabel,
                    hero => ConfirmThen("Klan lideri değiştir", () => KingdomTools.ChangeClanLeader(clan, hero), ShowPolitics),
                    ShowPolitics),
                ShowPolitics);

        private static void PickKingdomForRuler() =>
            Picker.Pick("Hangi krallık?", KingdomTools.LiveKingdoms(), k => k.Name + " (hükümdar: " + k.RulingClan?.Name + ")",
                k => Picker.Pick(k.Name + " - yeni hükümdar klan", KingdomTools.RulerCandidates(k), Lists.ClanLabel,
                    clan => ConfirmThen("Hükümdar değiştir", () => KingdomTools.ChangeRuler(k, clan), ShowPolitics),
                    ShowPolitics),
                ShowPolitics);

        private static void PickKingdomToDestroy() =>
            Picker.Pick("Yok edilecek krallık", KingdomTools.LiveKingdoms().Where(k => k != Clan.PlayerClan.Kingdom),
                k => k.Name.ToString(),
                k => ConfirmThen(k.Name + " yok edilsin mi?", () => KingdomTools.DestroyKingdom(k), ShowPolitics),
                ShowPolitics);

        private static void ConfirmThen(string title, Action action, Action back) =>
            MenuUI.Confirm(title, IrreversibleWarning,
                () =>
                {
                    MenuUI.SafeRun(title, action);
                    MenuUI.Defer(back);
                },
                back);

        // ---- Bilgi ----

        private static void ShowInfo()
        {
            ShowCategory("Bilgi", new List<MenuEntry>
            {
                MenuEntry.Submenu("Bir lordu bul...", () => Picker.Pick("Bir lordu bul", Lists.LordsAndCompanions(false),
                    Lists.HeroLabel, MapInfo.FindHero, ShowInfo)),
                MenuEntry.Toggle("Harita sisini kaldır (herkesi göster)", () => CheatState.RevealMap, MapInfo.SetReveal),
            }, ShowInfo, "Listedeki konumlar gerçek konumdur (oyunun 'son görülme' bilgisi değil).");
        }
    }
}
```

- [ ] **Step 3: Derle + doğrula** — build.ps1 hata yok; `verify_mod.ps1`: tipler yüklenir, oyunun `CampaignCheats` sınıfına 0 çağrı.

---

### Task 4: Oyun içi kontrol listesi (kullanıcı)

- [ ] Bağımsızken "Vasal olarak katıl" → krallık listesi → katıldın; "Krallığımdan ayrıl" → ayrıldın.
- [ ] Paralı asker olarak katıl → paralı askersin; aynı krallığa vasal olarak katıl → vasal oldun.
- [ ] Kendi krallığını kur (bağımsızken, yedekli kayıtta) → "X Krallığı" ansiklopedide.
- [ ] Bir krallıktayken "Krallığımın hükümdarı ol" → onay → hükümdarsın.
- [ ] Klan kademesi 5 → klan ekranında kademe 5.
- [ ] Savaş ilan ettir (iki AI krallığı) → ansiklopedide savaşta; Barış yaptır → barış.
- [ ] Bir AI klanını başka krallığa sok; bir AI klanının liderini değiştir; bir krallığın hükümdarını değiştir.
- [ ] Bir AI krallığını yok et (yedekli kayıtta) → krallık listesinden kalkar, klanları bağımsız.
- [ ] Bilgi → Bir lordu bul → kamera lorda kayar, konum mesajı doğru.
- [ ] Harita sisini kaldır AÇIK → uzaktaki partiler görünür; kaydet-yükle → hâlâ görünür; KAPALI → normal.

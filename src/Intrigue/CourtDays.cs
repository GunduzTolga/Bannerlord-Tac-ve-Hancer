using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Saray günü (P2.9, tasarım: docs/superpowers/specs/2026-10-19-p2-saray-gunu.md). Kendi şehrinde ya da kalende "Divan kur": üç
    /// dilekçe resimli kartla gelir; her seçeneğin sonucu düğmede yazar. Kararlar oyunun değerlerini (sadakat, güvenlik, refah, erzak),
    /// eşrafla ilişkini ve kişilik özelliklerini (oyunun görev ödülleri gibi) etkiler. Aynı yerde 21 günde bir.
    /// </summary>
    public static class CourtDays
    {
        private static int Today => SchemeFactors.Today;

        public static void RegisterMenus(CampaignGameStarter starter)
        {
            foreach (string menu in new[] { "town", "castle" })
                starter.AddGameMenuOption(menu, "tvh_courtday_" + menu, "Divan kur (saray günü)",
                    args =>
                    {
                        args.optionLeaveType = GameMenuOption.LeaveType.Manage;
                        Settlement here = Settlement.CurrentSettlement;
                        if (here?.Town == null || here.OwnerClan != Clan.PlayerClan) return false;
                        string blocked = Blocked(SchemeEngine.State, here);
                        if (blocked != null)
                        {
                            args.IsEnabled = false;
                            args.Tooltip = new TextObject(blocked);
                        }
                        return true;
                    },
                    _ => MenuUI.SafeRun("Divan", () => Start(SchemeEngine.State, Settlement.CurrentSettlement)),
                    false, 6);
        }

        public static string Blocked(IntrigueState state, Settlement settlement)
        {
            if (settlement?.Town == null || settlement.OwnerClan != Clan.PlayerClan) return "Yalnızca kendi şehrinde ya da kalende";
            if (state.CourtDayUntil.TryGetValue(settlement.StringId, out int until) && Today < until)
                return "Son divandan beri az zaman geçti; " + (until - Today) + " gün sonra";
            return null;
        }

        public static void Start(IntrigueState state, Settlement settlement)
        {
            string blocked = Blocked(state, settlement);
            if (blocked != null) throw new InvalidOperationException(blocked);
            state.CourtDayUntil[settlement.StringId] = Today + PetitionRules.CooldownDays;
            List<Petition> petitions = PetitionRules.Draw(settlement.IsTown, count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count));
            state.AddLog(Today, Tr.Locative(settlement.Name.ToString()) + " divan kuruldu");
            ShowNext(state, settlement, petitions, 0, new List<string>());
        }

        private static void ShowNext(IntrigueState state, Settlement settlement, List<Petition> petitions, int index, List<string> summary)
        {
            if (index >= petitions.Count)
            {
                var close = new UI.EventCards.CardOption { Label = "Divanı dağıt", Apply = () => { } };
                UI.EventCards.Show("DİVAN", "Divan dağıldı: " + settlement.Name,
                    "Bugünkü kararların:\n\n" + string.Join("\n", summary.Select(line => "· " + line)),
                    CardImages.CourtDay, null, new List<UI.EventCards.CardOption> { close }, close, null);
                return;
            }
            Petition petition = petitions[index];
            Hero petitioner = FindNotable(settlement, petition.Petitioner);
            List<UI.EventCards.CardOption> options = petition.Options.Select(option => new UI.EventCards.CardOption
            {
                Label = option.Label + Effects(option.Effect),
                DisabledHint = option.Effect.Gold < 0 && !SchemeFactors.CanAfford(-option.Effect.Gold) ? "Yeterli altının yok" : null,
                Apply = () =>
                {
                    string result = Apply(state, settlement, option.Effect, petitioner);
                    summary.Add(petition.Title + ": " + option.Label.ToLower(Tr.Culture) + (result.Length > 0 ? " (" + result + ")" : ""));
                    MenuUI.Defer(() => ShowNext(state, settlement, petitions, index + 1, summary));
                },
            }).ToList();
            UI.EventCards.Show("DİVAN " + (index + 1) + "/" + petitions.Count, petition.Title,
                petition.Text.Replace("{yer}", settlement.Name.ToString())
                + (petitioner != null ? "\n\nDilekçe sahibi: " + petitioner.Name + "." : ""),
                CardImages.CourtDay, petitioner, options, null, null);
        }

        private static string Effects(PetitionEffect effect)
        {
            string text = PetitionRules.EffectText(effect);
            return text.Length > 0 ? " (" + text + ")" : "";
        }

        /// <summary>Kararın sonuçlarını uygular; kart özetine yazılacak metni döner.</summary>
        private static string Apply(IntrigueState state, Settlement settlement, PetitionEffect e, Hero petitioner)
        {
            Town town = settlement.Town;
            if (e.Gold < 0) SchemeFactors.Pay(-e.Gold);
            if (e.Gold > 0) Hero.MainHero.ChangeHeroGold(e.Gold);
            if (e.Loyalty != 0) town.Loyalty = SchemeMath.Clamp((int)town.Loyalty + e.Loyalty, 0, 100);
            if (e.Security != 0) town.Security = SchemeMath.Clamp((int)town.Security + e.Security, 0, 100);
            if (e.Prosperity != 0) town.Prosperity = Math.Max(0f, town.Prosperity + e.Prosperity);
            if (e.Food != 0) town.FoodStocks = Math.Max(0f, Math.Min(town.FoodStocksUpperLimit(), town.FoodStocks + e.Food));
            if (e.Influence != 0) ChangeClanInfluenceAction.Apply(Clan.PlayerClan, e.Influence);
            if (petitioner != null && e.PetitionerRelation != 0) SchemeOutcomes.Relate(petitioner, e.PetitionerRelation);
            Hero other = e.OtherKind != NotableKind.None ? FindNotable(settlement, e.OtherKind) : null;
            if (other != null && e.OtherRelation != 0) SchemeOutcomes.Relate(other, e.OtherRelation);
            TraitObject trait = Trait(e.Trait);
            if (trait != null && e.TraitXp != 0)
                TraitLevelingHelper.OnIssueSolvedThroughQuest(petitioner ?? Hero.MainHero, new[] { Tuple.Create(trait, e.TraitXp) });
            if (e.Intel)
            {
                Hero master = SpyMaster(state);
                if (master != null)
                {
                    IntelReport report = SchemeOutcomes.CreateReport(master, Today);
                    state.Reports.RemoveAll(r => r.TargetId == master.StringId);
                    state.Reports.Add(report);
                    return "casus " + master.Name + " için çalışıyormuş: hakkında rapor Kişiler'de";
                }
            }
            return "";
        }

        /// <summary>Yerleşimin (ya da bağlı köylerinin) bu türden bir eşrafı; yoksa null.</summary>
        private static Hero FindNotable(Settlement settlement, NotableKind kind)
        {
            if (kind == NotableKind.None) return null;
            IEnumerable<Hero> notables = settlement.Notables.Concat(settlement.BoundVillages.SelectMany(v => v.Settlement.Notables));
            return notables.Where(n => n.IsAlive && Matches(n, kind)).OrderBy(n => n.StringId, StringComparer.Ordinal).FirstOrDefault();
        }

        private static bool Matches(Hero notable, NotableKind kind)
        {
            switch (kind)
            {
                case NotableKind.Artisan: return notable.IsArtisan;
                case NotableKind.Merchant: return notable.IsMerchant;
                case NotableKind.GangLeader: return notable.IsGangLeader;
                case NotableKind.Preacher: return notable.IsPreacher;
                case NotableKind.Headman: return notable.IsHeadman || notable.IsRuralNotable;
                default: return false;
            }
        }

        private static TraitObject Trait(TraitKind kind)
        {
            switch (kind)
            {
                case TraitKind.Mercy: return DefaultTraits.Mercy;
                case TraitKind.Generosity: return DefaultTraits.Generosity;
                case TraitKind.Honor: return DefaultTraits.Honor;
                case TraitKind.Calculating: return DefaultTraits.Calculating;
                case TraitKind.Valor: return DefaultTraits.Valor;
                default: return null;
            }
        }

        /// <summary>Yakalanan casusun efendisi: önce savaşta olduğun, yoksa başka bir krallığın klan liderlerinden biri (tohumlu).</summary>
        private static Hero SpyMaster(IntrigueState state)
        {
            IFaction mine = Clan.PlayerClan.MapFaction;
            List<Hero> leaders = Kingdom.All.Where(k => !k.IsEliminated && k != Clan.PlayerClan.Kingdom)
                .SelectMany(k => k.Clans).Where(c => !c.IsEliminated && c.Leader != null && c.Leader.IsAlive && c != Clan.PlayerClan)
                .Select(c => c.Leader).OrderBy(h => h.StringId, StringComparer.Ordinal).ToList();
            List<Hero> enemies = leaders.Where(h => FactionManager.IsAtWarAgainstFaction(h.MapFaction, mine)).ToList();
            List<Hero> pool = enemies.Count > 0 ? enemies : leaders;
            return pool.Count == 0 ? null : pool[SchemeRandom.Pick(state.Seed, ref state.Rolls, pool.Count)];
        }
    }
}

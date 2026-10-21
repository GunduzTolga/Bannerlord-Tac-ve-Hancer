using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Lordlarla iletişim (tasarım: docs/superpowers/specs/2026-10-21-lordlar.md): bir lordu yanına çağırmak (partisi oyunun eşlik
    /// emriyle sana gelir; konuşma oyunun kendi diyaloğu) ve kendi şehrinde ya da kalende ziyafet vermek. Ulak ve hediye: Messengers.
    /// Kurallar: LordContactRules. Pencere: UI.LordsScreen.
    /// </summary>
    public static class LordContacts
    {
        private static int Today => SchemeFactors.Today;

        private static bool SameKingdom(Hero lord) => Clan.PlayerClan.Kingdom != null && lord.Clan?.Kingdom == Clan.PlayerClan.Kingdom;

        private static bool AtWarWith(Hero lord) =>
            lord.MapFaction != null && Clan.PlayerClan.MapFaction != null && FactionManager.IsAtWarAgainstFaction(lord.MapFaction, Clan.PlayerClan.MapFaction);

        // ---- Yanına çağır ----

        public static Summon SummonOf(IntrigueState state, Hero lord) => state.Summons.FirstOrDefault(s => s.HeroId == lord.StringId);

        /// <summary>Gelme olasılığı; diplomatın varsa Charm'ı kadar artar (en fazla %95).</summary>
        public static int SummonChance(Hero lord) =>
            SchemeMath.Clamp(LordContactRules.SummonAcceptChance(lord.GetRelation(Hero.MainHero), SameKingdom(lord), SameKingdom(lord) && Hero.MainHero.IsKingdomLeader)
                + Diplomat.InviteBonus(), 5, 95);

        /// <summary>Çağırmanın bedeli: aynı krallıktansa nüfuz, değilse altın.</summary>
        public static string SummonCostText(Hero lord) =>
            SameKingdom(lord) ? LordContactRules.SummonInfluence + " nüfuz" : SchemeFactors.Money(LordContactRules.SummonGold);

        /// <summary>null: çağrılabilir; değilse neden.</summary>
        public static string SummonBlocked(IntrigueState state, Hero lord)
        {
            if (lord == null || !lord.IsAlive) return "Hayatta değil";
            if (lord.Clan == Clan.PlayerClan) return "Kendi klanından: ona zaten emir verebilirsin";
            if (lord.IsPrisoner) return "Esir";
            MobileParty party = lord.PartyBelongedTo;
            if (party == null || party.LeaderHero != lord) return "Başında bir parti yok";
            if (party.Army != null) return "Bir orduda";
            if (party.MapEvent != null || party.SiegeEvent != null) return "Savaşta ya da kuşatmada";
            if (AtWarWith(lord)) return "Düşmanın: çağrına gelmez";
            if (SummonOf(state, lord) != null) return "Zaten sana doğru yolda";
            if (MobileParty.MainParty == null || Hero.MainHero.IsPrisoner) return "Şu an olmaz";
            if (SameKingdom(lord)) return Clan.PlayerClan.Influence >= LordContactRules.SummonInfluence ? null : "Yeterli nüfuzun yok";
            return SchemeFactors.CanAfford(LordContactRules.SummonGold) ? null : "Yeterli altının yok";
        }

        /// <summary>Bedel ödenir, lord karar verir: gelirse partisi sana yürür (eşlik), gelmezse bedelin yarısı geri döner.</summary>
        public static string Summon(IntrigueState state, Hero lord)
        {
            string blocked = SummonBlocked(state, lord);
            if (blocked != null) throw new InvalidOperationException(blocked);
            bool same = SameKingdom(lord);
            if (same) ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -LordContactRules.SummonInfluence);
            else SchemeFactors.Pay(LordContactRules.SummonGold);
            if (state.Roll100() > SummonChance(lord))
            {
                if (same) ChangeClanInfluenceAction.Apply(Clan.PlayerClan, LordContactRules.SummonInfluence / 2);
                else Hero.MainHero.ChangeHeroGold(LordContactRules.SummonGold / 2);
                state.AddLog(Today, lord.Name + " çağrına gelmedi");
                return lord.Name + " çağrına nazikçe karşılık verdi ama gelmiyor: başka işleri var. Bedelin yarısı geri döndü.";
            }
            state.Summons.Add(new Summon { HeroId = lord.StringId, Until = Today + LordContactRules.SummonDays });
            Order(lord.PartyBelongedTo, true);
            state.AddLog(Today, lord.Name + " çağrıldı");
            return lord.Name + " çağrını kabul etti; partisiyle sana doğru yola çıktı. Gelince haritada üstüne tıklayıp konuşursun ("
                + LordContactRules.SummonDays + " günde gelemezse vazgeçer).";
        }

        /// <summary>Lordun partisine sana eşlik etme emri; yolda kendi kararlarını vermez. false: emir kalkar, kendi işine döner.</summary>
        private static void Order(MobileParty party, bool come)
        {
            if (party?.Ai == null) return;
            if (come)
            {
                party.SetMoveEscortParty(MobileParty.MainParty, MobileParty.NavigationType.Default, false);
                party.Ai.SetDoNotMakeNewDecisions(true);
            }
            else party.Ai.SetDoNotMakeNewDecisions(false);
        }

        /// <summary>Her saat: gelen lord bildirilir ve serbest kalır; süresi dolan ya da yolu kesilen vazgeçer. Emir her saat tazelenir.</summary>
        public static void Hourly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.Summons.Count == 0 || MobileParty.MainParty == null) return;
            foreach (Summon summon in state.Summons.ToList())
            {
                Hero lord = Hero.Find(summon.HeroId);
                MobileParty party = lord?.PartyBelongedTo;
                bool usable = lord != null && lord.IsAlive && !lord.IsPrisoner && party != null && party.LeaderHero == lord && party.Army == null
                    && party.MapEvent == null && party.SiegeEvent == null && !AtWarWith(lord) && !Hero.MainHero.IsPrisoner;
                if (!usable || Today >= summon.Until)
                {
                    state.Summons.Remove(summon);
                    Order(party, false);
                    if (lord != null)
                        SchemeOutcomes.Notify(state, "Gelemedi: " + lord.Name, lord.Name + " yanına gelemedi" + (usable ? " (yol çok uzun sürdü)" : "") + "; kendi işine döndü.",
                            lord, CardImages.Messenger);
                    continue;
                }
                if (party.GetPosition2D.Distance(MobileParty.MainParty.GetPosition2D) <= LordContactRules.ArriveDistance)
                {
                    state.Summons.Remove(summon);
                    Order(party, false);
                    MenuUI.Success(lord.Name + " yanına geldi: haritada üstüne tıklayıp konuşabilirsin");
                    state.AddLog(Today, lord.Name + " çağrına geldi");
                    continue;
                }
                Order(party, true);
            }
        }

        // ---- Ziyafet ----

        /// <summary>Ziyafetin verileceği yer: içinde bulunduğun kendi şehrin ya da kalen; yoksa null.</summary>
        public static Settlement FeastHall() =>
            Settlement.CurrentSettlement is Settlement s && s.IsFortification && s.OwnerClan == Clan.PlayerClan ? s : null;

        /// <summary>Ziyafete gelme olasılığı; savaştaysanız 0, değilse diplomatın Charm'ı kadar artar (en fazla %95).</summary>
        public static int AcceptChance(Hero lord)
        {
            int chance = LordContactRules.FeastAcceptChance(lord.GetRelation(Hero.MainHero), Messengers.TravelDays(lord), SameKingdom(lord), AtWarWith(lord));
            return chance <= 0 ? 0 : SchemeMath.Clamp(chance + Diplomat.InviteBonus(), 5, 95);
        }

        /// <summary>null: bu konuklarla ziyafet verilebilir; değilse neden.</summary>
        public static string FeastBlocked(IntrigueState state, IList<Hero> guests)
        {
            if (FeastHall() == null) return "Kendi şehrinde ya da kalende olmalısın";
            if (state.FeastUntil > Today) return "Son ziyafetin üstünden az zaman geçti (" + (state.FeastUntil - Today) + " gün)";
            if (guests == null || guests.Count == 0) return "Davet listesi boş: lord kartlarından davet et";
            if (guests.Count > LordContactRules.MaxGuests) return "En fazla " + LordContactRules.MaxGuests + " davetli";
            return SchemeFactors.CanAfford(LordContactRules.FeastCost(guests.Count)) ? null : "Yeterli altının yok";
        }

        /// <summary>Ziyafet verilir: kim geldi, ilişki ve nüfuz, sofrada konuşulanlar. Sonuç metni (kart için) döner.</summary>
        public static string Feast(IntrigueState state, IList<Hero> guests)
        {
            string blocked = FeastBlocked(state, guests);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Settlement hall = FeastHall();
            SchemeFactors.Pay(LordContactRules.FeastCost(guests.Count));
            state.FeastUntil = Today + LordContactRules.FeastCooldownDays;
            var came = new List<Hero>();
            var declined = new List<Hero>();
            foreach (Hero guest in guests.Where(g => g != null && g.IsAlive))
                (state.Roll100() <= AcceptChance(guest) && !guest.IsPrisoner ? came : declined).Add(guest);

            int gain = LordContactRules.FeastRelationGain(Hero.MainHero.GetSkillValue(DefaultSkills.Charm));
            foreach (Hero guest in came) SchemeOutcomes.Relate(guest, gain);
            int influence = Clan.PlayerClan.Kingdom != null ? came.Count * LordContactRules.InfluencePerGuest : 0;
            if (influence > 0) ChangeClanInfluenceAction.Apply(Clan.PlayerClan, influence);

            var lines = new List<string>
            {
                Tr.Locative(hall.Name.ToString()) + " salon kuruldu: uzun masalar, kızarmış etler, şarap ve çalgıcılar.",
            };
            if (came.Count > 0)
                lines.Add("Gelenler: " + string.Join(", ", came.Select(h => h.Name)) + ". Her biriyle ilişki +" + gain
                    + (influence > 0 ? ", nüfuz +" + influence : "") + ".");
            else lines.Add("Kimse gelmedi; masalar boş kaldı.");
            if (declined.Count > 0) lines.Add("Gelmeyenler: " + string.Join(", ", declined.Select(h => h.Name)) + " (özür diledi ya da yol uzundu).");

            Hero heard = null;
            if (came.Count > 0 && state.Roll100() <= LordContactRules.FeastGossipChance(Interrogation.Roguery(state)))
            {
                heard = came[state.Pick(came.Count)];
                state.Reports.RemoveAll(r => r.TargetId == heard.StringId);
                state.Reports.Add(SchemeOutcomes.CreateReport(heard, Today));
                string secretLine = "";
                if (state.Roll100() <= LordContactRules.FeastSecretChance)
                {
                    Secret secret = SchemeOutcomes.FindSecret(state, heard, state.Roll100, count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count));
                    if (secret != null)
                    {
                        RomanceSecrets.Record(state, secret);
                        secretLine = " " + SchemeOutcomes.SecretFoundLine(secret);
                    }
                }
                lines.Add("Şarap dilleri çözdü: " + heard.Name + " anlattıklarıyla bir casusluk raporu kadar bilgi verdi (Kişiler)." + secretLine);
            }
            state.AddLog(Today, Tr.Locative(hall.Name.ToString()) + " ziyafet: " + came.Count + " konuk");
            return string.Join("\n\n", lines);
        }
    }
}

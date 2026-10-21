using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Bundle;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Savaş Raporu'nun alt bölümleri (tasarım: docs/superpowers/specs/2026-10-21-savas-raporu.md). Üst kısım (sancaklar, savaş yorgunluğu)
    /// Diplomacy'nin kendi ViewModel'iyle çizilir; buradakiler oyunun savaş kayıtlarından (StanceLink), oyunun diplomasi modelinden,
    /// Diplomacy'nin yorgunluk dökümü ve barış koşullarından, serdardan ve oyunun savaş günlüğünden. Pencere: UI.WarReportScreen.
    /// </summary>
    public static class WarReport
    {
        public sealed class Row
        {
            public string Label, Ours, Theirs;
            /// <summary>Karşılaştırma çubuğu çizilsin mi; çizilirse değerler.</summary>
            public bool Bar;
            public double OurValue, TheirValue;
            /// <summary>Hazır ipucu (Diplomacy'nin BasicTooltipViewModel'i) ya da satırlar (ad, değer).</summary>
            public object OursHint, TheirsHint;
            public List<(string Name, string Value)> OursTip, TheirsTip;
        }

        public sealed class Section
        {
            public string Title;
            /// <summary>Başlığın altında iki tarafın adı (sol biz, sağ onlar).</summary>
            public bool Sides;
            public readonly List<Row> Rows = new List<Row>();
            public readonly List<string> Lines = new List<string>();
        }

        private const int MaxLog = 10;
        private const int MaxNames = 6;

        /// <summary>Sol sütun: gidişat, kuvvetler, savaş yorgunluğu dökümü.</summary>
        public static List<Section> Left(Kingdom ours, Kingdom enemy, object diplomacyReport) => new List<Section>
        {
            Safe("Savaşın gidişatı", true, s => Progress(s, ours, enemy)),
            Safe("Kuvvetler", true, s => Forces(s, ours, enemy)),
            Safe("Savaş yorgunluğu (Diplomacy)", true, s => Exhaustion(s, diplomacyReport)),
        };

        /// <summary>Sağ sütun: serdar, barış, esirler, savaş günlüğü.</summary>
        public static List<Section> Right(Kingdom ours, Kingdom enemy, object diplomacyReport) => new List<Section>
        {
            Safe("Serdarın raporu", true, s => MarshalView(s, enemy)),
            Safe("Barış", false, s => Peace(s, ours, enemy, diplomacyReport)),
            Safe("Esirler", false, s => Prisoners(s, ours, enemy)),
            Safe("Savaş günlüğü", false, s => Log(s, ours, enemy)),
        };

        /// <summary>Bir bölüm okunamazsa pencere yine açılır; bölümde nedeni yazar.</summary>
        private static Section Safe(string title, bool sides, Action<Section> fill)
        {
            var section = new Section { Title = title, Sides = sides };
            try
            {
                fill(section);
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] war report section '" + title + "' failed: " + ex);
                section.Rows.Clear();
                section.Lines.Clear();
                section.Lines.Add("Bu bölüm okunamadı (" + ex.Message + ").");
            }
            return section;
        }

        private static Row Compare(string label, double ours, double theirs, bool bar = true) => new Row
        {
            Label = label, Ours = ((int)Math.Round(ours)).ToString("N0"), Theirs = ((int)Math.Round(theirs)).ToString("N0"),
            Bar = bar, OurValue = ours, TheirValue = theirs,
        };

        // ---- Gidişat: oyunun savaş kayıtları ----

        private static void Progress(Section s, Kingdom ours, Kingdom enemy)
        {
            StanceLink war = ours.GetStanceWith(enemy);
            var model = Campaign.Current.Models.DiplomacyModel;
            ExplainedNumber mine = model.GetWarProgressScore(ours, enemy, true);
            ExplainedNumber theirs = model.GetWarProgressScore(enemy, ours, true);
            Row progress = Compare("Savaştaki üstünlük (oyunun puanı)", mine.ResultNumber, theirs.ResultNumber);
            progress.OursTip = Tip(mine);
            progress.TheirsTip = Tip(theirs);
            s.Rows.Add(progress);

            s.Rows.Add(Compare("Kayıplar (asker)", war.GetCasualties(ours), war.GetCasualties(enemy)));
            Row sieges = Compare("Fethedilen şehir ve kale", war.GetSuccessfulSieges(ours), war.GetSuccessfulSieges(enemy));
            sieges.Ours += Towns(war.GetSuccessfulTownSieges(ours));
            sieges.Theirs += Towns(war.GetSuccessfulTownSieges(enemy));
            s.Rows.Add(sieges);
            s.Rows.Add(Compare("Yağmalanan köy", war.GetSuccessfulRaids(ours), war.GetSuccessfulRaids(enemy)));
            s.Rows.Add(Compare("Esir düşen lordları", HeldBy(ours, enemy).Count, HeldBy(enemy, ours).Count));
            int paidByUs = war.GetTotalTributePaid(ours), paidByThem = war.GetTotalTributePaid(enemy);
            if (paidByUs > 0 || paidByThem > 0) s.Rows.Add(Compare("Ödenen haraç", paidByUs, paidByThem));
        }

        private static string Towns(int towns) => towns > 0 ? " (" + towns + " şehir)" : "";

        private static List<(string, string)> Tip(ExplainedNumber number) =>
            number.GetLines().Select(l => (l.name, SchemeFactors.Signed((int)Math.Round(l.number)))).ToList();

        // ---- Kuvvetler: herkesin bildiği sayılar ----

        private static void Forces(Section s, Kingdom ours, Kingdom enemy)
        {
            s.Rows.Add(Compare("Toplam güç", ours.CurrentTotalStrength, enemy.CurrentTotalStrength));
            s.Rows.Add(Compare("Sahadaki ordular", ours.Armies.Count, enemy.Armies.Count));
            Row clans = Compare("Klanlar", Clans(ours, false), Clans(enemy, false));
            clans.Ours += Mercenaries(ours);
            clans.Theirs += Mercenaries(enemy);
            s.Rows.Add(clans);
            s.Rows.Add(Compare("Lordlar", Lords(ours), Lords(enemy)));
            Row fiefs = Compare("Şehir / kale / köy", ours.Fiefs.Count, enemy.Fiefs.Count);
            fiefs.Ours = Holdings(ours);
            fiefs.Theirs = Holdings(enemy);
            s.Rows.Add(fiefs);
            s.Rows.Add(new Row { Label = "Müttefikler", Ours = Allies(ours), Theirs = Allies(enemy) });
            s.Rows.Add(new Row { Label = "Başka cepheler", Ours = OtherWars(ours, enemy), Theirs = OtherWars(enemy, ours) });
        }

        private static int Clans(Kingdom k, bool mercenary) => k.Clans.Count(c => !c.IsEliminated && c.IsUnderMercenaryService == mercenary);

        private static string Mercenaries(Kingdom k)
        {
            int count = Clans(k, true);
            return count > 0 ? " (+" + count + " paralı)" : "";
        }

        private static int Lords(Kingdom k) => k.AliveLords.Count(h => !h.IsChild);

        private static string Holdings(Kingdom k) =>
            k.Fiefs.Count(t => t.IsTown) + " / " + k.Fiefs.Count(t => t.IsCastle) + " / " + k.Settlements.Count(x => x.IsVillage);

        private static string Allies(Kingdom k) => WarReportRules.Names(k.AlliedKingdoms.Select(a => a.Name.ToString()).ToList(), 2);

        private static string OtherWars(Kingdom k, Kingdom besides) =>
            WarReportRules.Names(Kingdom.All.Where(o => o != k && o != besides && !o.IsEliminated && FactionManager.IsAtWarAgainstFaction(o, k))
                .Select(o => o.Name.ToString()).ToList(), 2);

        // ---- Savaş yorgunluğu: Diplomacy'nin dökümü ----

        private static void Exhaustion(Section s, object diplomacyReport)
        {
            foreach (DiplomacyCompat.WarBreakdown row in DiplomacyCompat.WarBreakdowns(diplomacyReport))
                s.Rows.Add(new Row
                {
                    Label = row.Text,
                    Ours = Exhausted(row.Ours, row.OursPercent),
                    Theirs = Exhausted(row.Theirs, row.TheirsPercent),
                    OursHint = row.OursHint,
                    TheirsHint = row.TheirsHint,
                });
            if (s.Rows.Count == 0) s.Lines.Add("Diplomacy'nin dökümü okunamadı.");
            else s.Lines.Add("Üstüne gel: hangi olay ne kadar yordu. Yorgunluk %100'e varınca o taraf barışa zorlanır.");
        }

        private static string Exhausted(int count, string percent) => count > 0 ? count.ToString("N0") + "  ·  " + percent : percent;

        // ---- Serdar ----

        private static void MarshalView(Section s, Kingdom enemy)
        {
            foreach ((string text, string ours, string theirs) in Marshal.WarReportRows(enemy))
                s.Rows.Add(new Row { Label = text, Ours = ours, Theirs = theirs });
            IntrigueState state = IntrigueBehavior.Current?.State;
            Hero marshal = state != null ? Marshal.Working(state) : null;
            if (marshal == null) return;
            foreach (Marshal.ArmyLine threat in Marshal.Threats(state, marshal).Where(a => a.Party?.MapFaction == enemy).Take(3))
                s.Lines.Add("Tehlike: " + threat.Name + " (~" + threat.Men + ") " + Marshal.Goal(threat) + " geliyor, " + MarshalRules.TravelText(threat.HoursToTarget) + ".");
            foreach (Marshal.FortLine fort in Marshal.WeakTargets(state, marshal, 12).Where(f => f.Settlement.MapFaction == enemy).Take(3))
                s.Lines.Add("Zayıf hedef: " + fort.Settlement.Name + " - garnizon ~" + fort.Garrison + ", milis ~" + fort.Militia + ", sur " + fort.Wall
                    + (fort.FoodDays == int.MaxValue ? "" : ", yiyecek ~" + fort.FoodDays + " gün") + ".");
        }

        // ---- Barış ----

        private static void Peace(Section s, Kingdom ours, Kingdom enemy, object diplomacyReport)
        {
            var model = Campaign.Current.Models.DiplomacyModel;
            s.Lines.Add(enemy.Name + ": " + WarReportRules.PeaceMood(model.GetScoreOfDeclaringPeace(enemy, ours)) + ".");
            s.Lines.Add(ours.Name + " (senin krallığın): " + WarReportRules.PeaceMood(model.GetScoreOfDeclaringPeace(ours, enemy)) + ".");
            if (DiplomacyCompat.Read<bool>(diplomacyReport, "PlayerWarExhaustionIsCritical"))
                s.Lines.Add("Krallığın bu savaşta tükenmek üzere: barış yakında kaçınılmaz olabilir.");
            if (DiplomacyCompat.Read<bool>(diplomacyReport, "OpponentWarExhaustionIsCritical"))
                s.Lines.Add(enemy.Name + " tükenmek üzere: barışa zorlanabilirler.");
            List<string> blockers = DiplomacyCompat.PeaceBlockers(ours, enemy);
            if (blockers == null) return;
            s.Lines.Add(blockers.Count == 0 ? "Barış teklif edebilirsin: Krallık > Diplomasi." : "Barış teklifi şimdi olmaz: " + string.Join(" ", blockers));
        }

        // ---- Esirler ----

        /// <summary>side krallığının, holder krallığının elinde esir olan lordları.</summary>
        private static List<Hero> HeldBy(Kingdom side, Kingdom holder) =>
            side.AliveLords.Where(h => h.IsPrisoner && h.PartyBelongedToAsPrisoner?.MapFaction == holder).ToList();

        private static void Prisoners(Section s, Kingdom ours, Kingdom enemy)
        {
            s.Lines.Add("Onların elinde: " + WarReportRules.Names(HeldBy(ours, enemy).Select(Captive).ToList(), MaxNames) + ".");
            s.Lines.Add("Senin krallığının elinde: " + WarReportRules.Names(HeldBy(enemy, ours).Select(Captive).ToList(), MaxNames) + ".");
        }

        private static string Captive(Hero hero)
        {
            var where = hero.PartyBelongedToAsPrisoner;
            string place = where?.Settlement?.Name?.ToString() ?? where?.Name?.ToString();
            return hero.Name + (string.IsNullOrEmpty(place) ? "" : " (" + place + ")");
        }

        // ---- Savaş günlüğü: oyunun kendi kayıtları ----

        private static void Log(Section s, Kingdom ours, Kingdom enemy)
        {
            StanceLink war = ours.GetStanceWith(enemy);
            var logs = Campaign.Current.LogEntryHistory.GameActionLogs;
            for (int i = logs.Count - 1; i >= 0 && s.Lines.Count < MaxLog; i--)
            {
                TaleWorlds.CampaignSystem.LogEntries.LogEntry entry = logs[i];
                if (entry.GameTime < war.WarStartDate) break;
                if (!(entry is IWarLog warLog) || !(entry is IEncyclopediaLog text) || !warLog.IsRelatedToWar(war, out _, out _)) continue;
                string line = WarReportRules.StripTags(text.GetEncyclopediaText()?.ToString());
                if (line.Length > 0) s.Lines.Add(WarReportRules.Ago((int)(CampaignTime.Now - entry.GameTime).ToDays) + ": " + line);
            }
            if (s.Lines.Count == 0) s.Lines.Add("Henüz kayda geçen bir olay yok.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Genelev işletmesi (tasarım: docs/superpowers/specs/2026-10-19-p2-genelev.md). Gölge ağın olan şehirde çete lideri
    /// üzerinden açılır. Çalışanlar isimsiz, gönüllü ve ücretli yetişkinlerdir (yalnızca sayı): pay düşükse ayrılırlar. Haftalık gelir
    /// ve müşterilerden duyulanlar (rapor, bazen sır) getirir. Yalnızca metin; ayardan açılır (varsayılan kapalı).
    /// </summary>
    public static class NightHouses
    {
        public static bool Enabled => SettingsStore.Current.NightHouses;

        /// <summary>null: bu şehirde genelev açılabilir; değilse neden.</summary>
        public static string OpenBlocked(IntrigueState state, Settlement town)
        {
            if (!Enabled) return "Kurallar'dan (MCM > Entrika) açılmalı";
            if (town == null || !town.IsTown) return "Yalnızca şehirlerde";
            if (state.HouseIn(town.StringId) != null) return "Burada zaten bir genelevin var";
            if (state.Network(town.StringId) == null) return "Önce burada bir gölge ağı kur (çete lideri işletir)";
            return SchemeFactors.CanAfford(HouseRules.OpenCost) ? null : "Yeterli altının yok (" + SchemeFactors.Money(HouseRules.OpenCost) + ")";
        }

        public static void Open(IntrigueState state, Settlement town)
        {
            string blocked = OpenBlocked(state, town);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(HouseRules.OpenCost);
            state.Houses.Add(new House { TownId = town.StringId, Staff = HouseRules.StartStaff, Share = HouseShare.Standard, Since = SchemeFactors.Today });
            state.AddLog(SchemeFactors.Today, Tr.Locative(town.Name.ToString()) + " genelev açıldı");
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " genelev açıldı: " + HouseRules.StartStaff + " çalışan");
        }

        public static int MaxStaff(IntrigueState state, Settlement town) => HouseRules.MaxStaff(state.Network(town.StringId)?.Level ?? 1);

        public static string HireBlocked(IntrigueState state, Settlement town)
        {
            House house = state.HouseIn(town.StringId);
            if (house == null) return "Burada genelevin yok";
            if (house.Staff >= MaxStaff(state, town)) return "Ağın bu kadarını gizleyebilir (en fazla " + MaxStaff(state, town) + "; ağı güçlendir)";
            return SchemeFactors.CanAfford(HouseRules.HireCost) ? null : "Yeterli altının yok";
        }

        /// <summary>Gönüllü bir çalışan daha (isimsiz).</summary>
        public static void Hire(IntrigueState state, Settlement town)
        {
            string blocked = HireBlocked(state, town);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(HouseRules.HireCost);
            House house = state.HouseIn(town.StringId);
            house.Staff++;
            MenuUI.Success("Bir çalışan daha katıldı: " + house.Staff + " kişi");
        }

        public static void SetShare(IntrigueState state, Settlement town, HouseShare share)
        {
            House house = state.HouseIn(town.StringId) ?? throw new InvalidOperationException("Burada genelevin yok");
            house.Share = share;
            MenuUI.Success("Çalışanların payı: " + HouseRules.ShareName(share));
        }

        public static void Close(IntrigueState state, Settlement town)
        {
            House house = state.HouseIn(town.StringId) ?? throw new InvalidOperationException("Burada genelevin yok");
            state.Houses.Remove(house);
            state.AddLog(SchemeFactors.Today, Tr.Locative(town.Name.ToString()) + " genelev kapatıldı");
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " genelev kapatıldı");
        }

        /// <summary>Ağ çöker ya da dağılırsa genelev de kapanır (çete işletiyordu).</summary>
        public static void OnNetworkLost(IntrigueState state, string townId) => state.Houses.RemoveAll(h => h.TownId == townId);

        /// <summary>Şehrin gölge ağı açıklamasına eklenen satır.</summary>
        public static string DescribeLine(IntrigueState state, Settlement town)
        {
            House house = state.HouseIn(town.StringId);
            if (house == null) return Enabled ? "Genelev: yok (" + SchemeFactors.Money(HouseRules.OpenCost) + " ile açılır)." : null;
            float prosperity = town.Town?.Prosperity ?? 0f;
            return "Genelev: " + house.Staff + "/" + MaxStaff(state, town) + " çalışan, payları " + HouseRules.ShareName(house.Share)
                + ", haftalık sana ~" + SchemeFactors.Money(HouseRules.WeeklyNet(house.Staff, prosperity, house.Share))
                + ", müşterilerden haber %" + HouseRules.IntelChance(house.Staff, house.Share)
                + (Enabled ? "" : " (Kurallar'dan kapalı: çalışmıyor)") + ".";
        }

        // ---- Haftalık ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.Houses.Count == 0 || !Enabled) return;
            int today = SchemeFactors.Today;
            int income = 0;
            foreach (House house in state.Houses.ToList())
            {
                Settlement town = Settlement.Find(house.TownId);
                if (town == null || state.Network(house.TownId) == null)
                {
                    state.Houses.Remove(house);
                    continue;
                }
                income += HouseRules.WeeklyNet(house.Staff, town.Town?.Prosperity ?? 0f, house.Share);
                Leavers(state, house, town);
                if (state.Roll100() <= HouseRules.ExposeChance)
                {
                    Exposed(state, house, town);
                    continue;
                }
                if (state.Roll100() <= HouseRules.IntelChance(house.Staff, house.Share)) Intel(state, town);
            }
            if (income > 0)
            {
                Hero.MainHero.ChangeHeroGold(income);
                state.AddLog(today, "Genelevler: haftalık +" + SchemeFactors.Money(income));
                MenuUI.Success("Genelevlerden haftalık gelir: " + SchemeFactors.Money(income));
            }
        }

        /// <summary>Düşük payda çalışanlar ayrılır (gönüllüler: kimse tutulamaz).</summary>
        private static void Leavers(IntrigueState state, House house, Settlement town)
        {
            int chance = HouseRules.LeaveChance(house.Share);
            if (chance <= 0 || house.Staff <= 0) return;
            int left = 0;
            for (int i = 0; i < house.Staff; i++)
                if (state.Roll100() <= chance) left++;
            if (left == 0) return;
            house.Staff = Math.Max(0, house.Staff - left);
            SchemeOutcomes.Notify(state, "Genelevden ayrılanlar: " + town.Name,
                left + " çalışan payını az bulup ayrıldı; kalan " + house.Staff + ". Pay artarsa kimse ayrılmaz.", null, CardImages.NightHouse);
        }

        /// <summary>Kendi şehrinde skandal; başka şehirde baskın (kapanır, şehir sahibiyle ilişki ve suç puanı).</summary>
        private static void Exposed(IntrigueState state, House house, Settlement town)
        {
            Clan owner = town.OwnerClan;
            if (owner == Clan.PlayerClan)
            {
                Kingdom kingdom = Clan.PlayerClan.Kingdom;
                List<Hero> honorable = kingdom == null ? new List<Hero>()
                    : kingdom.Clans.Where(c => c != Clan.PlayerClan && !c.IsEliminated && c.Leader != null && c.Leader.IsAlive
                            && c.Leader.GetTraitLevel(DefaultTraits.Honor) >= 1)
                        .Select(c => c.Leader).ToList();
                foreach (Hero leader in honorable) SchemeOutcomes.Relate(leader, -HouseRules.ScandalPenalty);
                SchemeOutcomes.Notify(state, "Dedikodu: " + town.Name,
                    Tr.Locative(town.Name.ToString()) + " genelevin sana ait olduğu duyuldu. Senin şehrin, senin kuralın; ev açık kalıyor."
                    + (honorable.Count > 0 ? "\n\nSonuç: onurlu " + honorable.Count + " klan lideriyle ilişki -" + HouseRules.ScandalPenalty + "." : ""),
                    null, CardImages.BrothelGossip);
                state.AddLog(SchemeFactors.Today, Tr.Locative(town.Name.ToString()) + " genelev dedikodusu");
                return;
            }
            state.Houses.Remove(house);
            var parts = new List<string> { "ev kapandı, çalışanlar dağıldı" };
            Hero lord = owner?.Leader;
            if (lord != null && lord.IsAlive)
            {
                SchemeOutcomes.Relate(lord, -HouseRules.RaidOwnerPenalty);
                parts.Add("şehrin sahibi " + lord.Name + " ile ilişki -" + HouseRules.RaidOwnerPenalty);
            }
            Kingdom theirs = owner?.Kingdom;
            if (theirs != null && theirs != Clan.PlayerClan.Kingdom && !FactionManager.IsAtWarAgainstFaction(theirs, Clan.PlayerClan.MapFaction))
            {
                ChangeCrimeRatingAction.Apply(theirs, HouseRules.RaidCrime, true);
                parts.Add(theirs.Name + " suç puanı +" + HouseRules.RaidCrime);
            }
            SchemeOutcomes.Notify(state, "Baskın: " + town.Name,
                "Muhafızlar " + Tr.Locative(town.Name.ToString()) + " geneleve baskın yaptı; izler sana çıktı.\n\nSonuç: " + string.Join(", ", parts) + ".",
                lord, CardImages.BrothelRaid);
            state.AddLog(SchemeFactors.Today, Tr.Locative(town.Name.ToString()) + " geneleve baskın");
        }

        /// <summary>Müşteriler konuştu: şehirdeki bir lord hakkında rapor, bazen bir sır.</summary>
        private static void Intel(IntrigueState state, Settlement town)
        {
            Hero target = Overhear(state, town, out string secretLine);
            if (target == null) return;
            int today = SchemeFactors.Today;
            SchemeOutcomes.Notify(state, "Genelevde duyulanlar: " + target.Name,
                Tr.Locative(town.Name.ToString()) + " genelevde " + target.Name + " hakkında konuşuldu; çalışanların duyduklarını sana iletti. "
                + "Hakkında güncel bir raporun var." + secretLine,
                target, CardImages.BrothelGossip);
            state.AddLog(today, "Genelev raporu: " + target.Name);
        }

        /// <summary>
        /// Genelevde konuşulanlar: şehirdeki bir yabancı lord hakkında güncel rapor yazılır, bazen bir sırrı da (secretLine dolar).
        /// Şehirde lord yoksa null. İşletmenin haftalık haberi ve müşteri olarak geçirilen gece bunu kullanır.
        /// </summary>
        internal static Hero Overhear(IntrigueState state, Settlement town, out string secretLine)
        {
            secretLine = "";
            List<Hero> lords = LordsIn(town);
            if (lords.Count == 0) return null;
            Hero target = lords[SchemeRandom.Pick(state.Seed, ref state.Rolls, lords.Count)];
            IntelReport report = SchemeOutcomes.CreateReport(target, SchemeFactors.Today);
            state.Reports.RemoveAll(r => r.TargetId == target.StringId);
            state.Reports.Add(report);
            if (state.Roll100() <= HouseRules.SecretChance)
            {
                Secret secret = SchemeOutcomes.FindSecret(state, target, state.Roll100, count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count));
                if (secret != null)
                {
                    RomanceSecrets.Record(state, secret);
                    secretLine = "\n\n" + SchemeOutcomes.SecretFoundLine(secret);
                }
            }
            return target;
        }

        /// <summary>Şehirdeki (partisiyle ya da partisiz) yabancı lordlar; sıralı (tohumlu seçim kayıttan bağımsız olsun).</summary>
        private static List<Hero> LordsIn(Settlement town) =>
            town.HeroesWithoutParty
                .Concat(town.Parties.Select(p => p.LeaderHero))
                .Where(h => h != null && h.IsAlive && h.IsLord && !h.IsChild && h != Hero.MainHero && h.Clan != Clan.PlayerClan)
                .Distinct()
                .OrderBy(h => h.StringId, StringComparer.Ordinal)
                .ToList();
    }
}

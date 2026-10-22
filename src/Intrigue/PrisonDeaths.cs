using System;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Esiri gizlice öldürtmek (tasarım: docs/superpowers/specs/2026-10-21-gizli-infaz.md): açık idam yerine hastalık, kaza ya da firar
    /// süsü. Uzaktaki zindanda da yapılır (gardiyanlar ayarlar). Şüphe ve ifşa olasılığı senin ya da casus başının Roguery'sine
    /// (hangisi yüksekse), esirin önemine ve yakalanalı kaç gün olduğuna bağlıdır. Kurallar: PrisonDeathRules.
    /// </summary>
    public static class PrisonDeaths
    {
        private static int Today => SchemeFactors.Today;

        public static int DaysHeld(Hero prisoner) =>
            Math.Max(0, (int)(CampaignTime.Now - prisoner.CaptivityStartTime).ToDays);

        private static KillCharacterAction.KillCharacterActionDetail Detail(PrisonDeathMethod method) =>
            method == PrisonDeathMethod.Illness ? KillCharacterAction.KillCharacterActionDetail.DiedOfOldAge
                : KillCharacterAction.KillCharacterActionDetail.WoundedInBattle;

        public static int Suspicion(IntrigueState state, Interrogation.Prisoner prisoner, PrisonDeathMethod method) =>
            PrisonDeathRules.Suspicion(method, Interrogation.Roguery(state), SchemeFactors.TargetTier(prisoner.Hero), prisoner.Hero.IsKingdomLeader,
                DaysHeld(prisoner.Hero), prisoner.Prison == null, SettingsStore.Current.IntrigueDifficulty);

        public static int Proof(IntrigueState state) => PrisonDeathRules.ProofChance(Interrogation.Roguery(state));

        /// <summary>null: bu yöntemle yapılabilir; değilse neden.</summary>
        public static string Blocked(IntrigueState state, Interrogation.Prisoner prisoner, PrisonDeathMethod method)
        {
            Hero hero = prisoner?.Hero;
            if (hero == null || !hero.IsAlive || !hero.IsPrisoner) return "artık esirin değil";
            if (hero.Clan == Clan.PlayerClan) return "kendi klanından biri";
            if (Hero.MainHero.IsPrisoner) return "esirken olmaz";
            if (!hero.CanDie(Detail(method))) return "bu kampanyada bu yolla ölüm kapalı (Doğum ve Ölüm ayarı)";
            return SchemeFactors.CanAfford(PrisonDeathRules.Cost(method)) ? null : "yeterli altının yok";
        }

        /// <summary>Kartta: "Kaza (1.000): şüphe %24, ifşa %8".</summary>
        public static string OptionLabel(IntrigueState state, Interrogation.Prisoner prisoner, PrisonDeathMethod method)
        {
            int suspicion = Suspicion(state, prisoner, method);
            return PrisonDeathRules.Name(method) + " (" + SchemeFactors.Money(PrisonDeathRules.Cost(method)) + "): şüphe %" + suspicion
                + ", ifşa %" + PrisonDeathRules.ExposeChance(suspicion, Proof(state));
        }

        /// <summary>Yapılır; sonuç metni (kart için) döner.</summary>
        public static string Execute(IntrigueState state, Interrogation.Prisoner prisoner, PrisonDeathMethod method)
        {
            string blocked = Blocked(state, prisoner, method);
            if (blocked != null) throw new InvalidOperationException(blocked);
            int suspicion = Suspicion(state, prisoner, method);
            int proof = Proof(state);
            SchemeFactors.Pay(PrisonDeathRules.Cost(method));
            bool hidden = CheatState.IntrigueNeverDiscovered;
            bool suspected = !hidden && state.Roll100() <= suspicion;
            bool proven = suspected && state.Roll100() <= proof;
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (spymaster != null && SchemeFactors.SpymasterBusyReason(spymaster) == null) spymaster.AddSkillXp(TaleWorlds.Core.DefaultSkills.Roguery, 300);
            return SchemeOutcomes.PrisonDeath(state, prisoner.Hero, method, suspected, proven);
        }
    }
}

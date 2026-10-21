using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Entrika sisteminin kampanya davranışı. Durum kayda tek bir metin olarak yazılır (kendi veri türü yok),
    /// böylece mod kaldırılsa bile kayıt açılır.
    /// </summary>
    public sealed class IntrigueBehavior : CampaignBehaviorBase
    {
        private const string SaveKey = "CheatMenu_Intrigue";

        public static IntrigueBehavior Current => Campaign.Current?.GetCampaignBehavior<IntrigueBehavior>();

        public IntrigueState State { get; private set; } = new IntrigueState();

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, starter =>
            {
                // Genel zar tohumu (yapay zekâ entrikaları, ifşa zarları): her kampanyada farklı, sonra kayıtla sabit.
                if (State.Seed == 0) State.Seed = MBRandom.RandomInt() | 1;
                ShadowNetworks.RegisterMenus(starter);
                CourtDays.RegisterMenus(starter);
                Tailing.RegisterMenus(starter);
                Brothels.RegisterMenus(starter);
                Treasury.ResetCache();
                IntrigueDialogs.Register(starter);
            });
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, () =>
            {
                MenuUI.SafeRun("Entrika günlük", SchemeEngine.DailyTick);
                MenuUI.SafeRun("Entrika günlük (yapay zekâ)", AiIntrigue.Daily);
                MenuUI.SafeRun("Entrika günlük (haberciler)", Messengers.DailyTick);
                MenuUI.SafeRun("Entrika günlük (kur ve gebelik)", Courtships.DailyTick);
                MenuUI.SafeRun("Entrika günlük (eşin)", SpouseLife.DailyTick);
                MenuUI.SafeRun("Aile editörü (yaşlanma)", Cheats.FamilyEditor.DailyTick);
                MenuUI.SafeRun("Divan günlük (kervan defteri)", Treasury.DailyTick);
                MenuUI.SafeRun("Divan günlük (mareşalin uyarıları)", Marshal.Daily);
                MenuUI.SafeRun("Divan günlük (kâhyanın uyarıları)", Steward.Daily);
                MenuUI.SafeRun("Genelev günlük (hastalık)", Brothels.Daily);
            });
            CampaignEvents.OnGivenBirthEvent.AddNonSerializedListener(this, (mother, children, stillborn) =>
                MenuUI.SafeRun("Hekim (doğum)", () => Physician.OnGivenBirth(mother)));
            CampaignEvents.OnChildConceivedEvent.AddNonSerializedListener(this, mother =>
                MenuUI.SafeRun("Hekim (gebelik)", () => Physician.OnChildConceived(mother)));
            CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, (party, settlement) =>
            {
                if (party != null && party.IsCaravan) MenuUI.SafeRun("Kervan defteri", () => Treasury.OnSettlementLeft(party, settlement));
            });
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, () =>
                MenuUI.SafeRun("Lordlar (çağrılanlar)", LordContacts.Hourly));
            CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, (oldPlayer, newPlayer, newParty, partyChanged) =>
                MenuUI.SafeRun("Varis", () => Dynasty.OnPlayerCharacterChanged(State, oldPlayer, newPlayer)));
            CampaignEvents.WarDeclared.AddNonSerializedListener(this, (a, b, detail) =>
                MenuUI.SafeRun("Rehine anlaşması (savaş)", () => Hostages.OnWarDeclared(a, b, detail)));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, () =>
            {
                MenuUI.SafeRun("Entrika haftalık (yapay zekâ)", AiIntrigue.Weekly);
                MenuUI.SafeRun("Entrika haftalık (gölge ağları)", ShadowNetworks.Weekly);
                MenuUI.SafeRun("Entrika haftalık (diplomatik kriz)", DiplomaticCrises.Weekly);
                MenuUI.SafeRun("Entrika haftalık (genelevler)", NightHouses.Weekly);
                MenuUI.SafeRun("Entrika haftalık (eşin)", SpouseLife.Weekly);
                MenuUI.SafeRun("Entrika haftalık (rehineler)", Hostages.Weekly);
                MenuUI.SafeRun("Entrika haftalık (casus başı görevi)", SpymasterMissions.Weekly);
                MenuUI.SafeRun("Barış Masası (elçiler)", PeaceTreaties.Weekly);
                MenuUI.SafeRun("Divan haftalık (hazinedar)", Treasury.Weekly);
                MenuUI.SafeRun("Divan haftalık (hekim)", Physician.Weekly);
                MenuUI.SafeRun("Divan haftalık (mareşal)", Marshal.Weekly);
                MenuUI.SafeRun("Divan haftalık (kâhya)", Steward.Weekly);
                MenuUI.SafeRun("Divan haftalık (diplomat)", Diplomat.Weekly);
            });
        }

        public override void SyncData(IDataStore dataStore)
        {
            string text = dataStore.IsSaving ? State.Write() : null;
            dataStore.SyncData(SaveKey, ref text);
            if (dataStore.IsLoading) State = IntrigueState.Parse(text);
        }
    }
}

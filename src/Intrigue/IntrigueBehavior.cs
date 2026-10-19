using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;

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
        }

        public override void SyncData(IDataStore dataStore)
        {
            string text = dataStore.IsSaving ? State.Write() : null;
            dataStore.SyncData(SaveKey, ref text);
            if (dataStore.IsLoading) State = IntrigueState.Parse(text);
        }
    }
}

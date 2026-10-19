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
            starter.AddModel<ClanTierModel>(new CheatClanTierModel());
            // Herkesi biç: savaşta öldürücü kılıç darbesi arkadakine geçer (ayar kapalıyken önceki modelin aynısı).
            starter.AddModel<TaleWorlds.MountAndBlade.ComponentInterfaces.AgentApplyDamageModel>(new CutThroughDamageModel());
        }
    }
}

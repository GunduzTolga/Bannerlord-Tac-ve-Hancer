using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Lord diyaloğuna eklenen satırlar: üzerinde kozun olan lordla "özel bir konu" açıp kozu kullanmak.
    /// Oyunun konuşma ağacına bağlanır ("hero_main_options" → bizim dallar → "lord_pretalk"). Harmony gerekmez.
    /// Aynı işlemler Entrika ekranı > Kişiler'den de yapılabilir.
    /// </summary>
    public static class IntrigueDialogs
    {
        private const string Options = "tvh_hook_options";
        private const string Done = "tvh_hook_done";

        public static void Register(CampaignGameStarter starter)
        {
            starter.AddPlayerLine("tvh_hook_start", "hero_main_options", "tvh_hook_reply",
                "Seninle özel bir konuyu konuşmamız gerek... Hakkında bazı şeyler biliyorum.", () => HookHere() != null, null, 110);
            starter.AddDialogLine("tvh_hook_reply", "tvh_hook_reply", Options, "(Yüzü bir an solar.) Ne istiyorsun?", null, null);

            foreach (HookActions.Use use in new[]
                     {
                         HookActions.Use.Gold, HookActions.Use.Influence, HookActions.Use.ReleasePrisoners,
                         HookActions.Use.DefectToMe, HookActions.Use.LeaveKingdom, HookActions.Use.Release,
                     })
            {
                HookActions.Use captured = use;
                starter.AddPlayerLine("tvh_hook_use_" + use, Options, Done, LineFor(use),
                    () => IsAvailable(captured),
                    () => MenuUI.SafeRun("Koz", () => HookActions.Apply(SchemeEngine.State, Hero.OneToOneConversationHero, captured)));
            }
            starter.AddPlayerLine("tvh_hook_cancel", Options, "lord_pretalk", "Şimdilik bir şey istemiyorum. Ama unutma, biliyorum.", null, null);
            starter.AddDialogLine("tvh_hook_done", Done, "lord_pretalk", "Peki... Ama bunu unutmayacağım.", null, null);
        }

        private static Hook HookHere()
        {
            Hero hero = Hero.OneToOneConversationHero;
            IntrigueState state = IntrigueBehavior.Current?.State;
            return hero == null || state == null ? null : state.ActiveHook(hero.StringId, SchemeFactors.Today);
        }

        private static bool IsAvailable(HookActions.Use use)
        {
            Hero hero = Hero.OneToOneConversationHero;
            if (hero == null || HookHere() == null) return false;
            return HookActions.Options(hero).Exists(o => o.Use == use && o.Blocked == null);
        }

        private static string LineFor(HookActions.Use use)
        {
            switch (use)
            {
                case HookActions.Use.Gold: return "Keseni biraz hafifletmeni istiyorum.";
                case HookActions.Use.Influence: return "Divandaki etkini benim için kullanacaksın.";
                case HookActions.Use.ReleasePrisoners: return "Elindeki esirlerimi serbest bırakacaksın.";
                case HookActions.Use.DefectToMe: return "Hükümdarını bırakıp bize katılacaksın.";
                case HookActions.Use.LeaveKingdom: return "Hükümdarına sırt çevireceksin.";
                default: return "Bu sefer seni affediyorum. Sırrın bende güvende.";
            }
        }
    }
}

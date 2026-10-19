using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    public static class CheatTargets
    {
        /// <summary>
        /// Bir savaş hilesinin bu ajana uygulanıp uygulanmayacağına karar verir.
        /// </summary>
        /// <param name="agent">Savaştaki ajan (insan veya at).</param>
        /// <param name="me">Hilenin [Ben] anahtarı açık mı.</param>
        /// <param name="army">Hilenin [Ordu] anahtarı açık mı.</param>
        public static bool IsInScope(Agent agent, bool me, bool army)
        {
            if (agent == null || !agent.IsActive()) return false;

            // Oyuncu yalnızca [Ben]'e, atlar binicilerine, diğerleri oyuncunun takımındaysa (ve seçiliyse
            // müttefik takımlardaysa) [Ordu]'ya tabi.
            if (agent.IsMainAgent) return me;
            if (agent.IsMount) return agent.RiderAgent != null && IsInScope(agent.RiderAgent, me, army);
            return army && agent.Team != null
                && (agent.Team.IsPlayerTeam || (CheatState.ArmyIncludesAllies && agent.Team.IsPlayerAlly));
        }
    }
}

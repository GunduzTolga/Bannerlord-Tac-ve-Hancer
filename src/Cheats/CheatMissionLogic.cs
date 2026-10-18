using System;
using System.Collections.Generic;
using CheatMenu.Menu;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    public sealed class CheatMissionLogic : MissionLogic
    {
        private const float RefreshInterval = 0.25f;

        private readonly HashSet<Agent> _madeInvulnerable = new HashSet<Agent>();
        private readonly List<KeyValuePair<Agent, Blow>> _pendingKills = new List<KeyValuePair<Agent, Blow>>();
        private float _timer;
        private bool _errorShown;

        // OnAgentHit hasar uygulandıktan sonra çağrılır ve Blow salt okunur (in) gelir; bu yüzden
        // kurban kuyruğa alınır ve bir sonraki tick'te Die(blow) ile öldürülür (kill/XP saldırana yazılır).
        // in parametreleri lambda'ya yakalanamadığı için burada düz try/catch kullanılıyor.
        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon,
            in Blow blow, in AttackCollisionData attackCollisionData)
        {
            if (!CheatState.OneHitMe && !CheatState.OneHitArmy) return;
            try
            {
                if (affectedAgent == null || affectorAgent == null || affectedAgent == affectorAgent) return;
                // Oyunun kendisinin Immortal yaptığı (hikâye/turnuva) ajanlara dokunma.
                if (!affectedAgent.IsActive() || affectedAgent.CurrentMortalityState != Agent.MortalityState.Mortal) return;
                if (!CheatTargets.IsInScope(affectorAgent, CheatState.OneHitMe, CheatState.OneHitArmy)) return;
                if (!affectorAgent.IsEnemyOf(affectedAgent)) return;
                _pendingKills.Add(new KeyValuePair<Agent, Blow>(affectedAgent, blow));
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        public override void OnMissionTick(float dt)
        {
            try
            {
                ProcessPendingKills();
                _timer += dt;
                if (_timer < RefreshInterval) return;
                _timer = 0f;
                RefreshGodMode();
                RefreshAmmo();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ProcessPendingKills()
        {
            if (_pendingKills.Count == 0) return;
            foreach (KeyValuePair<Agent, Blow> kill in _pendingKills)
            {
                Agent victim = kill.Key;
                if (victim.IsActive() && victim.CurrentMortalityState == Agent.MortalityState.Mortal)
                    victim.Die(kill.Value);
            }
            _pendingKills.Clear();
        }

        private void RefreshGodMode()
        {
            bool anyOn = CheatState.GodModeMe || CheatState.GodModeArmy;
            if (!anyOn && _madeInvulnerable.Count == 0) return;

            foreach (Agent agent in Mission.Agents)
            {
                bool wanted = anyOn && CheatTargets.IsInScope(agent, CheatState.GodModeMe, CheatState.GodModeArmy);
                // Yalnızca Mortal olanları değiştir ve kaydet; kapatınca sadece bunları geri al.
                if (wanted && agent.CurrentMortalityState == Agent.MortalityState.Mortal)
                {
                    agent.SetMortalityState(Agent.MortalityState.Invulnerable);
                    _madeInvulnerable.Add(agent);
                }
                else if (!wanted && _madeInvulnerable.Remove(agent))
                {
                    agent.SetMortalityState(Agent.MortalityState.Mortal);
                }
            }
            _madeInvulnerable.RemoveWhere(agent => !agent.IsActive());
        }

        private void RefreshAmmo()
        {
            if (!CheatState.InfiniteAmmoMe && !CheatState.InfiniteAmmoArmy) return;

            foreach (Agent agent in Mission.Agents)
            {
                if (!agent.IsHuman) continue;
                if (!CheatTargets.IsInScope(agent, CheatState.InfiniteAmmoMe, CheatState.InfiniteAmmoArmy)) continue;

                for (EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot; slot < EquipmentIndex.NumAllWeaponSlots; slot++)
                {
                    MissionWeapon weapon = agent.Equipment[slot];
                    if (weapon.IsEmpty || !weapon.IsAnyConsumable()) continue;
                    if (weapon.Amount < weapon.ModifiedMaxAmount)
                        agent.SetWeaponAmountInSlot(slot, weapon.ModifiedMaxAmount, false);
                }
            }
        }

        /// <summary>Her tick'te mesaj spam'i olmasın: ilk hata ekrana, hepsi loga.</summary>
        private void ReportError(Exception ex)
        {
            TaleWorlds.Library.Debug.Print("[CheatMenu] mission error: " + ex);
            if (_errorShown) return;
            _errorShown = true;
            MenuUI.Error("Savaş hilesinde hata: " + ex.Message);
        }
    }
}

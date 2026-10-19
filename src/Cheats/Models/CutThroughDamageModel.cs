using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// "Herkesi biç" (Kurallar): öldürücü kılıç darbesi düşmanın içinden geçip arkadakine devam eder (CutThroughRules).
    /// Oyunun kendi hasar modeli zincirlenir; yalnızca iki karar değişir: darbeden sonra kalan hız (CalculateRemainingMomentum)
    /// ve kılıcın tepkisi (DecideWeaponCollisionReaction: "içinden geç"). Geri kalan her şey önceki modele (Sandbox / NavalDLC /
    /// başka bir mod) aynen gider. Harmony yaması yok.
    /// </summary>
    public sealed class CutThroughDamageModel : AgentApplyDamageModel
    {
        private static bool ShouldCut(in AttackCollisionData collisionData, Agent attacker, Agent victim, in MissionWeapon attackerWeapon, bool isFatal)
        {
            int mode = SettingsStore.Current.CutThrough;
            if (mode == CutThroughRules.Off || attacker == null || victim == null) return false;
            return CutThroughRules.ShouldCut(mode, attacker.IsMainAgent, victim.IsEnemyOf(attacker), isFatal,
                collisionData.StrikeType == (int)StrikeType.Swing, !attackerWeapon.IsEmpty, collisionData.IsAlternativeAttack,
                collisionData.AttackBlockedWithShield || collisionData.CollidedWithShieldOnBack);
        }

        public override float CalculateRemainingMomentum(float originalMomentum, in Blow b, in AttackCollisionData collisionData, Agent attacker, Agent victim, in MissionWeapon attackerWeapon, bool isCrushThrough)
        {
            float remaining = BaseModel.CalculateRemainingMomentum(originalMomentum, in b, in collisionData, attacker, victim, in attackerWeapon, isCrushThrough);
            // Darbe kaydedilmeden önce de çağrılır: öldürücü sayılması için hasar kalan canı aşmalı ya da can bitmiş olmalı.
            bool fatal = victim != null && (victim.Health <= 0f || b.InflictedDamage >= victim.Health);
            return ShouldCut(in collisionData, attacker, victim, in attackerWeapon, fatal)
                ? CutThroughRules.RemainingMomentum(originalMomentum, remaining)
                : remaining;
        }

        public override void DecideWeaponCollisionReaction(in Blow registeredBlow, in AttackCollisionData collisionData, Agent attacker, Agent defender, in MissionWeapon attackerWeapon, bool isFatalHit, bool isShruggedOff, float momentumRemaining, out MeleeCollisionReaction colReaction)
        {
            BaseModel.DecideWeaponCollisionReaction(in registeredBlow, in collisionData, attacker, defender, in attackerWeapon, isFatalHit, isShruggedOff, momentumRemaining, out colReaction);
            // Balta gibi saplanan ya da sekme kararı, öldürücü kesişte "içinden geç"e döner (sersemletme kararına dokunulmaz).
            if ((colReaction == MeleeCollisionReaction.Stuck || colReaction == MeleeCollisionReaction.Bounced)
                && ShouldCut(in collisionData, attacker, defender, in attackerWeapon, isFatalHit))
                colReaction = MeleeCollisionReaction.SlicedThrough;
        }

        // ---- Geri kalanı önceki modele ----

        public override float ApplyDamageAmplifications(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage) =>
            BaseModel.ApplyDamageAmplifications(in attackInformation, in collisionData, baseDamage);

        public override float ApplyDamageReductions(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage) =>
            BaseModel.ApplyDamageReductions(in attackInformation, in collisionData, baseDamage);

        public override float ApplyDamageScaling(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage) =>
            BaseModel.ApplyDamageScaling(in attackInformation, in collisionData, baseDamage);

        public override float ApplyGeneralDamageModifiers(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage) =>
            BaseModel.ApplyGeneralDamageModifiers(in attackInformation, in collisionData, baseDamage);

        public override float CalculateAlternativeAttackDamage(in AttackInformation attackInformation, in AttackCollisionData collisionData, WeaponComponentData weapon) =>
            BaseModel.CalculateAlternativeAttackDamage(in attackInformation, in collisionData, weapon);

        public override void CalculateDefendedBlowStunMultipliers(Agent attackerAgent, Agent defenderAgent, CombatCollisionResult collisionResult, WeaponComponentData attackerWeapon, WeaponComponentData defenderWeapon, ref float attackerStunPeriod, ref float defenderStunPeriod) =>
            BaseModel.CalculateDefendedBlowStunMultipliers(attackerAgent, defenderAgent, collisionResult, attackerWeapon, defenderWeapon, ref attackerStunPeriod, ref defenderStunPeriod);

        public override float CalculateHullFireDamage(float baseFireDamage, IShipOrigin shipOrigin) =>
            BaseModel.CalculateHullFireDamage(baseFireDamage, shipOrigin);

        public override float CalculatePassiveAttackDamage(BasicCharacterObject attackerCharacter, in AttackCollisionData collisionData, float baseDamage) =>
            BaseModel.CalculatePassiveAttackDamage(attackerCharacter, in collisionData, baseDamage);

        public override float CalculateSailFireDamage(Agent attackerAgent, IShipOrigin shipOrigin, float baseDamage, bool damageFromShipMachine) =>
            BaseModel.CalculateSailFireDamage(attackerAgent, shipOrigin, baseDamage, damageFromShipMachine);

        public override float CalculateShieldDamage(in AttackInformation attackInformation, float baseDamage) =>
            BaseModel.CalculateShieldDamage(in attackInformation, baseDamage);

        public override float CalculateStaggerThresholdDamage(Agent defenderAgent, in Blow blow) =>
            BaseModel.CalculateStaggerThresholdDamage(defenderAgent, in blow);

        public override bool CanWeaponDealSneakAttack(in AttackInformation attackInformation, WeaponComponentData weapon) =>
            BaseModel.CanWeaponDealSneakAttack(in attackInformation, weapon);

        public override bool CanWeaponDismount(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData) =>
            BaseModel.CanWeaponDismount(attackerAgent, attackerWeapon, in blow, in collisionData);

        public override bool CanWeaponIgnoreFriendlyFireChecks(WeaponComponentData weapon) =>
            BaseModel.CanWeaponIgnoreFriendlyFireChecks(weapon);

        public override bool CanWeaponKnockback(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData) =>
            BaseModel.CanWeaponKnockback(attackerAgent, attackerWeapon, in blow, in collisionData);

        public override bool CanWeaponKnockDown(Agent attackerAgent, Agent victimAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData) =>
            BaseModel.CanWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, in blow, in collisionData);

        public override bool DecideAgentDismountedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow) =>
            BaseModel.DecideAgentDismountedByBlow(attackerAgent, victimAgent, in collisionData, attackerWeapon, in blow);

        public override bool DecideAgentKnockedBackByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow) =>
            BaseModel.DecideAgentKnockedBackByBlow(attackerAgent, victimAgent, in collisionData, attackerWeapon, in blow);

        public override bool DecideAgentKnockedDownByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow) =>
            BaseModel.DecideAgentKnockedDownByBlow(attackerAgent, victimAgent, in collisionData, attackerWeapon, in blow);

        public override bool DecideAgentShrugOffBlow(Agent victimAgent, in AttackCollisionData collisionData, in Blow blow) =>
            BaseModel.DecideAgentShrugOffBlow(victimAgent, in collisionData, in blow);

        public override bool DecideCrushedThrough(Agent attackerAgent, Agent defenderAgent, float totalAttackEnergy, Agent.UsageDirection attackDirection, StrikeType strikeType, WeaponComponentData defendItem, bool isPassiveUsageHit) =>
            BaseModel.DecideCrushedThrough(attackerAgent, defenderAgent, totalAttackEnergy, attackDirection, strikeType, defendItem, isPassiveUsageHit);

        public override void DecideMissileWeaponFlags(Agent attackerAgent, in MissionWeapon missileWeapon, ref WeaponFlags missileWeaponFlags) =>
            BaseModel.DecideMissileWeaponFlags(attackerAgent, in missileWeapon, ref missileWeaponFlags);

        public override bool DecideMountRearedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow) =>
            BaseModel.DecideMountRearedByBlow(attackerAgent, victimAgent, in collisionData, attackerWeapon, in blow);

        public override MeleeCollisionReaction DecidePassiveAttackCollisionReaction(Agent attacker, Agent defender, bool isFatalHit) =>
            BaseModel.DecidePassiveAttackCollisionReaction(attacker, defender, isFatalHit);

        public override float GetDamageMultiplierForBodyPart(BoneBodyPartType bodyPart, DamageTypes type, bool isHuman, bool isMissile) =>
            BaseModel.GetDamageMultiplierForBodyPart(bodyPart, type, isHuman, isMissile);

        public override float GetDismountPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData) =>
            BaseModel.GetDismountPenetration(attackerAgent, attackerWeapon, in blow, in collisionData);

        public override float GetHorseChargePenetration() =>
            BaseModel.GetHorseChargePenetration();

        public override float GetKnockBackPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData) =>
            BaseModel.GetKnockBackPenetration(attackerAgent, attackerWeapon, in blow, in collisionData);

        public override float GetKnockDownPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData) =>
            BaseModel.GetKnockDownPenetration(attackerAgent, attackerWeapon, in blow, in collisionData);

        public override bool IsDamageIgnored(in AttackInformation attackInformation, in AttackCollisionData collisionData) =>
            BaseModel.IsDamageIgnored(in attackInformation, in collisionData);

        public override bool ShouldMissilePassThroughAfterShieldBreak(Agent attackerAgent, WeaponComponentData attackerWeapon) =>
            BaseModel.ShouldMissilePassThroughAfterShieldBreak(attackerAgent, attackerWeapon);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : AbilityBehavior
    {
        public override void Use(AbilityUseParams abilityUseParams)
        {
            float damageToDeal = abilityUseParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage();
            abilityUseParams.target.TakeDamage(damageToDeal);
            PlayAbilitySound();
        }
    }
}
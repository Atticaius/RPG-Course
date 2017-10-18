using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            config = configToSet;
        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            float damageToDeal = abilityUseParams.baseDamage + config.GetExtraDamage();
            abilityUseParams.target.TakeDamage(damageToDeal);
        }
    }
}
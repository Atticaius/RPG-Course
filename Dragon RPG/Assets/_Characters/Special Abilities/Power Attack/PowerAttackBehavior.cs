using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : AbilityBehavior
    {
        public override void Use(GameObject target)
        {
            float damageToDeal = (config as PowerAttackConfig).GetAbilityDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
            PlayAbilityAnimation();
            PlayAbilitySound();
        }
    }
}
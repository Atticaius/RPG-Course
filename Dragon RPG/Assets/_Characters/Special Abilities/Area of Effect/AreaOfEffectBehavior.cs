using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaOfEffectBehavior : AbilityBehavior
    {

        public override void Use (AbilityUseParams abilityUseParams)
        {
            PlayAbilitySound();
            PlayParticleEffect();
            DealRadialDamage(abilityUseParams);
        }

        private void DealRadialDamage (AbilityUseParams abilityUseParams)
        {
            float damageToDeal = (config as AreaOfEffectConfig).GetExtraDamage() + abilityUseParams.baseDamage;
            float radius = (config as AreaOfEffectConfig).GetRadius();
            RaycastHit[] objectHits = Physics.SphereCastAll(transform.position, radius, Vector3.up, radius);
            foreach (RaycastHit objectHit in objectHits)
            {
                IDamageable damageable = objectHit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = objectHit.collider.gameObject.GetComponent<Player>();
                if (damageable != null && !hitPlayer)
                {
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
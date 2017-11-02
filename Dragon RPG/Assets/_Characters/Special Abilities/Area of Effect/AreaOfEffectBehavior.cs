using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaOfEffectBehavior : AbilityBehavior
    {

        public override void Use (GameObject target)
        {
            PlayAbilitySound();
            PlayAbilityAnimation();
            PlayParticleEffect();
            DealRadialDamage();
        }

        private void DealRadialDamage ()
        {
            float damageToDeal = (config as AreaOfEffectConfig).GetAbilityDamage();
            float radius = (config as AreaOfEffectConfig).GetRadius();
            RaycastHit[] objectHits = Physics.SphereCastAll(transform.position, radius, Vector3.up, radius);
            foreach (RaycastHit objectHit in objectHits)
            {
                HealthSystem healthSystem = objectHit.collider.gameObject.GetComponent<HealthSystem>();
                bool hitPlayer = objectHit.collider.gameObject.GetComponent<PlayerMovement>();
                if (healthSystem != null && !hitPlayer)
                {
                    healthSystem.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
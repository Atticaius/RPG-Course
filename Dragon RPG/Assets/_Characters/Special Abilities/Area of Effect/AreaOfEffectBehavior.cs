using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaOfEffectBehavior : MonoBehaviour, ISpecialAbility
    {
        AreaOfEffectConfig config;

        public void SetConfig(AreaOfEffectConfig configToSet)
        {
            config = configToSet;
        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            DealRadialDamage(abilityUseParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect ()
        {
            // Instantiate particle system attached to player
            GameObject newParticleSystem = Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity);
            // Get particle system component
            ParticleSystem myParticleSystem = newParticleSystem.GetComponent<ParticleSystem>();
            // Play particle system
            myParticleSystem.Play();
            // Destroy after duration
            Destroy(newParticleSystem, myParticleSystem.main.duration);
        }

        private void DealRadialDamage (AbilityUseParams abilityUseParams)
        {
            float damageToDeal = config.GetExtraDamage() + abilityUseParams.baseDamage;
            float radius = config.GetRadius();
            RaycastHit[] objectHits = Physics.SphereCastAll(transform.position, radius, Vector3.up, radius);
            foreach (RaycastHit objectHit in objectHits)
            {
                IDamageable damageable = objectHit.collider.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
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
            var particlePrefab = config.GetParticlePrefab();
            GameObject newParticleSystem = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
            ParticleSystem myParticleSystem = newParticleSystem.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
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
                bool hitPlayer = objectHit.collider.gameObject.GetComponent<Player>();
                if (damageable != null && !hitPlayer)
                {
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
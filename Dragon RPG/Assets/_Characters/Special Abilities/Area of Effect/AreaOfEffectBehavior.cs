using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

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
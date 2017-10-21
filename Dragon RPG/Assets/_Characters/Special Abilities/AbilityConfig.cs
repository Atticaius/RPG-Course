using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParams (IDamageable target, float baseDamage)
        {
            this.target = target;
            this.baseDamage = baseDamage;
        }
    }

    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip audioClip = null;

        public abstract AbilityBehavior GetBehaviorComponent(GameObject objectToAttachTo);

        protected AbilityBehavior behavior;

        public void AttachAbilityTo (GameObject objectToAttachTo)
        {
            AbilityBehavior behaviorComponent = GetBehaviorComponent(objectToAttachTo);
            behaviorComponent.SetConfig(this);
            behavior = behaviorComponent;
        }

        public void Use(AbilityUseParams abilityUseParams)
        {
            behavior.Use(abilityUseParams);
        }

        public float GetEnergyCost ()
        {
            return energyCost;
        }

        public GameObject GetParticlePrefab ()
        {
            return particlePrefab;
        }

        public AudioClip GetAudioClip ()
        {
            return audioClip;
        }
    }
}
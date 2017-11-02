using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] AnimationClip abilityAnimation;
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

        public void Use(GameObject target)
        {
            behavior.Use(target);
        }

        public float GetEnergyCost
        {
            get
            {
                return energyCost;
            }
        }

        public GameObject GetParticlePrefab
        {
            get
            {
                return particlePrefab;
            }
        }

        public AudioClip GetAudioClip
        {
            get
            {
                return audioClip;
            }
        }

        public AnimationClip GetAnimationClip
        {
            get
            {
                return abilityAnimation;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float maxAttackRange = 3f;
        [SerializeField] float timeBetweenAnimationCycles = .5f;
        [SerializeField] float additionalDamage = 10f;
        public Transform gripTransform;

        public float GetTimeBetweenAnimationCycles ()
        {
                return timeBetweenAnimationCycles;
        }

        public float GetAttackRange ()
        {
            return maxAttackRange;
        }

        public GameObject GetWeaponPrefab ()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAnimClip ()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        public float GetAdditionalDamage ()
        {
            return additionalDamage;
        }

        // Clears animation events so asset packs can't cause bugs
        void RemoveAnimationEvents ()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}
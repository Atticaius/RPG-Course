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
        public Transform gripTransform;
        [SerializeField] float attackRange = 3f;
        [SerializeField] float secondsBetweenHits = 3f;
        [SerializeField] float additionalDamage = 10f;

        public float GetSecondsBetweenHits ()
        {
                return secondsBetweenHits;
        }

        public float GetAttackRange ()
        {
            return attackRange;
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
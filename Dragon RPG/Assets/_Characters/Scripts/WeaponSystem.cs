using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        Character character;
        
        // Animation
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;
        const string DEFAULT_ATTACK = "Default Attack";
        const string ATTACK_TRIGGER = "Attack";

        // Attacking
        float lastHitTime;

        // Weapon Config
        [SerializeField] WeaponConfig currentWeaponConfig;
        public WeaponConfig GetCurrentWeapon ()
        {
            return currentWeaponConfig;
        }
        GameObject weaponObject;

        [SerializeField] float baseDamage = 10f;

        #region Setup
        void Start ()
        {
            character = GetComponent<Character>();
            animator = GetComponent<Animator>();
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        private void SetAttackAnimation ()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = character.GetOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip();
        }

        public void PutWeaponInHand (WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject);
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        private GameObject RequestDominantHand ()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player, please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominateHand scripts on player, please remove one.");
            return dominantHands[0].gameObject;
        }
        #endregion

        #region Attacking and Special Abilities
        public void AttackTarget (GameObject target)
        {
            if (target != null)
            {
                if (Time.time - lastHitTime > currentWeaponConfig.GetSecondsBetweenHits())
                {
                    SetAttackAnimation();
                    animator.SetTrigger(ATTACK_TRIGGER);
                    target.gameObject.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
                    lastHitTime = Time.time;
                }
            }
            else
            {
                CancelInvoke();
            }
        }

        float CalculateDamage ()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }
        #endregion
    }
}
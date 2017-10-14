using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        // Object references
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        CameraRaycaster cameraRaycaster;
        GameObject currentTarget;
        Animator animator;

        // Variables
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Weapon weaponInUse;
        [SerializeField] float attackDamage = 10f;

        float lastHitTime;
        float currentHealthPoints = 100f;
        public float HealthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        void Start ()
        {
            RegisterForMouseClicks();
            PutWeaponInHand();
            SetupAnimator();
            currentHealthPoints = maxHealthPoints;
        }

        private void SetupAnimator ()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAnimClip();
        }

        private void PutWeaponInHand ()
        {
            var weaponPrefab = weaponInUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            var weapon = Instantiate(weaponPrefab, dominantHand.transform);
            weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        private GameObject RequestDominantHand ()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player, please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominateHand scripts on player, please remove one.");
            return dominantHands[0].gameObject;
        }

        private void RegisterForMouseClicks ()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyMouseOverEnemy += OnMouseOverEnemy;
        }
   
        void OnMouseOverEnemy (Enemy enemy)
        {
            if (enemy != null)
            {
                currentTarget = enemy.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    InvokeRepeating("AttackTarget", 0, .1f);
                }
            }
        }

        void AttackTarget ()
        {
            if (currentTarget != null)
            {
                if (Time.time - lastHitTime > weaponInUse.GetSecondsBetweenHits() && IsInRange(currentTarget))
                {
                    animator.SetTrigger("Attack"); // TODO Make const
                    currentTarget.GetComponent<Enemy>().TakeDamage(attackDamage);
                    lastHitTime = Time.time;
                }
            } else
            {
                CancelInvoke();
            }
        }

        private bool IsInRange (GameObject target)
        {
            bool isInRange = Vector3.Distance(transform.position, target.transform.position) <= weaponInUse.GetAttackRange();
            return isInRange;
        }

        public void TakeDamage (float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
        }
    }
}
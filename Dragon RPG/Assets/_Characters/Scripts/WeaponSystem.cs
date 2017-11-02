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
        GameObject target;
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

        #region Attacking
        private void Update ()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;
            if (target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            } else
            {
                float targetHealth = target.GetComponent<HealthSystem>().HealthAsPercentage;
                targetIsDead = targetHealth <= Mathf.Epsilon;

                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().HealthAsPercentage;
            bool characterIsDead = characterHealth <= Mathf.Epsilon;

            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }
        }
        public void AttackTarget (GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        IEnumerator AttackTargetRepeatedly ()
        {
            bool attackerStillAlive = GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().HealthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && targetStillAlive)
            {
                float weaponHitPeriod = currentWeaponConfig.GetTimeBetweenAnimationCycles();
                float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier;
                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
        }

        void AttackTargetOnce ()
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.GetAttackRange())
            {
                transform.LookAt(target.transform);
                animator.SetTrigger(ATTACK_TRIGGER);
                float damageDelay = currentWeaponConfig.GetTimeBetweenAnimationCycles();
                StartCoroutine(DamageAfterDelay(damageDelay));
            } else
            {
                StopAttacking();
            }
        }

        public void StopAttacking ()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        IEnumerator DamageAfterDelay (float delaySeconds)
        {
            yield return new WaitForSecondsRealtime(delaySeconds);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        float CalculateDamage ()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }
        #endregion
    }
}
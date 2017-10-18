using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        // Object references
        GameObject currentTarget;

        // Animator
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        const string ATTACK_TRIGGER = "Attack";
        const string DEATH_TRIGGER = "Death";
        Animator animator;

        // Weapons
        [Header("Weapon")]
        [SerializeField] Weapon weaponInUse;
        [SerializeField] float baseDamage = 10f;
        float lastHitTime;

        // Abilities
        [Header("")]
        [SerializeField] SpecialAbility[] abilities;
        float abilityRange = 2f;

        // Sounds
        [Header("Sounds")]
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        AudioSource audioSource;

        // Health
        [Header("Health")]
        [SerializeField] float maxHealthPoints = 100f;
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
            Setup();
            abilities[0].AttachComponentTo(gameObject);
        }

    #region Setup
        private void Setup ()
        {
            RegisterForMouseClicks();
            PutWeaponInHand();
            SetupAnimator();
            audioSource = GetComponent<AudioSource>();
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

        private void SetHealth ()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void RegisterForMouseClicks ()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyMouseOverEnemy += OnMouseOverEnemy;
        }
        #endregion

    #region Range Checks
        private bool IsInWeaponRange (GameObject target)
        {
            bool isInRange = Vector3.Distance(transform.position, target.transform.position) <= weaponInUse.GetAttackRange();
            return isInRange;
        }

        private bool IsInAbilityRange (GameObject target)
        {
            bool isinRange = Vector3.Distance(transform.position, target.transform.position) <= abilityRange;
            return isinRange;
        }
    #endregion

    #region Attacking and Special Abilities
        void OnMouseOverEnemy (Enemy enemy)
        {
            if (enemy != null)
            {
                currentTarget = enemy.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    InvokeRepeating("AttackTarget", 0, .1f);
                } else if (Input.GetMouseButtonDown(1))
                {
                    AttemptSpecialAbility(enemy, 0);
                }
            }
        }

        void AttemptSpecialAbility (Enemy enemy, int abilityIndex)
        {
            Energy energy = GetComponent<Energy>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if (IsInAbilityRange(currentTarget) && energy.IsEnergyAvailable(energyCost))
            {
                energy.UseEnergy(energyCost);
                var abilityParams = new AbilityUseParams(enemy, baseDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
        }

        void AttackTarget ()
        {
            if (currentTarget != null)
            {
                if (Time.time - lastHitTime > weaponInUse.GetSecondsBetweenHits() && IsInWeaponRange(currentTarget))
                {
                    animator.SetTrigger(ATTACK_TRIGGER); // TODO Make const
                    currentTarget.GetComponent<Enemy>().TakeDamage(baseDamage);
                    lastHitTime = Time.time;
                }
            } else
            {
                CancelInvoke();
            }
        }
#endregion

    #region Taking Damage
        public void TakeDamage (float damage)
        {
            bool playerDies = currentHealthPoints - damage <= 0;
            ReduceHealth(damage);
            if (playerDies)
            {
                StartCoroutine(KillPlayer());
            } else
            {
                PlayDamageSound();
            }
        }

        void ReduceHealth (float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
        }

        void PlayDamageSound ()
        {
            audioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.Play();
        }

        void PlayDeathSound ()
        {
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();
        }

        IEnumerator KillPlayer()
        {
            animator.SetTrigger(DEATH_TRIGGER);
            PlayDeathSound();
            yield return new WaitForSecondsRealtime(audioSource.clip.length);
            SceneManager.LoadScene(0);
        }
#endregion
    }
}
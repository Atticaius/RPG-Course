using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using RPG.CameraUI;
using RPG.Core;

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
        const string DEFAULT_ATTACK = "Default Attack";
        Animator animator;

        // Weapons
        [Header("Weapon")]
        [SerializeField]
        Weapon currentWeaponConfig;
        [SerializeField] ParticleSystem criticalHitParticles;
        [SerializeField] float baseDamage = 10f;
        [Range(.1f, 1f)] [SerializeField] float critChance = .1f;
        [SerializeField] float critMultiplier = 1.5f;
        float lastHitTime;
        GameObject weaponObject;

        // Abilities
        [Header("")]
        [SerializeField]
        AbilityConfig[] abilities;
        float abilityRange = 2f;

        // Sounds
        [Header("Sounds")]
        [SerializeField]
        AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        AudioSource audioSource;

        // Health
        [Header("Health")]
        [SerializeField]
        public float maxHealthPoints = 100f;
        public float currentHealthPoints = 100f;
        public float HealthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }
        public float GetHealthDifference
        {
            get
            {
                return maxHealthPoints - currentHealthPoints;
            }
        }

        void Start ()
        {
            Setup();
        }

        void LateUpdate ()
        {
            if (HealthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        void ScanForAbilityKeyDown ()
        {
            for (int keyIndex = 1; keyIndex < abilities.Length; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    AttemptSpecialAbility(keyIndex);
                }
            }
        }

        public void PutWeaponInHand (Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject);
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        #region Setup
        private void Setup ()
        {
            audioSource = GetComponent<AudioSource>();
            AttachAbilities();
            RegisterForMouseClicks();
            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimator();
            SetHealth();
        }

        void AttachAbilities ()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }
        private void SetAttackAnimator ()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip();
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
            bool isInRange = Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.GetAttackRange();
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
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    AttemptSpecialAbility(0);
                }
            }
        }

        void AttemptSpecialAbility (int abilityIndex)
        {
            Energy energy = GetComponent<Energy>();
            if (currentTarget == null)
            {
                currentTarget = gameObject;
            }

            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (IsInAbilityRange(currentTarget.gameObject) && energy.IsEnergyAvailable(energyCost))
            {
                energy.UseEnergy(energyCost);
                var abilityParams = new AbilityUseParams(currentTarget.GetComponent<IDamageable>(), baseDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
        }

        void AttackTarget ()
        {
            if (currentTarget != null)
            {
                if (Time.time - lastHitTime > currentWeaponConfig.GetSecondsBetweenHits() && IsInWeaponRange(currentTarget.gameObject))
                {
                    SetAttackAnimator();
                    animator.SetTrigger(ATTACK_TRIGGER);
                    currentTarget.gameObject.GetComponent<IDamageable>().TakeDamage(CalculateDamage());
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
            float damageBeforeCrit = baseDamage + currentWeaponConfig.GetAdditionalDamage();
            bool isCrit = UnityEngine.Random.Range(0, 1f) < critChance;
            if (isCrit)
            {
                criticalHitParticles.Play();
                return damageBeforeCrit * critMultiplier;
            }
            else
            {
                return damageBeforeCrit;
            }
        }
        #endregion

        #region Taking Damage
        public void TakeDamage (float damage)
        {
            bool playerDies = currentHealthPoints - damage <= 0;
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
            if (playerDies)
            {
                StartCoroutine(KillPlayer());
            }
            else
            {
                audioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
                audioSource.Play();
            }
        }

        public void Heal (float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0, maxHealthPoints);
        }

        IEnumerator KillPlayer ()
        {
            animator.SetTrigger(DEATH_TRIGGER);
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();
            yield return new WaitForSecondsRealtime(audioSource.clip.length);
            SceneManager.LoadScene(0);
        }
        #endregion
    }
}
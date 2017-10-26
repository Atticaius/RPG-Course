using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        // Object references
        public GameObject currentTarget;
        SpecialAbilities specialAbilities;
        Character character;
        Camera mainCamera;

        // Animator
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        const string ATTACK_TRIGGER = "Attack";
        const string DEATH_TRIGGER = "Death";
        const string DEFAULT_ATTACK = "Default Attack";
        Animator animator;

        // Weapons
        [Header("Weapon")]
        [SerializeField] Weapon currentWeaponConfig;
        [SerializeField] ParticleSystem criticalHitParticles;
        [SerializeField] float baseDamage = 10f;
        [Range(.1f, 1f)] [SerializeField] float critChance = .1f;
        [SerializeField] float critMultiplier = 1.5f;
        float lastHitTime;
        GameObject weaponObject;

        // Abilities
        [Header("")]
        float abilityRange = 2f;

        // Sounds
        [Header("Sounds")]
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        // Movement
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;
        float turnAmount;
        float forwardAmount;

        #region Setup
        void Start ()
        {
            character = GetComponent<Character>();
            specialAbilities = GetComponent<SpecialAbilities>();

            // Camera
            mainCamera = Camera.main;
            RegisterForMouseEvents();
            PutWeaponInHand(currentWeaponConfig); // TODO Move to Weapon System
            SetAttackAnimator(); // TODO Move to Weapon System
        }

        private void SetAttackAnimator ()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip();
        }

        // TODO Move to Weapon System
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

        private GameObject RequestDominantHand ()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player, please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominateHand scripts on player, please remove one.");
            return dominantHands[0].gameObject;
        }

        private void RegisterForMouseEvents ()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.notifyMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }
        #endregion

        #region Ability Key Presses
        void LateUpdate ()
        {
                ScanForAbilityKeyDown();
        }

        void ScanForAbilityKeyDown ()
        {
            for (int keyIndex = 1; keyIndex < specialAbilities.abilityLength; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    specialAbilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }
        #endregion

        #region Handle Movement
        public void ProcessDirectMovement ()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            var camForward = Vector3.Scale(mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            var move = v * camForward + h * mainCamera.transform.right;

            Move(move);
        }

        public void Move (Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        private void SetForwardAndTurn (Vector3 movement)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }

            Vector3 localMovement = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMovement.x, localMovement.z);
            forwardAmount = localMovement.z;
        }

        void UpdateAnimator ()
        {
            // update the animator parameters
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        }

        void ApplyExtraTurnRotation ()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
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

        #region Mouse Events
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
                    specialAbilities.AttemptSpecialAbility(0, currentTarget);
                }
            }
        }

        void OnMouseOverPotentiallyWalkable (Vector3 destination)
        {
            if (Input.GetMouseButtonDown(0) && !character.isInDirectMovement)
            {
                character.SetDestination(destination); 
            }
        }
        #endregion

        #region Attacking and Special Abilities
        void AttackTarget ()
        {
            if (currentTarget != null)
            {
                if (Time.time - lastHitTime > currentWeaponConfig.GetSecondsBetweenHits() && IsInWeaponRange(currentTarget.gameObject))
                {
                    SetAttackAnimator();
                    animator.SetTrigger(ATTACK_TRIGGER);
                    currentTarget.gameObject.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
                    lastHitTime = Time.time;
                }
            }
            else
            {
                CancelInvoke();
            }
        }

        // TODO Move to Weapon System
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
    }
}
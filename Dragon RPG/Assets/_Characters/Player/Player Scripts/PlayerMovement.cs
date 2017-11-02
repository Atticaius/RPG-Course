using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;
using System;
using System.Collections;

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        // Abilities
        SpecialAbilities specialAbilities;
        float abilityRange = 2f;

        // Camera
        Camera mainCamera;

        // Character
        Character character;
        WeaponSystem weaponSystem;

        // Delegate
        public delegate void ChangedControlMode (String text);
        public event ChangedControlMode ControlModeDelegate;

        // Mouse Events
        GameObject currentTarget;

        // UI
        string controlMode;

        #region Setup
        void Start ()
        {
            character = GetComponent<Character>();
            specialAbilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            // UI
            CheckControlMode();
            ControlModeDelegate(controlMode);

            // Camera
            mainCamera = Camera.main;
            RegisterForMouseEvents();
        }

        private void RegisterForMouseEvents ()
        {
            CameraRaycaster cameraRaycaster = mainCamera.GetComponent<CameraRaycaster>();
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

            character.Move(move);
        }
        #endregion

        #region Mouse Events
        void OnMouseOverPotentiallyWalkable (Vector3 destination)
        {
            if (Input.GetMouseButtonDown(0) && !character.isInDirectMovement)
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
            }
        }

        void OnMouseOverEnemy (EnemyAI enemy)
        {
            if (enemy != null)
            {
                currentTarget = enemy.gameObject;
                if (Input.GetMouseButtonDown(0) && IsInWeaponRange(currentTarget))
                {
                    weaponSystem.AttackTarget(currentTarget);
                } else if (Input.GetMouseButton(0) && !IsInWeaponRange(currentTarget))
                {
                    StartCoroutine(MoveAndAttack(currentTarget));
                } else if (Input.GetMouseButtonDown(1) && IsInAbilityRange(currentTarget))
                {
                    specialAbilities.AttemptSpecialAbility(0, currentTarget);
                }
                else if (Input.GetMouseButtonDown(1) && !IsInAbilityRange(currentTarget))
                {
                    StartCoroutine(MoveAndPowerAttack(currentTarget));
                }
            }
        }

        IEnumerator MoveToTarget (GameObject target)
        {
            if (!character.isInDirectMovement)
            {
                character.SetDestination(target.transform.position);
            }
            
            while (!IsInWeaponRange(target))
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack (GameObject target)
        {
            yield return StartCoroutine(MoveToTarget(target));
            weaponSystem.AttackTarget(target);
        }

        IEnumerator MoveAndPowerAttack (GameObject target)
        {
            yield return StartCoroutine(MoveToTarget(target));
            specialAbilities.AttemptSpecialAbility(0, target);
        }
        #endregion

        #region Range Checks
        private bool IsInWeaponRange (GameObject target)
        {
            bool isInRange = Vector3.Distance(transform.position, target.transform.position) <= weaponSystem.GetCurrentWeapon().GetAttackRange();
            return isInRange;
        }

        private bool IsInAbilityRange (GameObject target)
        {
            bool isinRange = Vector3.Distance(transform.position, target.transform.position) <= abilityRange;
            return isinRange;
        }
        #endregion

        #region Update Control Mode
        private void FixedUpdate ()
        {
                CheckControlMode();
        }

        public void CheckControlMode ()
        {
            // Return state of the control mode
            if (character.isInDirectMovement)
            {
                ProcessDirectMovement();
                controlMode = "Keyboard";
            }
            else
            {
                controlMode = "Mouse";
            }
            ControlModeDelegate(controlMode);
        }
        #endregion
    }
}
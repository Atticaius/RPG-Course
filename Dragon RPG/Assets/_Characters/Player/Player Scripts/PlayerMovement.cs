using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerMovement : MonoBehaviour
    {
        // Object References
        AICharacterControl aiCharacterControl;
        ThirdPersonCharacter thirdPersonCharacter;
        CameraRaycaster cameraRaycaster;
        Camera mainCamera;
        NavMeshAgent navMeshAgent;
        GameObject walkTarget;

        // Variables
        public String controlMode;
        Vector3 currentClickTarget;
        bool isInDirectMovement = false;

        // Delegate
        public delegate void ChangedControlMode (String text);
        public event ChangedControlMode ControlModeDelegate;

        private void Start ()
        {
            // Object references
            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            mainCamera = Camera.main;
            walkTarget = new GameObject("Walk Target");

            // Delegate controls
            cameraRaycaster.notifyMouseOverPotentiallyWalkable += OnMouseOverWalkable;
            cameraRaycaster.notifyMouseOverEnemy += OnMouseOverEnemy;
            CheckControlMode();
            ControlModeDelegate(controlMode);
        }

        private void LateUpdate ()
        {
            if (Input.GetKeyUp(KeyCode.G))
            {
                // Enable/disable automatic controls
                isInDirectMovement = !isInDirectMovement;
                aiCharacterControl.enabled = !aiCharacterControl.enabled;
                navMeshAgent.enabled = !navMeshAgent.enabled;
                aiCharacterControl.SetTarget(null);

                // Check control mode and send to delegates
                CheckControlMode();
                ControlModeDelegate(controlMode);
            }
        }

        private void FixedUpdate ()
        {
            CheckControlMode();
        }

        private void CheckControlMode ()
        {
            // Return state of the control mode
            if (isInDirectMovement)
            {
                ProcessDirectMovement();
                controlMode = "Keyboard";
            }
            else
            {
                controlMode = "Mouse";
            }
        }

        void ProcessDirectMovement ()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            var camForward = Vector3.Scale(mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
            var move = v * camForward + h * mainCamera.transform.right;

            thirdPersonCharacter.Move(move);
        }

        void OnMouseOverWalkable (Vector3 destination)
        {
            if (Input.GetMouseButton(0) && !isInDirectMovement)
            {
                walkTarget.transform.position = destination;
                aiCharacterControl.SetTarget(walkTarget.transform);
            }
        }

        void OnMouseOverEnemy (Enemy enemy)
        {
            if (!isInDirectMovement)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    aiCharacterControl.SetTarget(enemy.transform);
                }
            }
            
        }
    }
}


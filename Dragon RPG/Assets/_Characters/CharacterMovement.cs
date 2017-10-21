using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class CharacterMovement : MonoBehaviour
    {
        // Object References
        ThirdPersonCharacter character;
        Camera mainCamera;
        NavMeshAgent agent;

        // Variables
        [SerializeField] float stoppingDistance = 1.5f;
        public String controlMode;
        Vector3 currentClickTarget;
        bool isInDirectMovement = false;

        // Delegate
        public delegate void ChangedControlMode (String text);
        public event ChangedControlMode ControlModeDelegate;

        private void Start ()
        {
            // Object references
            character = GetComponent<ThirdPersonCharacter>();

            // Camera
            mainCamera = Camera.main;
            CameraRaycaster cameraRaycaster = mainCamera.GetComponent<CameraRaycaster>();

            // NavMeshAgent
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updatePosition = true;
            agent.stoppingDistance = stoppingDistance;

            // Delegate controls
            cameraRaycaster.notifyMouseOverPotentiallyWalkable += OnMouseOverWalkable;
            cameraRaycaster.notifyMouseOverEnemy += OnMouseOverEnemy;
            CheckControlMode();
            ControlModeDelegate(controlMode);
        }

        private void Update ()
        {
            if (!isInDirectMovement)
            {
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    character.Move(agent.desiredVelocity);
                }
                else
                {
                    character.Move(Vector3.zero);
                }
            }
        }

        private void LateUpdate ()
        {
            HandleKeyPresses();
        }

        private void HandleKeyPresses ()
        {
            if (Input.GetKeyUp(KeyCode.G))
            {
                // Enable/disable automatic controls
                isInDirectMovement = !isInDirectMovement;
                agent.enabled = !agent.enabled;

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

            character.Move(move);
        }

        void OnMouseOverWalkable (Vector3 destination)
        {
            if (Input.GetMouseButton(0) && !isInDirectMovement)
            {
                agent.SetDestination(destination);
            }
        }

        void OnMouseOverEnemy (Enemy enemy)
        {
            if (!isInDirectMovement)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    agent.SetDestination(enemy.transform.position);
                }
            }
        }
    }
}


using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;
        Animator animator;

        [Header("Audio")]
        [Range(0, 1f)] [SerializeField] float spatialBlend = .5f;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 capsuleCenter = new Vector3(0, 0.9620609f, 0);
        [SerializeField] float capsuleRadius = .3f;
        [SerializeField] float capsuleHeight = 1.957076f;

        [Header("Movement")]
        [SerializeField] float navMeshStoppingDistance = 1.5f;
        [SerializeField] float moveSpeedMultiplier;
        
        
        PlayerMovement playerMovement;
        Rigidbody myRigidbody;
        NavMeshAgent navMeshAgent;
        
        string controlMode;
        [HideInInspector] public bool isInDirectMovement = false;
        bool isAlive = true;

        // Camera
        Camera mainCamera;
        CameraRaycaster cameraRaycaster;

        // Delegate
        public delegate void ChangedControlMode (String text);
        public event ChangedControlMode ControlModeDelegate;

        #region Setup
        void Awake ()
        {
            AddRequiredComponents();
        }

        void AddRequiredComponents ()
        {
            // Animator
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;
            animator.applyRootMotion = true;

            // AudioSource
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = spatialBlend;

            

            // Capsule Collider
            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = capsuleCenter;
            capsuleCollider.radius = capsuleRadius;
            capsuleCollider.height = capsuleHeight;

            // NavMesh Agent
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.stoppingDistance = navMeshStoppingDistance;

            // RigidBody
            myRigidbody = gameObject.AddComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            myRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        private void Start ()
        {
            // Delegate controls
            //cameraRaycaster.notifyMouseOverPotentiallyWalkable += OnMouseOverWalkable;
            //cameraRaycaster.notifyMouseOverEnemy += OnMouseOverEnemy;
            playerMovement = GetComponent<PlayerMovement>();
            CheckControlMode();
            ControlModeDelegate(controlMode);
        }
        #endregion

        #region KeyPresses
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
                navMeshAgent.enabled = !navMeshAgent.enabled;

                // Check control mode and send to delegates
                CheckControlMode();
                ControlModeDelegate(controlMode);
            }
        }
        #endregion

        #region Update Control Mode
        private void FixedUpdate ()
        {
            CheckControlMode();
        }

        private void CheckControlMode ()
        {
            // Return state of the control mode
            if (isInDirectMovement)
            {
                playerMovement.ProcessDirectMovement();
                controlMode = "Keyboard";
            }
            else
            {
                controlMode = "Mouse";
            }
        }
        #endregion

        #region Handle Animation and Movement
        private void Update ()
        {
            MoveCharacter();
        }

        private void MoveCharacter ()
        {
            if (!isInDirectMovement)
            {
                if (isAlive && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    playerMovement.Move(navMeshAgent.desiredVelocity);
                }
                else
                {
                    playerMovement.Move(Vector3.zero);
                }
            }
        }

        public void OnAnimatorMove ()
        {
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        

        public void Kill ()
        {
            isAlive = false;
        }

        
        #endregion
    }
}


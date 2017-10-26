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
        [SerializeField] float moveThreshold = 1f;
        public AnimatorOverrideController GetOverrideController
        {
            get
            {
                return animatorOverrideController;
            }
        }
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
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        float turnAmount;
        float forwardAmount;
        PlayerMovement playerMovement;
        Rigidbody myRigidbody;
        NavMeshAgent navMeshAgent;
        
        
        [HideInInspector] public bool isInDirectMovement = false;
        bool isAlive = true;

        // Camera
        Camera mainCamera;
        CameraRaycaster cameraRaycaster;

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
            playerMovement = GetComponent<PlayerMovement>();
        }
        #endregion
        
        #region Handle Animation and Movement
        private void Update ()
        {
            if (playerMovement)
            {
                HandleKeyPresses();
            }
                MoveCharacter();
            
        }

        public void Move (Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        private void MoveCharacter ()
        {
            if (!isInDirectMovement)
            {
                if (isAlive && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    Move(navMeshAgent.desiredVelocity);
                }
                else
                {
                    Move(Vector3.zero);
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

        public void SetDestination (Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
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
            Animator animator = GetComponent<Animator>();
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

        private void HandleKeyPresses ()
        {
            if (Input.GetKeyUp(KeyCode.G))
            {
                // Enable/disable automatic controls
                isInDirectMovement = !isInDirectMovement;
                navMeshAgent.enabled = !navMeshAgent.enabled;
                playerMovement.CheckControlMode();
            }
        }

        public void Kill ()
        {
            isAlive = false;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        // Object References
        AICharacterControl aiCharacterControl;
        GameObject player;

        // Variables
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float followRadius = 6f;
        [SerializeField] float attackRadius = 3f;
        float currentHealthPoints = 100f;
        bool isAttacking;

        // Projectiles
        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;
        [SerializeField] float damagePerShot = 8f;
        [SerializeField] float secondsBetweenShots = 1f;
        [SerializeField] Vector3 verticalAimOffset = new Vector3(0, 1f, 0);
        Projectile projectileSpawned;

        // Getters
        public float HealthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }
        public float GetHealth
        {
            get
            {
                return currentHealthPoints;
            }
        }

        private void Start ()
        {
            aiCharacterControl = GetComponent<AICharacterControl>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update ()
        {
            CheckTargetRanges();
        }

        void CheckTargetRanges ()
        {
            if (IsInChaseRange(player))
            {
                ChaseTarget(player);
            }
            else
            {
                aiCharacterControl.SetTarget(transform);
            }

            if (IsInAttackRange(player))
            {
                AttackTarget(player);
            }
        }

        void ChaseTarget (GameObject target)
        {
            aiCharacterControl.SetTarget(player.transform);
        }

        void AttackTarget (GameObject target)
        {
            if (!isAttacking)
            {
                InvokeRepeating("SpawnProjectile", 0, secondsBetweenShots);
                isAttacking = true;
            }
            else if (!IsInAttackRange(target))
            {
                CancelInvoke();
                isAttacking = false;
            }
        }

        bool IsInChaseRange (GameObject target)
        {
            return (Vector3.Distance(transform.position, target.transform.position) <= followRadius);
        }

        bool IsInAttackRange (GameObject target)
        {
            return Vector3.Distance(transform.position, player.transform.position) <= attackRadius;
        }

        void SpawnProjectile ()
        {
            if (IsInAttackRange(player))
            {
                projectileSpawned = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity).GetComponent<Projectile>();
                Vector3 unitVectorToPlayer = (player.transform.position + verticalAimOffset - projectileSpawned.transform.position).normalized;
                projectileSpawned.FireProjectile(gameObject, unitVectorToPlayer, damagePerShot);
            }
        }

        public void TakeDamage (float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
            if (currentHealthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmos ()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, followRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }
}
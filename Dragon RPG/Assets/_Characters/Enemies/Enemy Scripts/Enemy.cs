using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        // Object References
        AICharacterControl aiCharacterControl;
        Player player;

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
            player = FindObjectOfType<Player>();
        }

        public void TakeDamage (float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
            if (currentHealthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }



        private void Update ()
        {
            // Follow player
            if (Vector3.Distance(transform.position, player.transform.position) <= followRadius)
            {
                aiCharacterControl.SetTarget(player.transform);
            }
            else
            {
                aiCharacterControl.SetTarget(transform);
            }

            // Attack player
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius && !isAttacking)
            {
                InvokeRepeating("SpawnProjectile", 0, secondsBetweenShots);
                isAttacking = true;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) > attackRadius)
            {
                CancelInvoke();
                isAttacking = false;
            }
        }

        void SpawnProjectile ()
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius)
            {
                projectileSpawned = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity).GetComponent<Projectile>();
                Vector3 unitVectorToPlayer = (player.transform.position + verticalAimOffset - projectileSpawned.transform.position).normalized;
                projectileSpawned.FireProjectile(gameObject, unitVectorToPlayer, damagePerShot);
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
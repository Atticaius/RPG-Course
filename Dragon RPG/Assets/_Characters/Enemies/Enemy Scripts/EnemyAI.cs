using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        // Object References
        PlayerMovement player;
        Character character;

        // Chase Range
        float distanceToPlayer;

        // Weapon System
        WeaponSystem weaponSystem;
        float currentWeaponRange;

        // Variables
        [SerializeField] float chaseRadius = 6f;
        bool isAttacking; // TODO Turn into richer state

        enum State { idle, attacking, patrolling, chasing}
        State state = State.idle;

        private void Start ()
        {
            player = FindObjectOfType<PlayerMovement>();
            weaponSystem = GetComponent<WeaponSystem>();
            character = GetComponent<Character>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetAttackRange();
        }

        private void Update ()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                StopAllCoroutines();
                state = State.patrolling;
            }

            if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }

            if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
            {
                StopAllCoroutines();
                state = State.attacking;
            }
        }

        IEnumerator ChasePlayer ()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnDrawGizmos ()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);
        }
    }
}
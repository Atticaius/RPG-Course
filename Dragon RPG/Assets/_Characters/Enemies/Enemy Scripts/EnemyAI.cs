using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        // Object References
        PlayerMovement player;
        Character character;

        // Chase Range
        [SerializeField] float chaseRadius = 6f;
        float distanceToPlayer;

        // Waypoint System
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointDistance = 1f;
        float waitSeconds = .5f;
        enum State { idle, attacking, patrolling, chasing }
        State state = State.idle;
        int nextWaypointIndex = 0;

        // Weapon System
        WeaponSystem weaponSystem;
        float currentWeaponRange;

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
                StartCoroutine(Patrol());
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
                weaponSystem.AttackTarget(player.gameObject);
            }
        }

        IEnumerator Patrol ()
        {
            state = State.patrolling;
            while (true)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);
                CycleWaypointWhenClose(nextWaypointPos);
                yield return new WaitForSeconds(waitSeconds);
            }
        }

        void CycleWaypointWhenClose (Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointDistance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
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
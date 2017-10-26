﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class Projectile : MonoBehaviour
    {

        // Components
        Rigidbody myRigidbody;
        GameObject shooter;

        // Variables
        [SerializeField] float damageCaused = 10f;
        [SerializeField] float projectileSpeed = 10f;
        public float GetProjectileSpeed
        {
            get
            {
                return projectileSpeed;
            }
        }

        // Constants
        const float DEFAULT_DELAY = 10f;
        const float DESTROY_DELAY = .3f;
        
        public void FireProjectile (GameObject shotFrom, Vector3 unitVectorToTarget, float damage = DEFAULT_DELAY)
        {
            myRigidbody = GetComponent<Rigidbody>();
            damageCaused = damage;
            shooter = shotFrom;
            myRigidbody.velocity = (unitVectorToTarget * projectileSpeed);
        }

        private void OnCollisionEnter (Collision other)
        {
            if (shooter != null && shooter.layer != other.gameObject.layer)
            {
                DamageIfDamageable(other);
            } 
        }

        private void DamageIfDamageable (Collision other)
        {
            HealthSystem damageableComponent = other.gameObject.GetComponent<HealthSystem>();
            if (damageableComponent)
            {
                damageableComponent.TakeDamage(damageCaused);
                Destroy(gameObject);
            }
            Destroy(gameObject, DESTROY_DELAY);
        }
    }
}
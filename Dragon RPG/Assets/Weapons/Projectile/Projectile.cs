using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    // Components
    Rigidbody myRigidbody;

    // Variables
    [SerializeField] float damageCaused = 10f;
    const float defaultDamage = 10f;
    public float projectileSpeed;
    GameObject spawnedFrom;

    public void FireProjectile (GameObject spawner, Vector3 unitVectorToTarget, float damage = defaultDamage)
    {
        myRigidbody = GetComponent<Rigidbody>();
        damageCaused = damage;
        spawnedFrom = spawner;
        myRigidbody.velocity = (unitVectorToTarget * projectileSpeed);
    }

    

    private void OnCollisionEnter (Collision other)
    {
        Component damageableComponent = other.gameObject.GetComponent(typeof(IDamageable));
        if (damageableComponent && spawnedFrom != other.gameObject)
        {
            (damageableComponent as IDamageable).TakeDamage(damageCaused);
            Destroy(gameObject);
        } else if (spawnedFrom != other.gameObject)
        {
            Destroy(gameObject);
        }
    }

    
}

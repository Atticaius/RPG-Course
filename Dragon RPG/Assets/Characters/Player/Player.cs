using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    // Object references
    CameraRaycaster cameraRaycaster;
    [SerializeField] GameObject currentTarget;

    // Variables
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackRange = 3f;
    [SerializeField] float secondsBetweenHits = 3f;
    float lastHitTime;
    const int enemyLayer = 9;
    float currentHealthPoints = 100f;
    public float HealthAsPercentage
    {
        get
        {
            return currentHealthPoints / maxHealthPoints;
        }
    }

    void Start ()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }

    void OnMouseClick (RaycastHit raycastHit, int layerHit)
    {
        currentTarget = raycastHit.collider.gameObject;
        if (layerHit == 9 && currentTarget != null)
        {
            InvokeRepeating("AttackTarget", 0, .1f);
        }
    }

    void AttackTarget ()
    {
        if (currentTarget != null)
        {
            bool isInRange = Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange;
            if (Time.time - lastHitTime > secondsBetweenHits && isInRange)
            {
                currentTarget.GetComponent<Enemy>().TakeDamage(attackDamage);
                lastHitTime = Time.time;
            } 
        } else
        {
            CancelInvoke();
        }
    }

    public void TakeDamage (float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
    }
}

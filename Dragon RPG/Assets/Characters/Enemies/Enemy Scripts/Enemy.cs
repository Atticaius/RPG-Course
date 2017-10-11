using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable {

    // Object References
    AICharacterControl aiCharacterControl;
    Player player;

    // Variables
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float followRadius = 6f;
    [SerializeField] float attackRadius = 3f;
    float currentHealthPoints = 100f;
    
    public float HealthAsPercentage
    {
        get
        {
            return currentHealthPoints / maxHealthPoints;
        }
    }

    public void TakeDamage (float damage)

    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0, maxHealthPoints);
    }

    private void Start ()
    {
        aiCharacterControl = GetComponent<AICharacterControl>();
        player = FindObjectOfType<Player>();
    }

    private void Update ()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= followRadius)
        {
            aiCharacterControl.SetTarget(player.transform);
        } else
        {
            aiCharacterControl.SetTarget(transform);
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

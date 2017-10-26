using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Characters
{
    [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour
    {
        [SerializeField] Weapon weaponConfig;
        [SerializeField] AudioClip pickUpSound;
        AudioSource audioSource;

        // Use this for initialization
        void Start ()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update ()
        {
            if (!Application.isPlaying)
            {
                DestroyChildren();
                InstantiateWeapon();
            }
        }

        void DestroyChildren ()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        void InstantiateWeapon ()
        {
            GameObject weapon = weaponConfig.GetWeaponPrefab();
            weapon.transform.position = Vector3.zero;
            Instantiate(weapon, gameObject.transform);
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.GetComponent<PlayerMovement>())
            {
                PlayerMovement player = other.GetComponent<PlayerMovement>();
                player.PutWeaponInHand(weaponConfig);
                audioSource.PlayOneShot(pickUpSound);
            }
            
        }
    }
}
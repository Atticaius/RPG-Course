using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class Weapon : ScriptableObject
    {

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        public Transform gripTransform;

        public GameObject GetWeaponPrefab ()
        {
            return weaponPrefab;
        }
    }
}
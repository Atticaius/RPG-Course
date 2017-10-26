using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]
    public class SelfHealConfig : AbilityConfig
    {
        [Header("Self-Heal Specific")]
        [SerializeField] float extraHealth = 100f;
        PlayerMovement player;

        public override AbilityBehavior GetBehaviorComponent (GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<SelfHealBehavior>();
        }

        public float GetExtraHealth
        {
            get
            {
                return extraHealth;
            }
        }
    }
}
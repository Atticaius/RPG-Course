using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]
    public class SelfHealConfig : AbilityConfig
    {
        [Header("Self-Heal Specific")]
        [SerializeField] float extraHealth = 50f;
        Player player;

        public override AbilityBehavior GetBehaviorComponent (GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<SelfHealBehavior>();
        }

        public float GetHealingAmount ()
        {
            player = FindObjectOfType<Player>();
            float healingAmount = player.maxHealthPoints - player.currentHealthPoints;
            return healingAmount;
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
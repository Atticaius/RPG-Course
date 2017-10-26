using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Power Attack"))]
    public class PowerAttackConfig : AbilityConfig
    {
        [Header("Power Attack Specific")]
        [SerializeField]
        float extraDamage = 10f;

        public override AbilityBehavior GetBehaviorComponent (GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<PowerAttackBehavior>();
        }

        public float GetAbilityDamage ()
        {
            return extraDamage;
        }
    }
}
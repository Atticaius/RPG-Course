using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area of Effect"))]
    public class AreaOfEffectConfig : AbilityConfig
    {
        [Header("Area of Effect Specific")]
        [SerializeField]
        float radius = 5f;
        [SerializeField]
        float damageToEachTarget = 10f;

        public override AbilityBehavior GetBehaviorComponent (GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaOfEffectBehavior>();
        }

        public float GetAbilityDamage ()
        {
            return damageToEachTarget;
        }

        public float GetRadius ()
        {
            return radius;
        }
    }
}
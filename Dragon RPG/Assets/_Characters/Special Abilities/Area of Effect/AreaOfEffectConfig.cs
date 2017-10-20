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

        public override void AttachComponentTo (GameObject gameObjectToAttachTo)
        {
            AreaOfEffectBehavior behaviorComponent = gameObjectToAttachTo.AddComponent<AreaOfEffectBehavior>();
            behaviorComponent.SetConfig(this);
            behavior = behaviorComponent;
        }

        public float GetExtraDamage ()
        {
            return damageToEachTarget;
        }

        public float GetRadius ()
        {
            return radius;
        }
    }
}
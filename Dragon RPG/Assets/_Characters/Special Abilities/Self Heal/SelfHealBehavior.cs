using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : AbilityBehavior
    {
        PlayerMovement player;

        void Start ()
        {
            player = GetComponent<PlayerMovement>();
        }

        public override void Use(GameObject target)
        {
            player.GetComponent<HealthSystem>().Heal((config as SelfHealConfig).GetExtraHealth);
            PlayParticleEffect();
            PlayAbilitySound();
        }
    }
}
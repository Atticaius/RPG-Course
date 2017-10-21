using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : AbilityBehavior
    {
        Player player;

        void Start ()
        {
            player = GetComponent<Player>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            player.Heal((config as SelfHealConfig).GetExtraHealth);
            PlayParticleEffect();
            PlayAbilitySound();
        }
    }
}
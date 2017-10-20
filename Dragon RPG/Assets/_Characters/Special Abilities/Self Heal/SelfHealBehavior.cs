using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config;
        Player player;
        AudioSource audioSource;

        void Start ()
        {
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }

        public void SetConfig (SelfHealConfig configToSet)
        {
            config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            player.Heal(config.GetExtraHealth);
            PlayHealSound();
            PlayParticleEffect();
        }

        void PlayHealSound ()
        {
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }

        private void PlayParticleEffect ()
        {
            GameObject particleSystemPrefab = config.GetParticlePrefab();
            GameObject newParticleSystem = Instantiate(particleSystemPrefab, transform.position, particleSystemPrefab.transform.rotation);
            ParticleSystem myParticleSystem = newParticleSystem.GetComponent<ParticleSystem>();
            myParticleSystem.transform.parent = transform;
            myParticleSystem.Play();
            Destroy(newParticleSystem, myParticleSystem.main.duration);
        }
    }
}
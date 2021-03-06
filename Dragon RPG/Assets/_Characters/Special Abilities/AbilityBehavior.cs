﻿using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehavior : MonoBehaviour
    {
        protected AbilityConfig config;

        const float PARTICLE_CLEANUP_DELAY = 20f;
        const string DEFAULT_ATTACK = "Default Attack";
        const string ATTACK_TRIGGER = "Attack";

        public abstract void Use (GameObject target = null);

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayParticleEffect ()
        {
            GameObject particlePrefab = config.GetParticlePrefab;
            GameObject particleSystemObject = Instantiate(particlePrefab, transform.position, particlePrefab.transform.rotation);
            particleSystemObject.transform.parent = transform;
            particleSystemObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyParticleAfterDelay(particleSystemObject));
        }

        IEnumerator DestroyParticleAfterDelay (GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEANUP_DELAY);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }

        protected void PlayAbilitySound ()
        {
            AudioClip abilitySound = config.GetAudioClip;
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(abilitySound);
        }

        protected void PlayAbilityAnimation ()
        {
            AnimatorOverrideController animatorOverrideController = GetComponent<Character>().GetOverrideController;
            Animator animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = config.GetAnimationClip;
            animator.SetTrigger(ATTACK_TRIGGER);
        }
    }
}
using System.Collections.Generic;
using src.Singletons;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [SerializeField] private List<AudioClip> m_dash;
        [SerializeField] private List<AudioClip> m_hitEnemy;
        [SerializeField] private List<AudioClip> m_hitMe;
        [SerializeField] private List<AudioClip> m_mobStep;
        [SerializeField] private List<AudioClip> m_raleAgonie;
        [SerializeField] private List<AudioClip> m_hitBones;
        [SerializeField] private List<AudioClip> m_fireBall;
        [SerializeField] private List<AudioClip> m_fireBallExplosion;
        [SerializeField] private List<AudioClip> m_AoE;
        [SerializeField] private List<AudioClip> m_hitFlesh;
        [SerializeField] private List<AudioClip> m_ShootArrow;
        [SerializeField] private List<AudioClip> m_ArrowImpact;

        [SerializeField]
        private AudioSource m_audioSource;
        
        
        public static void PlaySoundFireBall()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_fireBall));
        }   
        
        public static void PlaySoundFireBallExplosion()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_fireBallExplosion));
        }   
        
        public static void PlaySoundAoE()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_AoE));
        }  
        
        public static void PlaySoundHitFlesh()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_hitFlesh));
        }   
        
        public static void PlaySoundShootArrow()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_ShootArrow));
        }
        
        public static void PlaySoundImpact()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_ArrowImpact));
        }
        
        public static void PlaySoundHitBones()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_hitBones));
        }

        
        public static void PlaySoundDash()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_dash));
        }
        
        public static void PlaySoundHitEnemy()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_hitEnemy));
        }
        
        public static void PlaySoundHitMe()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_hitMe));
        }
        
        public static void PlaySoundMobStep()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_mobStep));
        }
        
        public static void PlaySoundRaleAgonie()
        {
            if(instance == null) return;
            instance.m_audioSource.PlayOneShot(GetRandomSoundFromList(instance.m_raleAgonie));
        }

        private static AudioClip GetRandomSoundFromList(List<AudioClip> possibilities)
        {
            if (possibilities.Count <= 0)
            {
                return null;
            }
            return possibilities[Random.Range(0, possibilities.Count - 1)];
        }
        
    }
}
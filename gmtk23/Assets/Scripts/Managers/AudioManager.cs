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

        [SerializeField]
        private AudioSource m_audioSource;
        
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
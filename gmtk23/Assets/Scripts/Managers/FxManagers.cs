using System;
using src.Log;
using src.Pools;
using src.Singletons;
using UnityEngine;

namespace Managers
{
    public class FxManagers : MonoSingleton<FxManagers>
    {
        [SerializeField] private ParticleSystem m_damageFx;
        [SerializeField] private ParticleSystem m_humanBloodFx;
        [SerializeField] private ParticleSystem m_orcBloodFx;

        private Pool<ParticleSystem> m_damagesPool;
        private Pool<ParticleSystem> m_orcBloodPool;
        private Pool<ParticleSystem> m_humanBloodPool;

        private void Start()
        {
            m_damagesPool = new Pool<ParticleSystem>(m_damageFx);
            m_orcBloodPool = new Pool<ParticleSystem>(m_orcBloodFx);
            m_humanBloodPool = new Pool<ParticleSystem>(m_humanBloodFx);
        }

        public static void RequestDamageFxAtPos(Vector3 position)
        {
            if (instance == null)
            {
                Logs.Log("Fx manager instance null ...", LogType.Error);
                return;
            }

            var effect = instance.m_damagesPool.GetFromPool();
            effect.gameObject.SetActive(true);
            effect.Stop();

            effect.transform.position = position;
            
            effect.Play();
            
            instance.StartCoroutine(instance.m_damagesPool.AddToPoolLatter(effect, 0.5f));
        }
        
        public static void RequestBloodFxAtPos(Vector3 position,bool orcBlood)
        {
            if (instance == null)
            {
                Logs.Log("Fx manager instance null ...", LogType.Error);
                return;
            }

            var effect = orcBlood ? instance.m_orcBloodPool.GetFromPool() : instance.m_humanBloodPool.GetFromPool();
            effect.gameObject.SetActive(true);
            effect.Stop();

            effect.transform.position = position;
            
            effect.Play();
            
            instance.StartCoroutine( orcBlood ? instance.m_orcBloodPool.AddToPoolLatter(effect, 0.5f) : instance.m_humanBloodPool.AddToPoolLatter(effect, 0.5f));
        }
        
    }
}
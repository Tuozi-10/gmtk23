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

        private Pool<ParticleSystem> m_damagesPool;

        private void Start()
        {
            m_damagesPool = new Pool<ParticleSystem>(m_damageFx);
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
    }
}
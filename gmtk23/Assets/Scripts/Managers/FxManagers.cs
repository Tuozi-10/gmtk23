using System;
using IAs;
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
        [SerializeField] private ParticleSystem m_healFx;
        [SerializeField] private ParticleSystem m_healShockWaveFx;
        [SerializeField] private ParticleSystem m_hitShockWaveFx;
        [SerializeField] private ParticleSystem m_dashEffect;

        private Pool<ParticleSystem> m_damagesPool;
        private Pool<ParticleSystem> m_orcBloodPool;
        private Pool<ParticleSystem> m_humanBloodPool;
        private Pool<ParticleSystem> m_healPool;
        private Pool<ParticleSystem> m_healShockWavePool;
        private Pool<ParticleSystem> m_hitShockWavePool;
        private Pool<ParticleSystem> m_dashEffectPool;

        private void Start()
        {
            m_damagesPool = new Pool<ParticleSystem>(m_damageFx);
            m_orcBloodPool = new Pool<ParticleSystem>(m_orcBloodFx);
            m_humanBloodPool = new Pool<ParticleSystem>(m_humanBloodFx);
            m_healPool = new Pool<ParticleSystem>(m_healFx);
            m_healShockWavePool = new Pool<ParticleSystem>(m_healShockWaveFx);
            m_hitShockWavePool = new Pool<ParticleSystem>(m_hitShockWaveFx);
            m_dashEffectPool = new Pool<ParticleSystem>(m_dashEffect);
        }

        public static void Purge()
        { 
            instance.StopAllCoroutines();
            
            instance.m_damagesPool = new Pool<ParticleSystem>(instance.m_damageFx);
            instance.m_orcBloodPool = new Pool<ParticleSystem>(instance.m_orcBloodFx);
            instance.m_humanBloodPool = new Pool<ParticleSystem>(instance.m_humanBloodFx);
            instance.m_healPool = new Pool<ParticleSystem>(instance.m_healFx);
            instance.m_healShockWavePool = new Pool<ParticleSystem>(instance.m_healShockWaveFx);
            instance.m_hitShockWavePool = new Pool<ParticleSystem>(instance.m_hitShockWaveFx);
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
            
            instance.StartCoroutine( orcBlood ? instance.m_orcBloodPool.AddToPoolLatter(effect, 10) : instance.m_humanBloodPool.AddToPoolLatter(effect, 10));
        }
        
        public static void RequestHealFxAtPos(Vector3 position)
        {
            if (instance == null)
            {
                Logs.Log("Fx manager instance null ...", LogType.Error);
                return;
            }

            var effect =  instance.m_healPool.GetFromPool();
            effect.gameObject.SetActive(true);
            effect.Stop();

            effect.transform.position = position;
            
            effect.Play();
            
            instance.StartCoroutine( instance.m_humanBloodPool.AddToPoolLatter(effect, 3.5f));
        }
        
        public static void RequestHealShockWaveFxAtPos(Vector3 position, float scale = 1f, AI.Team team = AI.Team.Hero, int damages = 1)
        {
            if (instance == null)
            {
                Logs.Log("Fx manager instance null ...", LogType.Error);
                return;
            }

            position.y = 0.05f;
            var effect =  instance.m_healShockWavePool.GetFromPool();
            effect.gameObject.SetActive(true);
            effect.Stop();
            effect.transform.localScale = Vector3.one * scale;
            effect.transform.position = position;
            
            effect.Play();
            effect.GetComponent<ShockWave>().SetUp(team, damages);
            
            instance.StartCoroutine( instance.m_healShockWavePool.AddToPoolLatter(effect, .7f));
        }

        public static void RequestHitShockWaveFxAtPos(Vector3 position, float scale = 1f, AI.Team team = AI.Team.Hero, int damages = 1)
        {
            if (instance == null)
            {
                Logs.Log("Fx manager instance null ...", LogType.Error);
                return;
            }

            position.y = 0.05f;
            var effect =  instance.m_hitShockWavePool.GetFromPool();
            effect.transform.localScale = Vector3.one * scale;
            effect.gameObject.SetActive(true);
            effect.Stop();

            effect.transform.position = position;
            
            effect.Play();
            effect.GetComponent<ShockWave>().SetUp(team, damages);
            instance.StartCoroutine( instance.m_hitShockWavePool.AddToPoolLatter(effect, .7f));
        }
        
        public static void RequestDashFxAtPos(Vector3 position)
        {
            if (instance == null)
            {
                Logs.Log("Fx manager instance null ...", LogType.Error);
                return;
            }

            var effect = instance.m_dashEffectPool.GetFromPool();
            effect.gameObject.SetActive(true);
            effect.Stop();

            effect.transform.position = position;
            
            effect.Play();
            
            instance.StartCoroutine(instance.m_dashEffectPool.AddToPoolLatter(effect, 2f));
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IAs;
using UnityEngine;
using Random = UnityEngine.Random;

public class PackMobManager : MonoBehaviour
{
    [Header("Core Parameters")]
    [SerializeField] private Pack packLink;
    [SerializeField] private float spread;
    [SerializeField] private float timerToRespawn;
    [SerializeField] private float detectionRangeFactor;
    [SerializeField] private bool spawnRandom;
    [SerializeField] private bool isMenu;

    [Header("Random Spawn Parameters")]
    [SerializeField] private Vector2 rangeNumberSpawn;
    [SerializeField] private List<GameObject> mobsToSpawnRandom = new List<GameObject>();

    [Header("Fix Spawn Parameters")] [SerializeField]
    private List<GameObject> mobsToSpawnFix = new List<GameObject>();

    public List<GameObject> _mobsSpawned = new List<GameObject>();
    private bool isSpawnable;

    private void Start()
    {
        isSpawnable = true;
        
        switch (spawnRandom)
        {
            case true : SpawnRandomMobPack();
                break;
            case false : SpawnFixedMobPack();
                break;
        }
    }

    private void Update()
    {
        ClearList();
        
        if (_mobsSpawned.Count > 0 || !isSpawnable) return;
        
        isSpawnable = false;
        StartCoroutine(NewPackIn());
    }

    private void SpawnRandomMobPack()
    {
        var position = transform.position;
        var random = Random.Range(rangeNumberSpawn.x, rangeNumberSpawn.y);
        
        for (int i = 0; i < random; i++)
        {
            var ran = Random.Range(0, mobsToSpawnRandom.Count);
            var spreadVector = new Vector3(Random.Range(-spread, spread) + position.x, position.y, Random.Range(-spread, spread) + position.z);

            var newMob = Instantiate(mobsToSpawnRandom[ran], spreadVector, Quaternion.identity, transform);
            _mobsSpawned.Add(newMob);
            newMob.GetComponent<AI>().currentPack = packLink;
            packLink.m_ais.Add(newMob.GetComponent<AI>());
        }

        isSpawnable = true;
    }

    private void SpawnFixedMobPack()
    {
        var position = transform.position;
        
        foreach (var mob in mobsToSpawnFix)
        {
            var spreadVector = new Vector3(Random.Range(-spread, spread) + position.x, position.y, Random.Range(-spread, spread) + position.z);
            
            var newMob = Instantiate(mob, spreadVector, Quaternion.identity, transform);
            _mobsSpawned.Add(newMob);
            newMob.GetComponent<AI>().currentPack = packLink;
            packLink.m_ais.Add(newMob.GetComponent<AI>());
        }
        
        isSpawnable = true;
    }

    IEnumerator NewPackIn()
    {
        yield return new WaitForSeconds(timerToRespawn);
        switch (spawnRandom)
        {
            case true : SpawnRandomMobPack();
                break;
            case false : SpawnFixedMobPack();
                break;
        }
    }

    public void JsuisDead()
    {
        for (int i = 0; i < _mobsSpawned.Count; i++)
        {
            if (_mobsSpawned[i] == null) _mobsSpawned.Remove(_mobsSpawned[i]);
        }
    }
    
    private void ClearList()
    {
        List<GameObject> toremove = new();
        foreach (var ai in _mobsSpawned)
        {
            if (ai == null)
            {
                toremove.Add(ai);
            }
        }

        foreach (var t in toremove)
        {
            _mobsSpawned.Remove(t);
        }
        
    }
}
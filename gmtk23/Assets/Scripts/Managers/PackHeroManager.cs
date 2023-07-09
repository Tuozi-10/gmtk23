using System;
using System.Collections;
using System.Collections.Generic;
using IAs;
using src.Singletons;
using UnityEngine;
using Random = UnityEngine.Random;

public class PackHeroManager : MonoSingleton<PackHeroManager>
{
    [SerializeField] private Transform startEntry;
    [SerializeField] private GameObject firstHeroPack;
    [SerializeField] private float firstTimerBeforeSpawn;
    [SerializeField] private float timeBetweenRespawn;
    [SerializeField, Range(0,50)] private int numberOfPackMinToHave;
    [SerializeField] private List<GameObject> packHeroList = new List<GameObject>();

    private float timer;
    private bool isTuto;
    private bool needRespawn;
    [HideInInspector] public List<Pack> numberOfPackInGame = new List<Pack>();

    private void Start()
    {
        isTuto = true;
        
        var newPack = Instantiate(firstHeroPack, startEntry.position, Quaternion.identity);
        newPack.GetComponent<Pack>().tracking.m_currentRoad = RoadManager.GetRandomRoad();
        
        numberOfPackInGame.Add(newPack.GetComponent<Pack>());
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < timeBetweenRespawn) return;
        if (isTuto && timer < firstTimerBeforeSpawn) return;
        
        CheckForRespawn();
    }

    private void GetRandomPack(Vector3 position)
    {
        var ran = Random.Range(0, packHeroList.Count);
        var newPack = Instantiate(packHeroList[ran], position, Quaternion.identity, transform);
        newPack.GetComponent<Pack>().tracking.m_currentRoad = RoadManager.GetRandomRoad();
        
        numberOfPackInGame.Add(newPack.GetComponent<Pack>());
    }

    private void CheckForRespawn()
    {
        if (numberOfPackInGame.Count > numberOfPackMinToHave) return;
        GetRandomPack(startEntry.position);
        timer = 0;

        if (isTuto) isTuto = false;
    }
}

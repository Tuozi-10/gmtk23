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
    
    [Header("C'est pour le tuto wesh")]
    [SerializeField] private List<Transform> possibleSpawnPoint;
    [SerializeField] private bool isTutorial;
    [SerializeField] private float packToSpawn;

    private float timer;
    private bool isTuto;
    private bool needRespawn;
    [HideInInspector] public List<Pack> numberOfPackInGame = new List<Pack>();

    private void Start()
    {
        isTuto = true;
        
        var newPack = Instantiate(firstHeroPack, startEntry.position, Quaternion.identity);
        switch (isTutorial)
        {
            case false:
                newPack.GetComponent<Pack>().tracking.m_currentRoad = RoadManager.GetRandomRoad();
                break;
            case true:
            {
                for (int i = 0; i < packToSpawn; i++)
                {
                    var ran = Random.Range(0, packHeroList.Count);
                    var rand = Random.Range(0, possibleSpawnPoint.Count);
                    GameObject ElPacko = Instantiate(packHeroList[ran], possibleSpawnPoint[rand].position, Quaternion.identity);
                    numberOfPackInGame.Add(ElPacko.GetComponent<Pack>());
                }

                break;
            }
        }

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

        if (isTutorial)
        {
            var rand = Random.Range(0, possibleSpawnPoint.Count);
            GameObject ElPacko = Instantiate(packHeroList[ran], possibleSpawnPoint[rand].position, Quaternion.identity);
            numberOfPackInGame.Add(ElPacko.GetComponent<Pack>());
        }
        else
        {
            var newPack = Instantiate(packHeroList[ran], position, Quaternion.identity, transform);
            newPack.GetComponent<Pack>().tracking.m_currentRoad = RoadManager.GetRandomRoad();
        
            numberOfPackInGame.Add(newPack.GetComponent<Pack>());
        }
    }

    private void CheckForRespawn()
    {
        if (numberOfPackInGame.Count > numberOfPackMinToHave) return;

        switch (isTutorial)
        {
            case true :
                GetRandomPack(transform.position);
                break;
            case false :
                GetRandomPack(startEntry.position);
                break;
        }
        timer = 0;

        if (isTuto) isTuto = false;
    }
}

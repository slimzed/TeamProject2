using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefabWeak;
    [SerializeField] private GameObject enemyPrefabNormal;
    [SerializeField] private GameObject enemyPrefabElite;


    [Tooltip("-1 if its going left, 1 if its going to the right")]
    [SerializeField] private int moveDir = -1;
    
    
    
    [SerializeField] private int startTime = 0;
    [SerializeField] private int spawnOnBeatInterval = 1;
    [SerializeField] private int BeatsPerSpawn = 3;
    private int _initBeats;

    [SerializeField] private bool isMainSpawner;


    private void Start()
    {
        _initBeats = BeatsPerSpawn;
        AudioManager.OnBeat += HandleBeat;
    }
    public void CancelSpawn()
    {
        CancelInvoke("SpawnEnemyPrefab");
    }
    private void HandleBeat(int beatNumber, bool isFirstSpawner)
    {
        if (beatNumber >= startTime && (beatNumber - startTime) % spawnOnBeatInterval == 0)
        {
            // this if statement alternates between enemy spawners 
            if (isMainSpawner && isFirstSpawner || !isMainSpawner && !isFirstSpawner)
            {
                BeatsPerSpawn--;
                if (BeatsPerSpawn <= 0)
                {
                    SpawnEnemyPrefab();
                    BeatsPerSpawn = _initBeats;
                }
            }
        }
    }


    private void SpawnEnemyPrefab()
    {

        int enemySelection = Random.Range(0, 100); // hopefully going to be used to spawn enemies on a weighted average
        GameObject enemy;

        if (enemySelection <= 10)
        {
           enemy = Instantiate(enemyPrefabElite, gameObject.transform.position, Quaternion.identity);
        } else if (enemySelection > 10 && enemySelection <= 30)
        {
            enemy = Instantiate(enemyPrefabNormal, gameObject.transform.position, Quaternion.identity);
        } else
        {
            enemy = Instantiate(enemyPrefabWeak, gameObject.transform.position, Quaternion.identity);
        }
        enemy.transform.SetParent(transform);
        
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.moveDir = moveDir;
    }

}

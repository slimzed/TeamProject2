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
    [SerializeField] private int BeatsPerSpawn = 16;


    private int _initBeats;


    private int levelNumber = 1;

    [SerializeField] private bool isMainSpawner; // set in the inspector, this is used to determine whether or not the spawner is the main one.


    private void Start()
    {
        _initBeats = BeatsPerSpawn;
        AudioManager.OnBeat += HandleBeat;
        AudioManager.OnGameVictory += HandleVictory;
    }

    private void HandleBeat(int beatNumber, bool isFirstSpawner, float beatTimeDifference)
    {
        if (beatNumber >= startTime && (beatNumber - startTime) % spawnOnBeatInterval == 0)
        {
            if (isMainSpawner && isFirstSpawner || !isMainSpawner && !isFirstSpawner)
            {
                BeatsPerSpawn--;
                if (startTime <= 0 && BeatsPerSpawn <= 0)
                {
                    BeatsPerSpawn = _initBeats;
                    switch (levelNumber)
                    {
                        case 1:
                            SpawnEnemyPrefab1();
                            break;
                        case 2:
                            SpawnEnemyPrefab2();
                            break;
                        case 3:
                            SpawnEnemyPrefab3();
                            break;
                        default:
                            Debug.LogWarning("No enemy prefab defined for level " + levelNumber);
                            break;
                    }
                }
            }
        }
    }
    private void HandleVictory()
    {
        levelNumber++; // this will increment every time the player wins a level starting at 1
    }


    private int SelectPrefabNumber()
    {
        return Random.Range(0, 100); // hopefully going to be used to spawn enemies on a weighted average
  
    }

    /// <summary>
    /// The scripts below calculate weighted randoms to determine which enemy prefab to spawn based around the given level.
    /// </summary>
    private void SpawnEnemyPrefab1() {
        int enemySelection = SelectPrefabNumber();
        GameObject enemy;

        if (enemySelection < 70)
        {
            enemy = Instantiate(enemyPrefabWeak, transform.position, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(enemyPrefabNormal, transform.position, Quaternion.identity);
        }
        enemy.transform.SetParent(transform);

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.moveDir = moveDir;
    }
    private void SpawnEnemyPrefab2()
    {
        int enemySelection = SelectPrefabNumber();
        GameObject enemy;

        if (enemySelection < 50)
        {
            enemy = Instantiate(enemyPrefabWeak, transform.position, Quaternion.identity);
        }
        else if (enemySelection < 90)
        {
            enemy = Instantiate(enemyPrefabNormal, transform.position, Quaternion.identity);
        } else
        {
            enemy = Instantiate(enemyPrefabElite, transform.position, Quaternion.identity);
        }
    }
    private void SpawnEnemyPrefab3()
    {
        int enemySelection = SelectPrefabNumber();
        GameObject enemy;

        if (enemySelection < 30)
        {
            enemy = Instantiate(enemyPrefabWeak, transform.position, Quaternion.identity);
        }
        else if (enemySelection < 70)
        {
            enemy = Instantiate(enemyPrefabNormal, transform.position, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(enemyPrefabElite, transform.position, Quaternion.identity);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [Tooltip("-1 if its going left, 1 if its going to the right")]
    [SerializeField] private int moveDir = -1;
    
    
    
    [SerializeField] private int startTime = 0;
    [SerializeField] private int spawnOnBeatInterval = 1;

    [SerializeField] private bool isMainSpawner;


    private void Start()
    {
        Debug.Log("started");
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
            if (isMainSpawner && isFirstSpawner)
            {
                SpawnEnemyPrefab();
            } else if (!isMainSpawner && !isFirstSpawner)
            {
                SpawnEnemyPrefab();
            }
        }
    }


    private void SpawnEnemyPrefab()
    {
        Debug.Log("spawned");
        GameObject enemy = Instantiate(enemyPrefab, gameObject.transform.position, Quaternion.identity);
        enemy.transform.SetParent(transform);
        
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.moveDir = moveDir;
    }

}

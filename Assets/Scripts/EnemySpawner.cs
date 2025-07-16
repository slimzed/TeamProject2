using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [Tooltip("-1 if its going left, 1 if its going to the right")]
    [SerializeField] private int moveDir = -1;
    
    
    
    [SerializeField] private float startTime = 2f;
    [SerializeField] private float beatTime = 5f;


    private void Start()
    {
        Debug.Log("started");
        InvokeRepeating("SpawnEnemyPrefab", startTime, beatTime);
    }
    public void CancelSpawn()
    {
        CancelInvoke("SpawnEnemyPrefab");
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

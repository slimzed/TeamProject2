using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;


    private void Start()
    {
        Invoke("SpawnEnemyPrefab", 2f);
    }


    private void SpawnEnemyPrefab()
    {
        GameObject enemy = Instantiate(enemyPrefab, gameObject.transform);
    }

}

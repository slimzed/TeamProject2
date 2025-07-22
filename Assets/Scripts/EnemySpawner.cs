using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefabWeak;
    [SerializeField] private GameObject enemyPrefabNormal;
    [SerializeField] private GameObject enemyPrefabElite;

    [SerializeField] private List<EnemySpawnData> enemySpawnDatas = new();

    [Tooltip("-1 if its going left, 1 if its going to the right")]
    [SerializeField] private int moveDir = -1;

    [Tooltip("How often should you iterate down the intervals")]
    [SerializeField] private int spawnOnBeatInterval = 1;

    private int BeatsPerSpawn;
    private int _initBeatsPerSpawn;
    private int _currentLevelNumber = 1;
    private int startTime = 0;

    [SerializeField] private bool isMainSpawner;

    private AudioManager _audioManager;

    [System.Serializable]
    public class EnemySpawnData
    {
        [SerializeField] public List<IndividualEnemySpawnData> IndividualEnemySpawns;

        [SerializeField] public int BeatsPerSpawn;
    }
    
    
    [System.Serializable]
    public class IndividualEnemySpawnData
    {
        public EnemyType enemyType;
        public int AmountOfEntries;
    }

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        if (_audioManager == null)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        _currentLevelNumber = ScoreManager.Instance.LevelNumber;

        BeatsPerSpawn = enemySpawnDatas[_currentLevelNumber - 1].BeatsPerSpawn;
        

        if (!isMainSpawner)
        {
            startTime = BeatsPerSpawn / 2;
        }

        _initBeatsPerSpawn = BeatsPerSpawn;
    }

    private void OnEnable()
    {
        if (_audioManager != null)
        {
            _audioManager.OnBeat += HandleBeat;
        }
    }

    private void OnDisable()
    {
        if (_audioManager != null)
        {
            _audioManager.OnBeat -= HandleBeat;
        }
    }

    private void HandleBeat(int beatNumber, bool isFirstSpawner, float beatTimeDifference)
    {
        if (beatNumber >= startTime && (beatNumber - startTime) % spawnOnBeatInterval == 0)
        {
            if ((isMainSpawner && isFirstSpawner) || (!isMainSpawner && !isFirstSpawner))
            {
                BeatsPerSpawn--;
                if (BeatsPerSpawn <= 0)
                {
                    BeatsPerSpawn = _initBeatsPerSpawn;
                    Debug.Log("beats reset");
                    SpawnEnemyForCurrentLevel();
                }
            }
        }
    }


    private void SpawnEnemyForCurrentLevel()
    {
        Debug.Log("enemy spawned");
        GameObject enemyToSpawn = null;
        enemyToSpawn = SelectEnemyPrefab(_currentLevelNumber - 1);

        if (enemyToSpawn != null)
        {
            GameObject newEnemy = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            newEnemy.transform.SetParent(gameObject.transform);

            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.moveDir = moveDir;
                enemyController.InitializeEnemyCombos(enemyController.enemyType);
            }
        }
    }
    private GameObject SelectEnemyPrefab(int ArrayLocation)
    {
        float enemySelection = Random.value;
        int total = 0;
        
        for (int i=0; i < enemySpawnDatas[ArrayLocation].IndividualEnemySpawns.Count; i++)
        {
            total += enemySpawnDatas[ArrayLocation].IndividualEnemySpawns[i].AmountOfEntries;
        }
        Debug.Log("The total number of entries is: " + total);

        float threshold = 0f;
        foreach (IndividualEnemySpawnData option in enemySpawnDatas[ArrayLocation].IndividualEnemySpawns)
        {
            float scaledProbability = option.AmountOfEntries / (float)total;
            threshold += scaledProbability;

            if (enemySelection < threshold)
            {
                Debug.Log("Selected enemy type: " + option.enemyType + " at threshold: " + threshold);
                switch(option.enemyType)
                {
                    case EnemyType.Weak:
                        return enemyPrefabWeak;
                    case EnemyType.Normal:
                        return enemyPrefabNormal;
                    case EnemyType.Elite:
                        return enemyPrefabElite;
                    default:
                        Debug.LogError("Unknown enemy type selected.");
                        return null; // Fallback in case of an unknown type
                }
            }
        }
        return null;
    }
}
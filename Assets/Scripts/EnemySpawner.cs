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

    private int _initialBeatsPerSpawn;
    private int _currentLevelNumber = 1;

    [SerializeField] private bool isMainSpawner;

    private AudioManager _audioManager;

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
        _initialBeatsPerSpawn = BeatsPerSpawn;
    }

    private void OnEnable()
    {
        if (_audioManager != null)
        {
            _audioManager.OnBeat += HandleBeat;
        }
        AudioManager.OnGameVictory += HandleVictory;
    }

    private void OnDisable()
    {
        if (_audioManager != null)
        {
            _audioManager.OnBeat -= HandleBeat;
        }
        AudioManager.OnGameVictory -= HandleVictory;
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
                    BeatsPerSpawn = _initialBeatsPerSpawn;
                    SpawnEnemyForCurrentLevel();
                }
            }
        }
    }

    private void HandleVictory()
    {
        _currentLevelNumber++;
    }

    private void SpawnEnemyForCurrentLevel()
    {
        GameObject enemyToSpawn = null;
        switch (_currentLevelNumber)
        {
            case 1:
                enemyToSpawn = SelectEnemyPrefab1();
                break;
            case 2:
                enemyToSpawn = SelectEnemyPrefab2();
                break;
            case 3:
                enemyToSpawn = SelectEnemyPrefab3();
                break;
            default:
                break;
        }

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

    private GameObject SelectEnemyPrefab1()
    {
        int enemySelection = Random.Range(0, 100);
        if (enemySelection < 70)
        {
            return enemyPrefabWeak;
        }
        else
        {
            return enemyPrefabNormal;
        }
    }

    private GameObject SelectEnemyPrefab2()
    {
        int enemySelection = Random.Range(0, 100);
        if (enemySelection < 50)
        {
            return enemyPrefabWeak;
        }
        else if (enemySelection < 90)
        {
            return enemyPrefabNormal;
        }
        else
        {
            return enemyPrefabElite;
        }
    }

    private GameObject SelectEnemyPrefab3()
    {
        int enemySelection = Random.Range(0, 100);
        if (enemySelection < 30)
        {
            return enemyPrefabWeak;
        }
        else if (enemySelection < 70)
        {
            return enemyPrefabNormal;
        }
        else
        {
            return enemyPrefabElite;
        }
    }
}
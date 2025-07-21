using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefabWeak;
    [SerializeField] private GameObject enemyPrefabNormal;
    [SerializeField] private GameObject enemyPrefabElite;

    [Tooltip("-1 if its going left, 1 if its going to the right")]
    [SerializeField] private int moveDir = -1;

    [Tooltip("How often should you iterate down the intervals")]
    [SerializeField] private int spawnOnBeatInterval = 1;

    [SerializeField] private int Level1BeatsPerSpawn = 8;
    [SerializeField] private int Level2BeatsPerSpawn = 6;
    [SerializeField] private int Level3BeatsPerSpawn = 4;


    private int BeatsPerSpawn;
    private int _initBeatsPerSpawn;
    private int _currentLevelNumber = 1;
    private int startTime = 0;

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
        _currentLevelNumber = ScoreManager.Instance.LevelNumber;

        switch (_currentLevelNumber) 
        {
            case 1:
                BeatsPerSpawn = Level1BeatsPerSpawn;
                break;
            case 2:
                BeatsPerSpawn = Level2BeatsPerSpawn;
                break;
            case 3:
                BeatsPerSpawn = Level3BeatsPerSpawn;
                break;
            default:
                BeatsPerSpawn = Level1BeatsPerSpawn; 
                break;
        }

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
        Debug.Log("case 1");
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
        Debug.Log("case 2");
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
        Debug.Log("case 3");
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
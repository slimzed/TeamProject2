using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Weak,
    Normal,
    Elite
}


public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;

    [SerializeField] private GameObject explosionParticles;

    public Color enemyColor
    {
        get
        {
            switch(enemyType)
            {
                case EnemyType.Weak:
                    return Color.grey;
                case EnemyType.Normal:
                    return Color.green;
                case EnemyType.Elite:
                    return Color.blue;
                default:
                    return Color.white; // Fallback color
            }   
        }
        private set
        {
            enemyColor = value;
        }
    }

    
    
    [SerializeField] private float enemyMovementSpeed = 2f;
    [Tooltip("-1 for left, 1 for right")]
    [SerializeField] public int moveDir = -1;
    [SerializeField] private int enemyHealth = 5;
    [SerializeField] private float enemyLifeSpan = 1f; // how long the enemy will live before it is destroyed   


    // change from enemy to enemy
    private KeyCode[] requiredSequence;
    private KeyCode midInput;


    public KeyCode[] ComboSequence => requiredSequence; // creates a public variable that just inherits from the requiredSequence private variable

    private void Start()
    {
        if (moveDir == 1)
        {
            midInput = KeyCode.LeftArrow;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        } else if (moveDir == -1)
        {
            midInput = KeyCode.RightArrow;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        InitializeEnemyCombos(enemyType); // enemy type is set within the editor

    }

    private void Update()
    {
        transform.Translate(CalcMovement());
    }

    private Vector3 CalcMovement()
    {
        float horizontalDisplacement = enemyMovementSpeed * moveDir * Time.deltaTime;
        return new Vector3(horizontalDisplacement, 0, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            moveDir = 0; // stops the object from moving when it collides with the player 
            Invoke("EnemyKill", enemyLifeSpan);
        }
    }
    private void InitializeEnemyCombos(EnemyType enemyType)
    {
        switch (enemyType) 
        {
            case EnemyType.Weak:
                requiredSequence = new KeyCode[] { KeyCode.DownArrow, KeyCode.UpArrow };
                break;
            case EnemyType.Normal:
                requiredSequence = new KeyCode[] { midInput, KeyCode.UpArrow };
                break;
            case EnemyType.Elite:
                requiredSequence = new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, midInput };
                break;
        }
    }
    public void KillEnemy() // public accessor so that other scripts can actually call the function 
    {
        StartCoroutine(EnemyKillSequence());
    }

    private IEnumerator EnemyKillSequence() // accessed by the enemy itself
    {
        Destroy(gameObject);
        yield return StartCoroutine(ShowExplosionParticles());
        ScoreManager.Instance.AddToScore(-50); // adds a penalty to the score if the enemy is not killed in time
    }

    private IEnumerator ShowExplosionParticles()
    {
        GameObject explosion = Instantiate(explosionParticles, transform.position, Quaternion.identity);   
        yield return new WaitForSeconds(0.4f);
        Destroy(explosion);
    }




}

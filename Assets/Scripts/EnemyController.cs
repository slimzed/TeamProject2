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
    [SerializeField] private float enemyLifeSpan = 1f; // how long the enemy will live before it is destroyed
    [SerializeField] private float beatMoveTime = 0.15f;


    // change from enemy to enemy
    private KeyCode[] requiredSequence;
    private KeyCode midInput;


    private Vector2 startPos;
    private Vector2 endPos;
    private float moveStartTime;
    private bool isMoving = false;
    private Rigidbody2D rb;
    private AnimationCurve beatMovementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    public KeyCode[] ComboSequence => requiredSequence; // creates a public variable that just inherits from the requiredSequence private variable

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
        endPos = rb.position;
        if (moveDir == 1)
        {
            midInput = KeyCode.LeftArrow;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        } else if (moveDir == -1)
        {
            midInput = KeyCode.RightArrow;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        isMoving = true;
        InitializeEnemyCombos(enemyType); // enemy type is set within the editor
        AudioManager.OnBeat += HandleBeat; 

    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            float t = (Time.time - moveStartTime) / beatMoveTime;
            t = Mathf.Clamp01(t); // clamps the value between 0 and 1
            var locationOnCurve = beatMovementCurve.Evaluate(t);

            Vector2 desiredPos = Vector2.Lerp(startPos, endPos, locationOnCurve);
            rb.MovePosition(desiredPos);

            if (t>= 1f) // checks if the lerp is done
            {
                rb.MovePosition(endPos);
                startPos = endPos; // sets the start position to the end position so that it can move again
            }
        }
    }

    private void HandleBeat(int beatNumber, bool isFirstSpawner)
    {
        moveStartTime = Time.time;
        endPos = startPos + CalcMovement();
    }   

    private Vector2 CalcMovement()
    {
        float horizontalDisplacement = enemyMovementSpeed * moveDir;
        return new Vector2(horizontalDisplacement, 0);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            moveDir = 0; // stops the object from moving when it collides with the player 
            isMoving = false;
            StartCoroutine(EnemyColorLerpSequence());
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
    public void KillEnemy(bool playerKill) // public accessor so that other scripts can actually call the function 
    {
        StartCoroutine(EnemyKillSequence(playerKill));
    }

    private IEnumerator EnemyKillSequence(bool playerKill) // accessed by the enemy itself
    {
        Destroy(gameObject);
        yield return StartCoroutine(ShowExplosionParticles());
        if (playerKill)
        {
            ScoreManager.Instance.AddToScore(100); // Add score for killing the enemy
        } else
        {
            ScoreManager.Instance.AddToScore(-50); // adds a penalty to the score if the enemy is not killed in time
        }
    }

    private IEnumerator ShowExplosionParticles()
    {
        GameObject explosion = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.4f);
    }

    private IEnumerator EnemyColorLerpSequence()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color startColor = spriteRenderer.color; // should pull the tinted blue
        Color targetColor = Color.red;
        float elapsedTime = 0f;
        while (elapsedTime < enemyLifeSpan)
        {
            spriteRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime / enemyLifeSpan);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        spriteRenderer.color = targetColor; // Ensure the final color is set
        StartCoroutine(EnemyKillSequence(false));
    }




}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private int highAttackDamage = 3;
    [SerializeField] private float highAttackCooldown = 0f;
    [SerializeField] private int medAttackDamage = 2;
    [SerializeField] private int lowAttackDamage = 1;
    [SerializeField] private float enemyLifeSpan = 1f;

    private KeyCode[] comboSequence = { KeyCode.UpArrow, KeyCode.None, KeyCode.DownArrow};
    private int currentComboIndex = 0;
    [SerializeField] private float comboTime = 0.5f;
    private float comboTimer = 0;

    private float lastAttackTime = -Mathf.Infinity;

    private List<EnemyController> collidedEnemies = new List<EnemyController>();
    private EnemyController currentEnemy;

    private bool isRight = false;
    private bool isEnemyRight = false;


    // script calls
    private GameObject ScriptCalls;
    private AudioSource source;

    [SerializeField] private AudioClip failure;
    [SerializeField] private AudioClip highhit;
    [SerializeField] private AudioClip lowhit;
    [SerializeField] private AudioClip midhit;
    [SerializeField] private AudioClip enemyCollision;


    private void Awake()
    {
        ScriptCalls = GameObject.Find("ScriptCalls");
        source = ScriptCalls.GetComponent<AudioSource>();
    }

    void Update()
    {
        ApplyPlayerSpriteDirection();

        DetectCurrentEnemy();

        ComboDetectionFunction();
    }


    private void ApplyPlayerSpriteDirection()
    {
        if ( Input.GetKeyDown(KeyCode.D))
        {
            isRight = true;
            gameObject.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            isRight = false;
            gameObject.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    private void DetectCurrentEnemy()
    {
        collidedEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy); // stack overflow code lol, cleans up all enemies once they are destroyed


        if (collidedEnemies.Count > 4)
        {
            Destroy(collidedEnemies[0].gameObject);
            collidedEnemies.RemoveAt(0); // removes the last enemy

            ScoreManager.Instance.AddToScore(-50);
        }
    }
    private void ComboDetectionFunction() 
    {
        Color defaultColor = Color.white;
        if (currentEnemy != null)
        {
            defaultColor = currentEnemy.enemyColor;
        }

        if (currentComboIndex > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                Debug.Log("Combo failed! Time ran out.");
                currentEnemy.GetComponent<SpriteRenderer>().color = defaultColor;
                source.clip = failure;
                source.Play();
                ResetCombo();
            }
        }

        if (currentEnemy != null && Time.time >= lastAttackTime + highAttackCooldown)
        {
            KeyCode currentInputKey = CheckAndAnimateInput();

                if (currentInputKey != KeyCode.None)
            {
                KeyCode requiredKey = KeyCode.None;
                if (currentComboIndex < comboSequence.Length)
                {
                    requiredKey = comboSequence[currentComboIndex];
                }

                if (currentInputKey == requiredKey)
                {
                    Debug.Log("Correct combo input: " + requiredKey);
                    currentComboIndex++;
                    comboTimer = comboTime;
                    lastAttackTime = Time.time;

                    ApplyEnemySpriteHit();

                    if (currentComboIndex == comboSequence.Length)
                    {
                        Debug.Log("Full combo executed! Killing enemy");
                        currentEnemy.KillEnemy();

                        Debug.Log(collidedEnemies.Count);
                        if (collidedEnemies.Count > 1)
                        {
                            currentEnemy = collidedEnemies[collidedEnemies.Count-2];
                            Debug.Log(currentEnemy.name);
                        } else
                        {
                            currentEnemy = null;
                        }



                            lastAttackTime = Time.time;
                        ScoreManager.Instance.AddToScore(50);
                        ResetCombo();
                    }
                }
                else
                {
                    Debug.Log("incorrect input");
                    currentEnemy.gameObject.GetComponent<SpriteRenderer>().color = defaultColor;
                    ScoreManager.Instance.AddToScore(-25);
                    source.clip = failure;
                    source.Play();
                }
            }
        }
    }

    private void ResetCombo()
    {
        currentComboIndex = 0;
        comboTimer = 0f;
        if (currentEnemy != null) comboSequence = currentEnemy.ComboSequence; // accesses the enemies combo 
        Debug.Log("Combo reset.");
    }
    private KeyCode CheckAndAnimateInput()
    {
        KeyCode currentInputKey = KeyCode.None;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentInputKey = KeyCode.DownArrow;
            source.clip = lowhit;
            source.Play();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentInputKey = KeyCode.RightArrow;
            source.clip = midhit;
            source.Play();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentInputKey = KeyCode.LeftArrow;
            source.clip = midhit;
            source.Play();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentInputKey = KeyCode.UpArrow;
            source.clip = highhit;
            source.Play();
        }
        return currentInputKey;
    }
    private void ApplyEnemySpriteHit()
    {
        currentEnemy.GetComponent<SpriteRenderer>().color = Color.red;
        if (isEnemyRight)
        {
            gameObject.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            gameObject.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy") && !collidedEnemies.Contains(collision.gameObject.GetComponent<EnemyController>()))
        {
            source.clip = enemyCollision;
            source.Play();
            
            Vector2 hitPoint = collision.contacts[0].point;

            Vector2 localHit = transform.InverseTransformDirection(hitPoint);

            if (localHit.x > 0)
            {
                isEnemyRight = true;
            }
            else
            {
                isEnemyRight = false;
            }

            collidedEnemies.Add(collision.gameObject.GetComponent<EnemyController>());
            currentEnemy = collision.gameObject.GetComponent<EnemyController>();
            comboSequence = collision.gameObject.GetComponent<EnemyController>().ComboSequence; // accesses the combo sequence of the enemy 
        }

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private int highAttackDamage = 3;
    [SerializeField] private float highAttackCooldown = 0f;
    [SerializeField] private int medAttackDamage = 2;
    [SerializeField] private int lowAttackDamage = 1;

    private KeyCode[] comboSequence = { KeyCode.UpArrow, KeyCode.None, KeyCode.DownArrow};
    private int currentComboIndex = 0;
    [SerializeField] private float comboTime = 0.5f;
    private float comboTimer = 0;

    private float lastAttackTime = -Mathf.Infinity;

    private List<EnemyController> collidedEnemies = new List<EnemyController>();
    private EnemyController currentEnemy;

    private bool isRight = false;
    private bool isEnemyRight = false;
    private bool canHit = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            isRight = true;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            isRight = false;
        }

        collidedEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy); // stack overflow code lol, cleans up all enemies once they are destroyed

        if (isRight && isEnemyRight)
        {
            canHit = true;
        } else if (!isRight && !isEnemyRight)
        {
            canHit = true;
        } else
        {
            Debug.Log($"Player {isRight} but the enemy {isEnemyRight}");
            canHit = false;
        }

        if (collidedEnemies.Count > 0)
        {
            currentEnemy = collidedEnemies[0];
        }
        else
        {
            if (currentEnemy != null)
            {
            }
            currentEnemy = null;
        }

        if (currentEnemy == null && currentComboIndex > 0)
        {
            ResetCombo();
        }

        ComboDetectionFunction();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy") && !collidedEnemies.Contains(collision.gameObject.GetComponent<EnemyController>()))
        {
            Vector2 hitPoint = collision.contacts[0].point;

            Vector2 localHit = transform.InverseTransformDirection(hitPoint);

            if (localHit.x > 0)
            {
                isEnemyRight = true;
            } else
            {
                isEnemyRight = false;
            }

            Debug.Log(isEnemyRight);
            collidedEnemies.Add(collision.gameObject.GetComponent<EnemyController>());
            comboSequence = collision.gameObject.GetComponent<EnemyController>().ComboSequence; // accesses the combo sequence of the enemy 
        }
        
    }
    private void ComboDetectionFunction() 
    {
        Color defaultColor = Color.white;
        if (currentEnemy != null)
        {
            defaultColor = currentEnemy.GetComponent<SpriteRenderer>().color;
        }

            if (currentComboIndex > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                Debug.Log("Combo failed! Time ran out.");
                currentEnemy.GetComponent<SpriteRenderer>().color = defaultColor;
                ResetCombo();
            }
        }

        if (currentEnemy != null && Time.time >= lastAttackTime + highAttackCooldown)
        {
            KeyCode currentInputKey = KeyCode.None;

            if (Input.GetKeyDown(KeyCode.DownArrow)) currentInputKey = KeyCode.DownArrow;
            else if (Input.GetKeyDown(KeyCode.None)) currentInputKey = KeyCode.None;
            else if (Input.GetKeyDown(KeyCode.UpArrow)) currentInputKey = KeyCode.UpArrow;


                KeyCode requiredKey = KeyCode.None;
                if (currentComboIndex < comboSequence.Length)
                {
                    requiredKey = comboSequence[currentComboIndex];
                }

                if (currentInputKey == requiredKey && canHit)
                {
                    Debug.Log("Correct combo input: " + requiredKey);
                    currentComboIndex++;
                    comboTimer = comboTime;
                    lastAttackTime = Time.time;

                    currentEnemy.GetComponent<SpriteRenderer>().color = Color.red;

                    if (currentComboIndex == comboSequence.Length)
                    {
                        Debug.Log("Full combo executed! Killing enemy");
                        currentEnemy.KillEnemy();
                        lastAttackTime = Time.time;
                        ScoreManager.Instance.AddToScore(50);
                        ResetCombo();
                    }
                }
        }
    }

    private void ResetCombo()
    {
        currentComboIndex = 0;
        comboTimer = 0f;
        Debug.Log("Combo reset.");
    }
}
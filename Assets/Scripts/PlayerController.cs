using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private int highAttackDamage = 3;
    [SerializeField] private float highAttackCooldown = 0f;
    [SerializeField] private int medAttackDamage = 2;
    [SerializeField] private int lowAttackDamage = 1;

    private string[] comboSequence1 = { "Alpha1", "Alpha2", "Alpha3" };
    private int currentComboIndex = 0;
    [SerializeField] private float comboTime = 0.5f;
    private float comboTimer = 0;

    private float lastAttackTime = -Mathf.Infinity;

    private List<EnemyController> collidedEnemies = new List<EnemyController>();
    private EnemyController currentEnemy;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Move right");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Move left");
        }

        if (currentComboIndex > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                Debug.Log("Combo failed! Time ran out.");
                ResetCombo();
            }
        }

        collidedEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy); // stack overflow code lol, cleans up all enemies once they are destroyed

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

        if (currentEnemy != null && Time.time >= lastAttackTime + highAttackCooldown)
        {
            KeyCode currentInputKey = KeyCode.None;
            
            if (Input.GetKeyDown(KeyCode.Alpha1)) currentInputKey = KeyCode.Alpha1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) currentInputKey = KeyCode.Alpha2;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) currentInputKey = KeyCode.Alpha3;

            if (currentInputKey != KeyCode.None)
            {
                KeyCode requiredKey = KeyCode.None;
                if (currentComboIndex < comboSequence1.Length)
                {
                    requiredKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), comboSequence1[currentComboIndex]);
                }

                if (currentInputKey == requiredKey)
                {
                    Debug.Log("Correct combo input: " + requiredKey);
                    currentComboIndex++;
                    comboTimer = comboTime;
                    lastAttackTime = Time.time;

                    if (currentComboIndex == comboSequence1.Length)
                    {
                        Debug.Log("Full combo executed! Killing enemy");
                        currentEnemy.KillEnemy();
                        lastAttackTime = Time.time;
                        ScoreManager.Instance.AddToScore(50);
                        ResetCombo();
                    }
                }
                else
                {
                    Debug.Log("combo failed");
                    ResetCombo();
                    ScoreManager.Instance.AddToScore(-5);
                }
            }
        } else
        {
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy") && !collidedEnemies.Contains(collision.gameObject.GetComponent<EnemyController>()))
        {
            collidedEnemies.Add(collision.gameObject.GetComponent<EnemyController>());
        }
        
    }

    private void ResetCombo()
    {
        currentComboIndex = 0;
        comboTimer = 0f;
        Debug.Log("Combo reset.");
    }
}
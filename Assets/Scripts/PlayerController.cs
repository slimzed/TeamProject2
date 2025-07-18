using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0f;
    [SerializeField] private float hitStopDuration = 0.5f;

    private AnimationCurve hitstopCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // used for hit stop, not implemented yet
    private Color enemyDetectionColor = Color.blue;
    [SerializeField] private float tintLevel = 0.2f;


    private KeyCode[] comboSequence = { KeyCode.UpArrow, KeyCode.None, KeyCode.DownArrow};
    private int currentComboIndex = 0;
    [SerializeField] private float comboTime = 0.5f;
    private float comboTimer = 0;

    private float lastAttackTime = -Mathf.Infinity;

    private List<EnemyController> collidedEnemies = new List<EnemyController>();
    private EnemyController currentEnemy;

    private bool isRight = false;
    private bool isEnemyRight = false;
    private bool isHitStop = false; // used for hit stop, not implemented yet


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

        CleanUpEnemies();

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
    private void CleanUpEnemies()
    {
        // Remove null or inactive enemies
        collidedEnemies.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);

        // Handle overflow
        if (collidedEnemies.Count > 4)
        {
            if (collidedEnemies[0] != null)
            {
                collidedEnemies[0].KillEnemy(false);
            }
            collidedEnemies.RemoveAt(0);
            ScoreManager.Instance.AddToScore(-50);
        }

        // Update current enemy reference
        if (currentEnemy == null && collidedEnemies.Count > 0)
        {
            currentEnemy = collidedEnemies[^1]; // Using index from end operator
            SetTint(currentEnemy.GetComponent<SpriteRenderer>().material, true);
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
                ResetInputArrow(currentEnemy.gameObject);
                source.clip = failure;
                source.Play();
                ResetCombo();
            }
        }

        if (currentEnemy != null && Time.time >= lastAttackTime + attackCooldown)
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
                    RemoveInputArrow(currentEnemy.gameObject);

                    if (currentComboIndex == comboSequence.Length)
                    {
                        Debug.Log("Full combo executed! Killing enemy");
                        currentEnemy.KillEnemy(true);

                        if (collidedEnemies.Count > 1)
                        {
                            currentEnemy = collidedEnemies[collidedEnemies.Count-2];
                            ResetInputArrow(currentEnemy.gameObject);
                            SetTint(currentEnemy.GetComponent<SpriteRenderer>().material, true);
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
                    ResetInputArrow(currentEnemy.gameObject);
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
            StartCoroutine(HitStopCoroutine());
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentInputKey = KeyCode.RightArrow;
            source.clip = midhit;
            source.Play();
            StartCoroutine(HitStopCoroutine());
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentInputKey = KeyCode.LeftArrow;
            source.clip = midhit;
            source.Play();
            StartCoroutine(HitStopCoroutine());
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentInputKey = KeyCode.UpArrow;
            source.clip = highhit;
            source.Play();
            StartCoroutine(HitStopCoroutine());
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

            if (collidedEnemies.Count > 1)
            {
                SetTint(collidedEnemies[collidedEnemies.Count - 2].GetComponent<SpriteRenderer>().material, false); // disables the emission of the previous enemy
                SetTint(currentEnemy.GetComponent<SpriteRenderer>().material, true); // enables the emission of the current enemy
            } else
            {
                SetTint(currentEnemy.GetComponent<SpriteRenderer>().material, true); // enables the emission of the current enemy
            }



                comboSequence = collision.gameObject.GetComponent<EnemyController>().ComboSequence; // accesses the combo sequence of the enemy 
        }

    }
    private IEnumerator HitStopCoroutine()
    {
        if (isHitStop) yield break;
        isHitStop = true;

        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0.1f;

        yield return new WaitForSecondsRealtime(hitStopDuration);

        float timer = 0f;
        while (timer < hitStopDuration)
        {
            float t = hitstopCurve.Evaluate(timer / hitStopDuration);
            Time.timeScale = Mathf.Lerp(Time.timeScale, originalTimeScale, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = originalTimeScale;
        isHitStop = false;
    }
    private void SetTint(Material material, bool enable)
    {
        if (material == null) return;
        if (enable)
        {
            material.color = Color.Lerp(material.color, enemyDetectionColor, tintLevel);
        } else
        {
            material.color = Color.white;
        }

    }

    private void RemoveInputArrow(GameObject parentArrow)
    {
        foreach (Transform arrow in parentArrow.transform)
        {
            if (arrow.CompareTag("InputArrow") && arrow.gameObject.activeInHierarchy)
            {
                arrow.gameObject.SetActive(false);
                break;
            } else
            {
                continue;
            }
        }
    }
    private void ResetInputArrow(GameObject parentArrow)
    {
        foreach (Transform arrow in parentArrow.transform)
        {
            if (arrow.CompareTag("InputArrow") && !arrow.gameObject.activeInHierarchy)
            {
                arrow.gameObject.SetActive(true);
            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0f;
    [SerializeField] private GameObject targetingIndicator;


    [SerializeField] private float hitStopDuration = 0.5f;
    private AnimationCurve hitstopCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);



    private KeyCode[] comboSequence = { KeyCode.UpArrow, KeyCode.None, KeyCode.DownArrow};
    private int currentComboIndex = 0;
    private float comboTimer = 0; // internal timer to track how long the combo has been active
    [Tooltip("This determines how long the combo will last after the first input")]
    [SerializeField] private float comboTime = 0.5f; // decides how long the combo will last after the first input
    [Tooltip("This determines how long after the beat the player has to input the combo")]


    private float lastAttackTime = -Mathf.Infinity; // input track for debounce

    private List<EnemyController> collidedEnemies = new List<EnemyController>();
    private EnemyController currentEnemy;

    private bool isRight = false;
    private bool isEnemyRight = false;
    private bool isHitStop = false;
    private bool canCombo = false;


    /// <summary>
    /// The following variables refer to Audio/Script calls.
    /// </summary>
    private GameObject ScriptCalls;
    private GameObject BeatIndicatorOnPlayer;
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
        BeatIndicatorOnPlayer = gameObject.transform.Find("BeatIndicator").gameObject;
        BeatIndicatorOnPlayer.GetComponent<SpriteRenderer>().color = Color.red;
        AudioManager.OnBeat += HandleBeat;

    }

    void Update()
    {
        ApplyPlayerSpriteDirection();

        CleanUpEnemies();

        ComboDetectionFunction();
    }

    /// <summary>
    /// The following functions handle beat detection, flipping a bool on and off based around a combo detection window.
    /// </summary>
    /// <param name="beatNumber"></param>
    /// <param name="isFirstSpawner"></param>
    private void HandleBeat(int beatNumber, bool isFirstSpawner, float beatTimeDifference)
    {
        Debug.Log($"Beat {beatNumber}");
        BeatIndicatorOnPlayer.GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(BeatComboDetectionRoutine(beatTimeDifference/2));

    }
    private IEnumerator BeatComboDetectionRoutine(float beatWindow)
    {
        canCombo = true;

        yield return new WaitForSeconds(beatWindow);
        BeatIndicatorOnPlayer.GetComponent<SpriteRenderer>().color = Color.red; // resets the color of the beat indicator
        canCombo = false;
        Debug.Log("combo detection ended");

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
            TurnOnTarget(currentEnemy.transform, true);
        }
    }
    private void ComboDetectionFunction() 
    {



        if (currentEnemy != null && Time.time >= lastAttackTime + attackCooldown) // the enemy exists and the cd is over 
        {
            KeyCode currentInputKey = CheckAndAnimateInput();

                if (currentInputKey != KeyCode.None)
                {
                    KeyCode requiredKey = KeyCode.None;
                    if (currentComboIndex < comboSequence.Length)
                    {
                        requiredKey = comboSequence[currentComboIndex];
                    }

                    if (currentInputKey == requiredKey) // checks if input is correct and within the detection time window
                    {
                        if (canCombo)
                        {
                            Debug.Log("Correct combo input: " + requiredKey);
                            currentComboIndex++;
                            comboTimer = comboTime;
                            lastAttackTime = Time.time;
                            ApplyEnemySpriteHit();
                            RemoveInputArrow(currentEnemy.gameObject);
                            ScoreManager.Instance.IncreaseCombo();
                        } else
                        {
                            Debug.Log("combo input outside of beat window, nothing happens though");
                            ScoreManager.Instance.AddToScore(-20); // adding an arbitrary penalty for inputting outside of window
                        }


                        if (currentComboIndex == comboSequence.Length)
                    {
                        Debug.Log("Full combo executed! Killing enemy");
                        ScoreManager.Instance.AddToScore(currentEnemy.scoreValue);
                        currentEnemy.KillEnemy(false); // we are already added passing in the added scoreValue into ScoreManager, where it is multiplied by the combo multiplier

                        if (collidedEnemies.Count > 1)
                        {
                            currentEnemy = collidedEnemies[collidedEnemies.Count - 2];
                            ResetInputArrow(currentEnemy.gameObject);
                            TurnOnTarget(currentEnemy.transform, true); // targets the next enemy in the list of collided enemies 
                        }
                        else
                        {
                            currentEnemy = null;
                        }



                        lastAttackTime = Time.time;
                        ResetCombo();
                    }
                    }
                    else
                    {
                        Debug.Log("incorrect input");
                        ScoreManager.Instance.AddToScore(currentEnemy.scoreValue * -1);
                        
                        ResetInputArrow(currentEnemy.gameObject);
                        ScoreManager.Instance.ResetCombo();
                        ResetCombo();
                        
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
        if (collision.transform.CompareTag("Enemy"))
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
                TurnOnTarget(collidedEnemies[collidedEnemies.Count - 2].transform, false); // disables the emission of the previous enemy
                TurnOnTarget(currentEnemy.transform, true); // enables the emission of the current enemy
            } else
            {
                TurnOnTarget(currentEnemy.transform, true); // enables the emission of the current enemy
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
    private void TurnOnTarget(Transform parent, bool enable)
    {
        // basically im just looping through all children of the parent and checking their tags so that i can change their sprite color
        if (parent == null) return;
        if (enable)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag("Target"))
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag("Target"))
                {
                    child.gameObject.SetActive(false);
                }
            }
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
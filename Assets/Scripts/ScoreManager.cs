using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public static Action OnGameOver;
    public static Action OnGameVictory; 

    private int _score = 250;
    private int lives = 15;

    public int LevelNumber { get; private set; } = 1;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private Animator comboAnimator;


    // combo handling
    [SerializeField] private float comboResetTime = 3f;
    [SerializeField] private int maxCombo = 10;
    [SerializeField] private float comboScoreMultiplier = 0.5f;

    private int currentCombo = 0;
    private float comboResetTimer = 0f;
    private bool isComboActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        UpdateUI();
        
        AudioManager.OnGameVictory += ResetAllStats; // Reset combo on game victory
        AudioManager.OnGameVictory += HandleGameVictory; // Handle game victory
    }
    private void Update()
    {
        if (isComboActive)
        {
            comboResetTimer -= Time.deltaTime;
            if (comboResetTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    public void IncreaseCombo()
    {
        currentCombo++;
        currentCombo = Mathf.Clamp(currentCombo, currentCombo, maxCombo);
        comboResetTimer = comboResetTime;
        isComboActive = true;
        PlayComboAnimation();

        if (currentCombo == 1)
        {
            Debug.Log($"[ComboSystem] COMBO STARTED! First kill");
        }
        else
        {
            Debug.Log($"[ComboSystem] COMBO INCREASED! Now at {currentCombo} kills");

            // Special messages at certain milestones
            if (currentCombo == 3)
            {
                Debug.Log("[ComboSystem] NICE! 3-kill combo!");
            }
            else if (currentCombo == 5)
            {
                Debug.Log("[ComboSystem] GREAT! 5-kill combo!");
            }
            else if (currentCombo == maxCombo)
            {
                Debug.Log($"[ComboSystem] AMAZING! MAX COMBO ({maxCombo}) ACHIEVED!");
            }
        }

        Debug.Log($"[ComboSystem] Current multiplier: {GetScoreMultiplier():F1}x");
        UpdateUI();
    }

    public void ResetCombo()
    {
        if (isComboActive)
        {
            if (currentCombo > 0)
            {
                Debug.Log($"[ComboSystem] COMBO ENDED! Final streak: {currentCombo} kills");

                if (currentCombo >= 5)
                {
                    Debug.Log("[ComboSystem] Impressive streak!");
                }
            }
            else
            {
                Debug.Log("[ComboSystem] Combo reset (no kills)");
            }
        }

        currentCombo = 0;
        isComboActive = false;
    }

    public int GetCurrentCombo()
    {
        return currentCombo;
    }

    public float GetScoreMultiplier()
    {
        return 1f + (comboScoreMultiplier * currentCombo);
    }

    public int Score
    {
        get
        {
            return _score;
        }
        private set
        {
            _score = value;
            UpdateUI();
        }
    }

    public void AddToScore(int score)
    {
        int comboScore = score * (int) GetScoreMultiplier();
        if (score < 0)
        {
            Score += score;
        } else
        {
            Score += comboScore;
        }


        if (Score <= 0)
        {
            Score = 0;
            OnGameOver?.Invoke();
        } else if (Score >= 6500)
        {
            OnGameVictory?.Invoke();
        }
    }

    private void UpdateUI()
    {
        CheckText();
        scoreText.text = "Score: " + Score.ToString();
        comboText.text = "Combo: " + currentCombo.ToString();
        livesText.text = "Lives: " + lives.ToString();  
    }
    private void CheckText()
    {
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            if (scoreText == null)
            {
                Debug.LogError("ScoreText not found in the scene, please create a TextMeshPro element with the name 'ScoreText'");
            }
        }
        if (comboText == null)
        {
            comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
            if (comboText == null)
            {
                Debug.LogError("ComboText not found in the scene, please create a TextMeshPro element with the name 'ComboText'");
            }
        }
        if (livesText == null)
        {
            livesText = GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>();
            if (livesText == null)
            {
                Debug.LogError("LivesText not found in the scene, please create a TextMeshPro element with the name 'LivesText'");
            }
        }
    }
    private void ResetAllStats()
    {
        Score = 250; // Reset score to initial value
        lives = 15;
        ResetCombo(); // Reset combo
        UpdateUI(); // Update UI elements
    }
    private void HandleGameVictory()
    {
        Debug.Log(LevelNumber);
        LevelNumber += 1;
        Debug.Log(LevelNumber);
    }
    private void PlayComboAnimation()
    {
        if (comboAnimator == null) return;
        comboAnimator.SetTrigger("Pulse");
    }


    public void SubtractLives()
    {
        Debug.Log("lives subtracted");
        lives--;
        if (lives <= 0)
        {
            OnGameOver?.Invoke();
            LevelNumber = 1;
            ResetAllStats();
        }
        UpdateUI();    
    }
}

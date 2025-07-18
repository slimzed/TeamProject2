using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public static Action OnGameOver;
    private int _score = 250;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
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
            Destroy(Instance);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        UpdateUI();
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
        score *= (int) GetScoreMultiplier();
        Debug.Log(score);
        Score += score;
        if (Score <= 0)
        {
            Score = 0;
            OnGameOver?.Invoke();
        }
    }

    private void UpdateUI()
    {
        CheckText();
        scoreText.text = "Score: " + Score.ToString();
        comboText.text = "Combo: " + currentCombo.ToString();
    }
    private void CheckText()
    {
        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        }
        if (comboText == null)
        {
            comboText = GameObject.Find("ComboText").GetComponent<TextMeshProUGUI>();
        }
    }
    private void PlayComboAnimation()
    {
        if (comboAnimator == null) return;
        comboAnimator.SetTrigger("Pulse");
    }
}

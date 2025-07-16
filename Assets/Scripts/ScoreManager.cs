using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int scoreValue;
    public int Score
    {
        get
        {
            return scoreValue;
        }
        private set
        {
            scoreValue = value;
            UpdateUI();
        }
    }

    [SerializeField] private TextMeshProUGUI scoreText;
    
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        UpdateUI();
    }

    public void AddToScore(int score)
    {
        Score += score;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + Score.ToString();
    }
}

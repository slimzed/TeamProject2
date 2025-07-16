using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int Score = 0;
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
    public int GetScore()
    {
        return Score;
    }
    private void UpdateUI()
    {
        scoreText.text = "Score: " + Score.ToString();
    }
}

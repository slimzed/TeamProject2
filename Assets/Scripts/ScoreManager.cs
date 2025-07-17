using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int _score = 1000;
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

    [SerializeField] private TextMeshProUGUI scoreText;
    
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
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + Score.ToString();
    }
}

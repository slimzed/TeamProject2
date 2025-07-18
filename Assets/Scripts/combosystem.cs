using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTime = 3f;
    [SerializeField] private int maxCombo = 10;
    [SerializeField] private float comboScoreMultiplier = 0.5f;

    private int currentCombo = 0;
    private float comboResetTimer = 0f;
    private bool isComboActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[ComboSystem] Combo System initialized");
        }
        else
        {
            Destroy(gameObject);
        }
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
        currentCombo = Mathf.Min(currentCombo + 1, maxCombo);
        comboResetTimer = comboResetTime;
        isComboActive = true;

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
}
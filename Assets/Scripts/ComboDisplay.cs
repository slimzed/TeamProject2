using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// This script is unused currently. It serves as a display for the combo system implemented within the ScoreManager singleton. 
/// </summary>
public class ComboDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Animator comboAnimator;

    private void OnEnable()
    {
        ScoreManager.Instance.ResetCombo();
        UpdateComboDisplay(0);
    }

    private void Start()
    {
        ScoreManager.Instance.ResetCombo();
        UpdateComboDisplay(0);
    }

    private void Update()
    {
        int currentCombo = ScoreManager.Instance.GetCurrentCombo();
        UpdateComboDisplay(currentCombo);
    }

    private void UpdateComboDisplay(int combo)
    {
        if (combo > 0)
        {
            comboText.text = $"{combo} HIT COMBO!";
            comboAnimator.SetTrigger("Pulse");
        }
        else
        {
            comboText.text = "";
        }
    }
}
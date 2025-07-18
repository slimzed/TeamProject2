using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject VictoryText;
    [SerializeField] private GameObject VictoryButton;
    private Transform UICanvasParent;

    private void Start()
    {
        UICanvasParent = GameObject.FindObjectOfType<Canvas>().transform;
        AudioManager.OnGameVictory += ShowVictoryUI;
        DontDestroyOnLoad(gameObject);
    }
    private void ShowVictoryUI()
    {
        GameObject victoryText = Instantiate(VictoryText, UICanvasParent.transform.position, Quaternion.identity);
        victoryText.GetComponent<RectTransform>().anchoredPosition = new Vector2(1075, 650);
        victoryText.transform.SetParent(UICanvasParent);

        GameObject victoryButton = Instantiate(VictoryButton, UICanvasParent.transform.position, Quaternion.identity);
        victoryButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, 550);
        victoryButton.transform.SetParent(UICanvasParent);  
    }
}

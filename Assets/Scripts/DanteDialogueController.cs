using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DanteDialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    public string[] dialogueLines;
    public float typingSpeed;
    private int currentLineIndex = 0;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == dialogueLines[currentLineIndex]) // check if the text component is the same as the current line in the dialogue lines
            {
                NextLine();
            } else // if clicked again, then just fill out the line fully 
            {
                StopAllCoroutines();
                textComponent.text = dialogueLines[currentLineIndex]; // if not, show the full line immediately
            }
        }
    }
    void StartDialogue()
    {
        currentLineIndex = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // type out the characters
        foreach(char c in dialogueLines[currentLineIndex].ToCharArray()) // iterate through each character in the line and append it to the text component 
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }


    void NextLine()
    {
        if (currentLineIndex < dialogueLines.Length - 1)
        {
            currentLineIndex++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        } else
        {
            gameObject.SetActive(false);
        }
    }
}

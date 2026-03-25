using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    public static TextManager instance;
    public GameObject textBoxObject;
    public TextMeshProUGUI textDisplay;
    
    private Queue<string> sentences = new Queue<string>();

    void Awake()
    {
        if (instance == null) instance = this;
        textBoxObject.SetActive(false);
    }

    public void ShowMessage(string[] lines)
    {
        sentences.Clear();
        foreach (string line in lines) sentences.Enqueue(line);
        
        textBoxObject.SetActive(true);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            textBoxObject.SetActive(false);
            return;
        }

        textDisplay.text = sentences.Dequeue();
    }

    public bool IsDisplayingText() => textBoxObject.activeSelf;
}
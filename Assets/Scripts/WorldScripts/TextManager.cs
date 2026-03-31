using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    public static TextManager instance;
    public GameObject textBoxObject;
    public TextMeshProUGUI textDisplay;
    public GameObject popupObject;
    public TextMeshProUGUI popupTextDisplay;
    
    private Queue<string> sentences = new Queue<string>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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

    public void ShowPopup(string message)
    {
        popupTextDisplay.text = message;

        popupObject.SetActive(true);
    }

    public bool IsDisplayingPopup() => popupObject.activeSelf;
    public void HidePopup()
    {
        popupObject.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance { private set; get; }

    [HideInInspector] public bool isInPlay;

    [HideInInspector] public UnityEvent onIncorrectLetter;
    [HideInInspector] public UnityEvent onCorrectLetter;
    [HideInInspector] public UnityEvent onCompleteWord;

    [HideInInspector] public string currentWord;
    [HideInInspector] public string remainingString;
    [HideInInspector] public string completedString;

    [Tooltip("Text file containing all the possible words. " +
        "Each line should be a single unique word.")]
    [SerializeField] private TextAsset wordBank;
    [SerializeField] private Color completedStringColor = Color.yellow;
    [SerializeField] private Color finishedWordColor = Color.green;

    [SerializeField] private References references;

    private List<string> availableWords;
    private List<string> usedWords;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this; 
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // setup UnityEvents
        onIncorrectLetter = new UnityEvent();
        onCorrectLetter = new UnityEvent();
        onCompleteWord = new UnityEvent();

        isInPlay = true;
    }

    private void Start()
    {
        ProcessWordBank();
        SetNewWord();
    }

    private void Update()
    {
        CheckInput();
    }

    private void UpdateText()
    {
        references.wordText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(completedStringColor) + ">"
            + completedString + "</color>" + remainingString;
    }

    private void InputChar(char c)
    {
        if (remainingString.Length > 0 && remainingString[0] == c)
        {
            completedString += remainingString[0];
            remainingString = remainingString.Substring(1);
            UpdateText();
            onCorrectLetter.Invoke();
        }
        else
        {
            onIncorrectLetter.Invoke();
        }

        if (remainingString.Length == 0)
        {
            references.wordText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(finishedWordColor) + ">"
                + completedString + "</color>";
            onCompleteWord.Invoke();
        }
    }

    /// <summary>
    /// Parse the word bank and store the words in a list.
    /// </summary>
    private void ProcessWordBank()
    {
        // each line in the wordBank is a new and unique word
        string[] words = wordBank.text.Split('\n');
        availableWords = new List<string>(words);

        // remove any "invisible" chars
        for (int i = 0; i < availableWords.Count; i++)
        {
            availableWords[i] = availableWords[i].Replace("\r", string.Empty);
        }

        usedWords = new List<string>();
    }

    private string GetNewWord()
    {
        // if there are no more words, reset the list
        if (availableWords.Count == 0)
        {
            availableWords = new List<string>(usedWords);
            usedWords.Clear();
        }

        // word from the available list at random
        int index = Random.Range(0, availableWords.Count);
        string word = availableWords[index];
        availableWords.RemoveAt(index);
        usedWords.Add(word);

        return word;
    }

    private void SetNewWord()
    {
        currentWord = GetNewWord();
        remainingString = currentWord;
        completedString = string.Empty;
        references.wordText.text = currentWord;
    }

    private void CheckInput()
    {
        if (Input.anyKeyDown)
        {
            foreach (char letter in Input.inputString)
            {
                InputChar(letter);
            }
        }
    }

    [System.Serializable]
    public struct References
    {
        public Canvas canvas;
        public TMP_Text wordText;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum ControlScheme
{
    Normal,
    Switched
}

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance { private set; get; }

    [HideInInspector] public bool IsInPlay;
    [HideInInspector] public ControlScheme CurrentControls;

    [HideInInspector] public UnityEvent OnIncorrectLetter;
    [HideInInspector] public UnityEvent OnCorrectLetter;
    [HideInInspector] public UnityEvent OnCompleteWord;

    [HideInInspector] public UnityEvent OnCompleteCourse;
    [HideInInspector] public UnityEvent OnFailCourse;

    [HideInInspector] public string CurrentWord;
    [HideInInspector] public string RemainingString;
    [HideInInspector] public string CompletedString;

    [Tooltip("Text file containing all the possible words. " +
        "Each line should be a single unique word.")]
    [SerializeField] private TextAsset wordBank;
    [SerializeField] private Color completedStringColor = Color.yellow;
    [SerializeField] private Color finishedWordColor = Color.green;

    [Space(5)]

    [Header("Player Ball Settings")]
    [Range(0f, 100f)]
    public float speed = 10f;

    [Space(10)]

    [SerializeField] private ObstacleCourses courses;
    [SerializeField] private References references;

    private List<string> availableWords;
    private List<string> usedWords;

    private bool isWordComplete;
    private bool isCourseComplete;

    private int score;

    public void GameOver()
    {
        IsInPlay = false;
    }

    public void SetupNewLevel()
    {
        // new word
        isWordComplete = false;
        SetNewWord();
    }

    public void InputLetters(string letters)
    {
        foreach (char c in letters)
        {
            InputChar(c);
        }
    }

    private void Awake()
    {
        if (!Instance)
        { 
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }

        // setup UnityEvents
        OnIncorrectLetter = new();
        OnCorrectLetter = new();
        OnCompleteWord = new();

        OnCompleteCourse = new();
        OnFailCourse = new();

        IsInPlay = true;
        CurrentControls = ControlScheme.Switched;
    }

    private void Start()
    {
        ProcessWordBank();
        SetNewWord();

        OnFailCourse.AddListener(GameOver);
        OnIncorrectLetter.AddListener(GameOver);

        OnCompleteWord.AddListener(() => isWordComplete = true);
        OnCompleteCourse.AddListener(() => isCourseComplete = true);
    }

    private void Update()
    {
        switch (CurrentControls)
        {
            case ControlScheme.Normal:
                CheckInput();
                break;
            case ControlScheme.Switched:
                MoveBall(Time.deltaTime);
                break;
        }

        if (isWordComplete && isCourseComplete)
        {
            CompleteLevel();
        }
    }

    private void UpdateText()
    {
        references.wordText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(completedStringColor) + ">"
            + CompletedString + "</color>" + RemainingString;
    }

    private void CompleteLevel()
    {
        score++;

        SetupNewLevel();
    }

    private void InputChar(char c)
    {
        if (RemainingString.Length > 0 && RemainingString[0] == c)
        {
            CompletedString += RemainingString[0];
            RemainingString = RemainingString.Substring(1);
            UpdateText();
            OnCorrectLetter.Invoke();
        }
        else
        {
            OnIncorrectLetter.Invoke();
        }

        if (RemainingString.Length == 0)
        {
            references.wordText.text = "<color=#" + ColorUtility.ToHtmlStringRGB(finishedWordColor) + ">"
                + CompletedString + "</color>";
            OnCompleteWord.Invoke();
        }
    }

    /// <summary>
    /// Parse the word bank and store the words in a list.
    /// </summary>
    private void ProcessWordBank()
    {
        // each line in the wordBank is a new and unique word
        string[] words = wordBank.text.Split('\n');
        availableWords = new(words);

        // remove any "invisible" chars
        for (int i = 0; i < availableWords.Count; i++)
        {
            availableWords[i] = availableWords[i].Replace("\r", string.Empty);
        }

        usedWords = new();
    }

    private string GetNewWord()
    {
        // if there are no more words, reset the list
        if (availableWords.Count == 0)
        {
            availableWords = new(usedWords);
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
        CurrentWord = GetNewWord();
        RemainingString = CurrentWord;
        CompletedString = string.Empty;
        references.wordText.text = CurrentWord;
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

    private void MoveBall(float deltaTime)
    {
        // move the player ball based on horizontal and vertical input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new(horizontal, vertical, 0);
        references.playerBall.Translate(deltaTime * speed * direction);
    }

    [System.Serializable]
    public struct References
    {
        [Header("UI")]
        public Canvas canvas;
        public TMP_Text wordText;
        public GameObject onScreenKeyboard;

        [Space(5)]

        [Header("Player")]
        public Transform playerBall;

        [Space(5)]

        [Header("Obstacles")]
        public Transform topGate;
        public Transform bottomGate;
    }

    [System.Serializable]
    public struct ObstacleCourses
    {
        [HideInInspector] public bool IsReversed;

        public GameObject[] normalCourses;
    }
}

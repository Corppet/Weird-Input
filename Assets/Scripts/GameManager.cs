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

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance { private set; get; }

    [HideInInspector] public bool IsInPlay;
    private ControlScheme _currentControls;
    [HideInInspector] public ControlScheme CurrentControls
    {
        set
        {
            switch (value)
            {
                case ControlScheme.Normal:
                    references.onScreenKeyboard.SetActive(false);
                    break;
                case ControlScheme.Switched:
                    references.onScreenKeyboard.SetActive(true);
                    break;
            }
            _currentControls = value;
        }

        get { return _currentControls; }
    }
    [HideInInspector] public Difficulty CurrentDifficulty;
    private int _score;
    [HideInInspector] public int Score
    {
        private set
        {
            foreach (TMP_Text text in references.scoreTexts)
            {
                text.text = value.ToString();
            }
            _score = value;
        }

        get { return _score; }
    }

    [HideInInspector] public UnityEvent OnIncorrectLetter;
    [HideInInspector] public UnityEvent OnCorrectLetter;
    [HideInInspector] public UnityEvent OnCompleteWord;

    [HideInInspector] public UnityEvent OnCompleteCourse;
    [HideInInspector] public UnityEvent OnFailCourse;

    [HideInInspector] public string CurrentWord;
    [HideInInspector] public string RemainingString;
    [HideInInspector] public string CompletedString;

    [Header("Word Settings")]
    [Tooltip("Text file containing all the possible words. " +
        "Each line should be a single unique word.")]
    [SerializeField] private TextAsset wordBank;
    [Range(0, 10)]
    [SerializeField] private int easyMaxLength = 5;
    [Range(0, 20)]
    [SerializeField] private int mediumMaxLength = 8;
    [SerializeField] private Color completedStringColor = Color.yellow;
    [SerializeField] private Color finishedWordColor = Color.green;

    [Space(5)]

    [Header("Player Settings")]
    [Range(0f, 100f)]
    public float ballSpeed = 10f;
    [Range(0f, 1f)]
    public float switchedControlsRate = .5f;

    [Space(10)]

    [SerializeField] private ObstacleCourses courses;
    [SerializeField] private References references;

    private List<string> availableWords;
    private List<string> usedWords;

    private bool isWordComplete;
    private bool isCourseComplete;

    public void GameOver()
    {
        IsInPlay = false;
        CurrentControls = ControlScheme.Normal;

#if DEBUG
        Debug.Log("Game Over");
#endif
    }

    public void SetupNewLevel()
    {
        // new word
        isWordComplete = false;
        SetNewWord();

        // new obstacle course
        isCourseComplete = false;
        SetNewCourse();

        // attempt to switch controls
        if (Random.value < switchedControlsRate)
        {
            CurrentControls = ControlScheme.Switched;
        }
        else
        {
            CurrentControls = ControlScheme.Normal;
        }
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

        // initialize variables
        IsInPlay = true;
        CurrentControls = ControlScheme.Switched;
        CurrentDifficulty = Difficulty.Easy;
        Score = 0;
    }

    private void Start()
    {
        OnFailCourse.AddListener(GameOver);
        OnIncorrectLetter.AddListener(GameOver);

        OnCompleteWord.AddListener(() => isWordComplete = true);
        OnCompleteCourse.AddListener(() => isCourseComplete = true);

        // setup gates
        references.topGate.IsGoal = true;
        references.bottomGate.IsGoal = false;

        ProcessWordBank();
        SetupNewLevel();
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
        Score++;
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

#if DEBUG
        // debugging
        string longestWord = string.Empty;
        foreach (string word in availableWords)
        {
            if (word.Length <= longestWord.Length)
                continue;

            longestWord = word;
        }
        Debug.Log("Longest word: " + longestWord + "(" + longestWord.Length + ")");
#endif
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
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
                while (availableWords[index].Length > easyMaxLength)
                {
                    index = Random.Range(0, availableWords.Count);
                }
                break;
            case Difficulty.Medium:
                while (availableWords[index].Length > mediumMaxLength)
                {
                    index = Random.Range(0, availableWords.Count);
                }
                break;
        }
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
        references.playerBall.Translate(deltaTime * ballSpeed * direction);
    }

    private GameObject GetNewCourse(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                int index = Random.Range(0, courses.easyCourses.Length);
                return courses.easyCourses[index];
            case Difficulty.Medium:
                index = Random.Range(0, courses.mediumCourses.Length);
                return courses.mediumCourses[index];
            case Difficulty.Hard:
                index = Random.Range(0, courses.hardCourses.Length);
                return courses.hardCourses[index];
        }

        throw new System.ArgumentException("Invalid difficulty");
    }

    private void SetNewCourse()
    {
        Transform parent = references.obstacleParent;

        // remove current obstacle course
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        // get a random obstacle course
        GameObject newCourse = GetNewCourse((Difficulty)Random.Range(0, CurrentDifficulty.GetHashCode() + 1));
        Instantiate(newCourse, parent);

        // switch gates
        references.topGate.IsGoal = !references.topGate.IsGoal;
        references.bottomGate.IsGoal = !references.bottomGate.IsGoal;
    }

    [System.Serializable]
    public struct References
    {
        [Header("UI")]
        public Canvas canvas;
        public TMP_Text[] scoreTexts;
        public TMP_Text wordText;
        public GameObject onScreenKeyboard;

        [Space(5)]

        [Header("Player")]
        public Transform playerBall;

        [Space(5)]

        [Header("Obstacles")]
        public Transform obstacleParent;
        public Gate topGate;
        public Gate bottomGate;
    }

    [System.Serializable]
    public struct ObstacleCourses
    {
        [HideInInspector] public bool IsReversed;

        public GameObject[] easyCourses;
        public GameObject[] mediumCourses;
        public GameObject[] hardCourses;
    }
}

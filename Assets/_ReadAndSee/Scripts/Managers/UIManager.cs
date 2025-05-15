using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Quiz UI References")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] ImageOptionButton[] imageOptionButtons;
    [SerializeField] TextMeshProUGUI questionCounter;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI titleText;

    [Header("Quiz Panel References")]
    public RectTransform panel;

    private Vector2 hiddenPosition;
    private Vector2 centerPosition;

    [Header("Results UI References")]
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI incorrectAtmpTxt;
    [SerializeField] Transform starsContainer;
    [SerializeField] GameObject withStarPrefab;
    [SerializeField] GameObject noStarPrefab;
    [SerializeField] Image levelThumbnail;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "QuizGame")
        {
            InitializePanels();
            UpdateQuizCard();
        }
        else if (SceneManager.GetActiveScene().name == "Results")
        {
            ShowResults();
        }
    }

    public void InitializePanels()
    {
        centerPosition = Vector2.zero;
        float screenWidth = ((RectTransform)panel.parent).rect.width;
        hiddenPosition = new Vector2(screenWidth, 0);
        panel.anchoredPosition = hiddenPosition;
    }

    public void UpdateQuizCard()
    {
        Question currentQuestion = GameManager.Instance.GetCurrentQuestion();

        if (currentQuestion != null)
        {
            // Update question text
            questionText.text = currentQuestion.questionText;

            // Update options
            for (int i = 0; i < imageOptionButtons.Length; i++)
            {
                if (i < currentQuestion.imageOptions.Length)
                {
                    imageOptionButtons[i].gameObject.SetActive(true);
                    imageOptionButtons[i].Setup(currentQuestion.imageOptions[i], i);
                }
                else
                {
                    imageOptionButtons[i].gameObject.SetActive(false);
                }
            }

            // Update progress
            int currentIndex = GameManager.Instance.CurrentQuestionIndex + 1;
            int total = GameManager.Instance.TotalQuestions;
            questionCounter.text = $"Question {currentIndex}/{total}";

            // Update score
            titleText.text = GameManager.Instance.CurrentLevel;
            scoreText.text = $"Score: {GameManager.Instance.CorrectAnswers}";
        }
    }

    public void ShowQuizPanel()
    {
        LeanTween.move(panel, centerPosition, 0.5f).setEase(LeanTweenType.easeOutExpo);
    }

    private void ShowResults()
    {
        playerName.text = $"Player: {SaveManager.Instance.CurrentPlayerData.playerName}";
        incorrectAtmpTxt.text = $"Incorrect attempts: {GameManager.Instance.TotalQuestions - GameManager.Instance.CorrectAnswers}";
        levelThumbnail.sprite = QuizDatabase.Instance.GetLevel(GameManager.Instance.CurrentLevel).thumbnail;
        int stars = SaveManager.Instance.CurrentPlayerData.GetLevelStars(GameManager.Instance.CurrentLevel);
        DisplayStars(stars);
    }

    public void DisplayStars(int starCount)
    {
        foreach (Transform child in starsContainer) Destroy(child.gameObject);

        const int totalStars = 3;
        for (int i = 0; i < starCount && i < totalStars; i++) Instantiate(withStarPrefab, starsContainer);
        for (int i = starCount; i < totalStars; i++) Instantiate(noStarPrefab, starsContainer);
    }
}
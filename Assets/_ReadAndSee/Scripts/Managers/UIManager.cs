using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Quiz UI References")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private ImageOptionButton[] imageOptionButtons;
    [SerializeField] private TextMeshProUGUI questionCounter;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Results UI References")]
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI incorrectAtmpTxt;
    [SerializeField] Transform starsContainer;
    [SerializeField] GameObject withStarPrefab;
    [SerializeField] GameObject noStarPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "QuizGame")
        {
            UpdateQuizCard();
        }
        else if (SceneManager.GetActiveScene().name == "Results")
        {
            ShowResults();
        }
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
            scoreText.text = $"Score: {GameManager.Instance.CurrentScore}";
        }
    }

    private void ShowResults()
    {
        playerName.text = $"Player: {SaveManager.Instance.CurrentPlayerData.playerName}";
        incorrectAtmpTxt.text = $"Incorrect attempts: {GameManager.Instance.TotalQuestions - GameManager.Instance.CorrectAnswers}";
        int stars = SaveManager.Instance.CurrentPlayerData.GetStarsForCategory(GameManager.Instance.CurrentLevel);
        DisplayStars(stars);
    }

    public void DisplayStars(int starCount)
    {
        foreach (Transform child in starsContainer) Destroy(child.gameObject);

        const int totalStars = 3;
        for (int i = 0; i < starCount && i < totalStars; i++) Instantiate(withStarPrefab, starsContainer);
        for (int i = starCount; i < totalStars; i++) Instantiate(noStarPrefab, starsContainer);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPlayAgainButtonClicked()
    {
        GameManager.Instance.StartGame(GameManager.Instance.CurrentLevel);
    }
}
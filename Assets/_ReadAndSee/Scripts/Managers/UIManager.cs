using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Quiz UI References")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private ImageOptionButton[] imageOptionButtons;
    [SerializeField] private TextMeshProUGUI questionCounter;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Results UI References")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI correctAnswersText;
    [SerializeField] private TextMeshProUGUI totalQuestionsText;
    [SerializeField] private TextMeshProUGUI percentageText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "QuizGame")
        {
            categoryText.text = GameManager.Instance.currentCategory;
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
            int currentIndex = GameManager.Instance.currentQuestionIndex + 1;
            int total = GameManager.Instance.totalQuestions;
            questionCounter.text = $"Question {currentIndex}/{total}";
            progressBar.value = GameManager.Instance.GetProgress();

            // Update score
            scoreText.text = $"Score: {GameManager.Instance.currentScore}";
        }
    }

    private void ShowResults()
    {
        finalScoreText.text = $"Final Score: {GameManager.Instance.currentScore}";
        correctAnswersText.text = $"Correct Answers: {GameManager.Instance.correctAnswers}";
        totalQuestionsText.text = $"Total Questions: {GameManager.Instance.totalQuestions}";

        float percentage = (float)GameManager.Instance.correctAnswers / GameManager.Instance.totalQuestions * 100f;
        percentageText.text = $"Accuracy: {percentage:F1}%";
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPlayAgainButtonClicked()
    {
        GameManager.Instance.StartGame(GameManager.Instance.currentCategory);
    }
}
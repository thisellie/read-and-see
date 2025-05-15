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
    [SerializeField] private TextMeshProUGUI CorrectAnswersText;
    [SerializeField] private TextMeshProUGUI TotalQuestionsText;
    [SerializeField] private TextMeshProUGUI percentageText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "QuizGame")
        {
            categoryText.text = GameManager.Instance.CurrentCategory.ToString();
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
            progressBar.value = GameManager.Instance.GetProgress();

            // Update score
            scoreText.text = $"Score: {GameManager.Instance.CurrentScore}";
        }
    }

    private void ShowResults()
    {
        finalScoreText.text = $"Final Score: {GameManager.Instance.CurrentScore}";
        CorrectAnswersText.text = $"Correct Answers: {GameManager.Instance.CorrectAnswers}";
        TotalQuestionsText.text = $"Total Questions: {GameManager.Instance.TotalQuestions}";

        float percentage = (float)GameManager.Instance.CorrectAnswers / GameManager.Instance.TotalQuestions * 100f;
        percentageText.text = $"Accuracy: {percentage:F1}%";
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnPlayAgainButtonClicked()
    {
        GameManager.Instance.StartGame(GameManager.Instance.CurrentCategory);
    }
}
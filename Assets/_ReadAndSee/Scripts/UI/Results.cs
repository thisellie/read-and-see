using UnityEngine;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI correctAnswersText;
    [SerializeField] private TextMeshProUGUI finalMessageText;

    [SerializeField] private string excellentResultMessage = "Excellent! You're a quiz master!";
    [SerializeField] private string goodResultMessage = "Well done! That's a good score!";
    [SerializeField] private string averageResultMessage = "Not bad! Keep practicing!";
    [SerializeField] private string poorResultMessage = "Better luck next time!";

    private void Start()
    {
        UpdateResultsUI();
    }

    public void UpdateResultsUI()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager != null)
        {
            finalScoreText.text = $"Final Score: {gameManager.currentScore}";
            correctAnswersText.text = $"Correct Answers: {gameManager.correctAnswers}/{gameManager.totalQuestions}";

            // Calculate percentage correct
            float percentage = (float)gameManager.correctAnswers / gameManager.totalQuestions;

            // Set appropriate message based on performance
            if (percentage >= 0.9f)
                finalMessageText.text = excellentResultMessage;
            else if (percentage >= 0.7f)
                finalMessageText.text = goodResultMessage;
            else if (percentage >= 0.5f)
                finalMessageText.text = averageResultMessage;
            else
                finalMessageText.text = poorResultMessage;
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  // Game state
  public int currentScore { get; private set; }
  public int currentQuestionIndex { get; private set; }
  public int correctAnswers { get; private set; }
  public int totalQuestions { get; private set; }
  public string currentCategory { get; private set; }

  private Question[] currentQuestions;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public void StartGame(string categoryName)
  {
    currentCategory = categoryName;
    ResetGameState();
    LoadQuizScene();
  }

  private void ResetGameState()
  {
    currentScore = 0;
    currentQuestionIndex = 0;
    correctAnswers = 0;

    // Load questions from selected category
    QuizCategory category = QuizDatabase.Instance.GetCategory(currentCategory);
    if (category != null)
    {
      currentQuestions = category.questions;
      totalQuestions = currentQuestions.Length;
    }
    else
    {
      Debug.LogError("Category not found: " + currentCategory);
    }
  }

  public Question GetCurrentQuestion()
  {
    if (currentQuestionIndex < currentQuestions.Length)
    {
      return currentQuestions[currentQuestionIndex];
    }
    return null;
  }

  public void AnswerQuestion(int selectedOptionIndex)
  {
    Question currentQuestion = GetCurrentQuestion();

    if (currentQuestion != null)
    {
      bool isCorrect = (selectedOptionIndex == currentQuestion.correctAnswerIndex);

      if (isCorrect)
      {
        currentScore += currentQuestion.pointValue;
        correctAnswers++;
        AudioManager.Instance.PlayCorrectSound();
      }
      else
      {
        AudioManager.Instance.PlayWrongSound();
      }

      // Move to next question or end game
      currentQuestionIndex++;

      if (currentQuestionIndex >= currentQuestions.Length)
      {
        EndGame();
      }
      else
      {
        // Small delay before showing next question
        Invoke("UpdateQuizUI", 1.0f);
      }

      return;
    }

    Debug.LogWarning("Tried to answer but no current question exists");
  }

  private void UpdateQuizUI()
  {
    if (UIManager.Instance != null)
    {
      UIManager.Instance.UpdateQuizCard();
    }
  }

  private void EndGame()
  {
    // Save stats if needed
    SceneManager.LoadScene("Results");
  }

  private void LoadQuizScene()
  {
    SceneManager.LoadScene("QuizGame");
  }

  public float GetProgress()
  {
    return (float)currentQuestionIndex / totalQuestions;
  }
}
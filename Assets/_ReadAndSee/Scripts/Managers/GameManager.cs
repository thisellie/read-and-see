using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game state
    public DifficultyLevel CurrentDifficulty { get; set; }
    public int CurrentScore { get; private set; }
    public int CurrentQuestionIndex { get; private set; }
    public int CorrectAnswers { get; private set; }
    public int TotalQuestions { get; private set; }
    public string CurrentCategory { get; private set; }

    private Question[] currentQuestions;
    private float startTime;

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
        CurrentCategory = categoryName;
        ResetGameState();
        LoadQuizScene();
    }

    private void ResetGameState()
    {
        CurrentScore = 0;
        CurrentQuestionIndex = 0;
        CorrectAnswers = 0;
        startTime = Time.time;

        QuizCategory category = QuizDatabase.Instance.GetCategory(CurrentCategory);
        if (category != null)
        {
            currentQuestions = category.questions;
            TotalQuestions = currentQuestions.Length;
        }
        else
        {
            Debug.LogError("Category not found: " + CurrentCategory);
        }
    }

    public Question GetCurrentQuestion()
    {
        if (CurrentQuestionIndex < currentQuestions.Length)
        {
            return currentQuestions[CurrentQuestionIndex];
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
                CorrectAnswers++;
                AudioManager.Instance.PlayCorrectSound();
            }
            else
            {
                AudioManager.Instance.PlayWrongSound();
            }

            CurrentQuestionIndex++;

            if (CurrentQuestionIndex >= currentQuestions.Length)
            {
                EndGame();
            }
            else
            {
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
        float totalTime = Time.time - startTime;
        int starsEarned = 1; // 1 star for completion

        if (CorrectAnswers == TotalQuestions)
            starsEarned++; // Perfect score

        if (totalTime < 180f)
            starsEarned++; // Under 3 minutes

        Debug.Log($"Quiz Finished in {totalTime:F2} seconds. Stars Earned: {starsEarned}");

        // Store the result in PlayerData
        DifficultyProgress progress = SaveManager.Instance.CurrentPlayerData.allProgress
            .Find(d => d.difficultyName == CurrentDifficulty);

        if (progress != null)
        {
            LevelProgress existingLevel = progress.levels.Find(l => l.quizCategory == CurrentCategory);

            if (existingLevel != null)
            {
                if (starsEarned > existingLevel.starsEarned)
                    existingLevel.starsEarned = starsEarned;

                existingLevel.timeTaken = totalTime.ToString();
            }
            else
            {
                progress.levels.Add(new LevelProgress(
                   CurrentCategory,
                   starsEarned,
                   DateTime.Now
                ));
            }
        }

        SaveManager.Instance.CurrentPlayerData.lastSavedTime = DateTime.Now.ToString();
        SaveManager.Instance.SavePlayer();

        SceneManager.LoadScene("Results");
    }

    private void LoadQuizScene()
    {
        SceneManager.LoadScene("QuizGame");
    }

    public float GetProgress()
    {
        return (float)CurrentQuestionIndex / TotalQuestions;
    }
}
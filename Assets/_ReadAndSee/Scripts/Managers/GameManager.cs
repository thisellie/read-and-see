using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game state
    public string currentDifficulty { get; set; }
    public int currentScore { get; private set; }
    public int currentQuestionIndex { get; private set; }
    public int correctAnswers { get; private set; }
    public int totalQuestions { get; private set; }
    public string currentCategory { get; private set; }

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
        currentCategory = categoryName;
        ResetGameState();
        LoadQuizScene();
    }

    private void ResetGameState()
    {
        currentScore = 0;
        currentQuestionIndex = 0;
        correctAnswers = 0;
        startTime = Time.time;

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
                correctAnswers++;
                AudioManager.Instance.PlayCorrectSound();
            }
            else
            {
                AudioManager.Instance.PlayWrongSound();
            }

            currentQuestionIndex++;

            if (currentQuestionIndex >= currentQuestions.Length)
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

        if (correctAnswers == totalQuestions)
            starsEarned++; // Perfect score

        if (totalTime < 180f)
            starsEarned++; // Under 3 minutes

        Debug.Log($"Quiz Finished in {totalTime:F2} seconds. Stars Earned: {starsEarned}");

        // Store the result in PlayerData
        DifficultyProgress progress = SaveManager.Instance.CurrentPlayerData.allProgress
            .Find(d => d.difficultyName == currentDifficulty);

        if (progress != null)
        {
            LevelProgress existingLevel = progress.levels.Find(l => l.quizCategory == currentCategory);

            if (existingLevel != null)
            {
                if (starsEarned > existingLevel.starsEarned)
                    existingLevel.starsEarned = starsEarned;

                existingLevel.timeTaken = totalTime.ToString();
            }
            else
            {
                progress.levels.Add(new LevelProgress(
                   currentCategory,
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
        return (float)currentQuestionIndex / totalQuestions;
    }
}
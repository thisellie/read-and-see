using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game state
    public Difficulty CurrentDifficulty { get; set; }
    public int CurrentQuestionIndex { get; private set; }
    public int CorrectAnswers { get; private set; }
    public int TotalQuestions { get; private set; }
    public string CurrentLevel { get; private set; }
    public QuizLevel Level { get; private set; }
    public bool IsPaused { get; set; } = false;

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

    public void StartGame(string levelName)
    {
        CurrentLevel = levelName;
        ResetGameState();
        LoadQuizScene();
    }

    private void ResetGameState()
    {
        CurrentQuestionIndex = 0;
        CorrectAnswers = 0;
        startTime = Time.time;

        Level = QuizDatabase.Instance.GetLevel(CurrentLevel);
        if (Level != null)
        {
            currentQuestions = Level.questions;
            TotalQuestions = currentQuestions.Length;
        }
        else
        {
            Debug.LogError("Level not found: " + CurrentLevel);
        }
    }

    private void LoadQuizScene()
    {
        SceneManager.LoadScene("QuizGame");
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
            AudioClip clipToPlay = isCorrect ? currentQuestion.correctClip : currentQuestion.wrongClip;
            if (isCorrect) CorrectAnswers++;
            StartCoroutine(PlayClip(clipToPlay));
            return;
        }

        Debug.LogWarning("Tried to answer but no current question exists");
    }

    private IEnumerator PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            AudioManager.Instance.PlaySound(clip);
            yield return new WaitForSeconds(clip.length);
        }
        else
        {
            yield return new WaitForSeconds(1f); // fallback wait
        }

        CurrentQuestionIndex++;

        // TODO: Play video before EndGame()
        if (CurrentQuestionIndex >= currentQuestions.Length) EndGame();
        else UpdateQuizUI();
    }

    private void UpdateQuizUI()
    {
        if (UIManager.Instance != null) UIManager.Instance.UpdateQuizCard();
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
            LevelProgress existingLevel = progress.levels.Find(l => l.quizCategory == CurrentLevel);

            if (existingLevel != null)
            {
                if (starsEarned > existingLevel.starsEarned) existingLevel.starsEarned = starsEarned;
                existingLevel.timeTaken = totalTime.ToString();
            }
            else
            {
                progress.levels.Add(new LevelProgress(
                   CurrentLevel,
                   starsEarned,
                   DateTime.Now
                ));
            }
        }

        SaveManager.Instance.CurrentPlayerData.lastSavedTime = DateTime.Now.ToString();
        SaveManager.Instance.SavePlayer();
        SceneManager.LoadScene("Results");
    }
}
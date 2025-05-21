using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Game state
    public Difficulty CurrentDifficulty { get; set; }
    public int CurrentQuestionIndex { get; private set; }
    public int IncorrectAttempts { get; private set; }
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

    private void Start()
    {
#if UNITY_EDITOR
        QuizLevel testLevel = Resources.Load<QuizLevel>("Levels/JohnKite");
        CurrentLevel = testLevel.levelName;
        ResetGameState();
#endif
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
        IncorrectAttempts = 0;
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

    public void AnswerQuestion(int selectedOptionIndex, Button selectedButton)
    {
        Question currentQuestion = GetCurrentQuestion();
        if (currentQuestion == null)
        {
            Debug.LogWarning("Tried to answer but no current question exists");
            return;
        }

        bool isCorrect = (selectedOptionIndex == currentQuestion.correctAnswerIndex);
        AudioClip clipToPlay = isCorrect ? currentQuestion.correctClip : currentQuestion.wrongClip;
        UIManager.Instance.UpdateMascotSprite(isCorrect);

        if (isCorrect)
        {
            StartCoroutine(PlayClipThenNext(clipToPlay));
        }
        else
        {
            IncorrectAttempts++;
            StartCoroutine(PlayWrongClipAndForceCorrect(currentQuestion.correctAnswerIndex, selectedButton, clipToPlay));
        }

        // TODO: Update mascot or feedback UI
    }

    private IEnumerator PlayClipThenNext(AudioClip clip)
    {
        if (clip != null) AudioManager.Instance.PlaySound(clip);
        yield return new WaitForSeconds(clip != null ? clip.length : 1f);

        CurrentQuestionIndex++;

        if (CurrentQuestionIndex >= currentQuestions.Length)
            StartCoroutine(UIManager.Instance.PlayVideo(Level.videoClip));
        else
            UpdateQuizUI();
    }

    private IEnumerator PlayWrongClipAndForceCorrect(int correctIndex, Button wrongButton, AudioClip wrongClip)
    {
        if (wrongClip != null) AudioManager.Instance.PlaySound(wrongClip);
        yield return new WaitForSeconds(wrongClip != null ? wrongClip.length : 1f);

        foreach (var option in FindObjectsByType<ImageOptionButton>(FindObjectsSortMode.None))
        {
            bool isCorrect = (option.optionIndex == correctIndex);
            option.button.interactable = isCorrect;
        }

        wrongButton.interactable = false;
    }

    private void UpdateQuizUI()
    {
        if (UIManager.Instance != null) UIManager.Instance.UpdateQuizCard();
    }


    public void EndGame()
    {
        float totalTime = Time.time - startTime;
        int starsEarned = 1; // 1 star for completion

        if (IncorrectAttempts == 0)
            starsEarned++; // Perfect score

        if (totalTime < 180f)
            starsEarned++; // Under 3 minutes

        Debug.Log($"Quiz Finished in {totalTime:F2} seconds. Stars Earned: {starsEarned}. Incorrect Attempts: {IncorrectAttempts}");

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
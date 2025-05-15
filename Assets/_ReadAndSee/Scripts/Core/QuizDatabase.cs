using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Wrapper class for JSON deserialization
[System.Serializable]
public class QuizData
{
    public QuizLevel[] levels;
}

public class QuizDatabase : MonoBehaviour
{
  public static QuizDatabase Instance { get; private set; }

  [SerializeField] private TextAsset quizDataJson;
  private QuizLevel[] levels;

  private void Awake()
  {
    // Setup singleton
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      LoadQuestions();
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void LoadQuestions()
  {
    if (quizDataJson != null)
    {
      // Parse the JSON data
      string jsonData = quizDataJson.text;
      QuizData loadedData = JsonUtility.FromJson<QuizData>(jsonData);
      levels = loadedData.levels;
    }
    else
    {
      Debug.LogError("Quiz data JSON file not assigned!");
    }

        foreach (var category in levels)
        {
            foreach (var question in category.questions)
            {
                question.imageOptions = new Sprite[question.imageOptionPaths.Length];
                for (int i = 0; i < question.imageOptionPaths.Length; i++)
                {
                    question.imageOptions[i] = LoadSpriteFromResources(question.imageOptionPaths[i]);
                }
            }
        }
    }

    private Sprite LoadSpriteFromResources(string path)
    {
        return Resources.Load<Sprite>(path);
    }

    // Get all levels based on the difficulty from the GameManager.Instance.currentDifficulty
    public QuizLevel[] GetLevels()
    {
        return levels.Where(level => level.difficulty == GameManager.Instance.CurrentDifficulty).ToArray();
    }

    public QuizLevel GetLevel(string levelName)
  {
    foreach (var category in levels)
    {
      if (category.levelName == levelName)
        return category;
    }
    return null;
  }
}

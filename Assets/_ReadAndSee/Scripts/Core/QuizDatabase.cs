using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuizDatabase : MonoBehaviour
{
  public static QuizDatabase Instance { get; private set; }

  [SerializeField] private TextAsset quizDataJson;
  private QuizCategory[] categories;

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
      categories = loadedData.categories;
    }
    else
    {
      Debug.LogError("Quiz data JSON file not assigned!");
    }

        foreach (var category in categories)
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
    public QuizCategory[] GetLevels()
    {
        QuizCategory[] levels = categories.Where(level => level.difficulty == GameManager.Instance.CurrentDifficulty).ToArray();
        return levels;
    }

    public QuizCategory GetCategory(string categoryName)
  {
    foreach (var category in categories)
    {
      if (category.categoryName == categoryName)
        return category;
    }
    return null;
  }

  public QuizCategory GetCategory(int index)
  {
    if (index >= 0 && index < categories.Length)
      return categories[index];
    return null;
  }

  public int GetCategoryCount()
  {
    return categories.Length;
  }
}

// Wrapper class for JSON deserialization
[System.Serializable]
public class QuizData
{
  public QuizCategory[] categories;
}
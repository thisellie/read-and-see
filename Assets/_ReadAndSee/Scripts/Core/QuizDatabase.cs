using System.Linq;
using UnityEngine;

public class QuizDatabase : MonoBehaviour
{
    public static QuizDatabase Instance { get; private set; }
    public QuizLevel[] levels;

    private void Awake()
    {
        // Setup singleton
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

    // Get the levels based on the difficulty from the GameManager.Instance.currentDifficulty
    public QuizLevel[] GetLevels()
    {
        return levels.Where(level => level.difficulty == GameManager.Instance.CurrentDifficulty).ToArray();
    }

    public QuizLevel GetLevel(string levelName)
    {
        foreach (var category in levels) 
            if (category.levelName == levelName) 
                return category;

        return null;
    }
}

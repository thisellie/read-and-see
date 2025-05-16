using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public enum Difficulty
{
    Beginner,
    Normal,
    Challenging
}

[Serializable]
public class PlayerData
{
    public string playerName;
    public string lastSavedTime;

    public List<DifficultyProgress> allProgress = new();

    public int GetLevelStars(string category)
    {
        foreach (var progress in allProgress)
        {
            var level = progress.levels.FirstOrDefault(l => l.quizCategory == category);
            if (level != null) return level.starsEarned;
        }

        return 0;
    }
}

[Serializable]
public class DifficultyProgress
{
    public Difficulty difficultyName;
    public List<LevelProgress> levels = new();

    public DifficultyProgress(Difficulty name)
    {
        difficultyName = name;
    }
}

[Serializable]
public class LevelProgress
{
    public string quizCategory;
    public int starsEarned;
    public string timeTaken;

    public LevelProgress(string category, int stars, DateTime time)
    {
        quizCategory = category;
        starsEarned = stars;
        timeTaken = time.ToString("s");
    }
}

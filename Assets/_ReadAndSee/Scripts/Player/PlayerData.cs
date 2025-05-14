using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string lastSavedTime;

    public List<DifficultyProgress> allProgress = new();

    public PlayerData()
    {
        allProgress.Add(new DifficultyProgress(DifficultyLevel.Beginner.ToString()));
        allProgress.Add(new DifficultyProgress(DifficultyLevel.Normal.ToString()));
        allProgress.Add(new DifficultyProgress(DifficultyLevel.Challenging.ToString()));
    }

    public DifficultyProgress GetProgress(string difficultyName)
    {
        return allProgress.Find(p => p.difficultyName.Equals(difficultyName, StringComparison.OrdinalIgnoreCase));
    }
}

[System.Serializable]
public enum DifficultyLevel
{
    Beginner,
    Normal,
    Challenging
}

[System.Serializable]
public class DifficultyProgress
{
    public string difficultyName;
    public List<LevelProgress> levels = new();

    public DifficultyProgress(string name)
    {
        difficultyName = name;
    }
}

[System.Serializable]
public class LevelProgress
{
    public string quizCategory;
    public int starsEarned;
    public string timeTaken;

    public LevelProgress(string category, int stars, DateTime time)
    {
        this.quizCategory = category;
        this.starsEarned = stars;
        this.timeTaken = time.ToString("s");
    }

    public DateTime GetTimeTakenAsDateTime()
    {
        DateTime.TryParse(timeTaken, out var result);
        return result;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string lastSavedTime;

    public List<DifficultyProgress> allProgress = new();

    public PlayerData()
    {
        allProgress.Add(new DifficultyProgress(Difficulty.Beginner));
        allProgress.Add(new DifficultyProgress(Difficulty.Normal));
        allProgress.Add(new DifficultyProgress(Difficulty.Challenging));
    }

    public DifficultyProgress GetProgress(Difficulty difficultyName)
    {
        return allProgress.Find(p => p.difficultyName.Equals(difficultyName));
    }

    public int GetStarsForCategory(string category)
    {
        foreach (var progress in allProgress)
        {
            var level = progress.levels.FirstOrDefault(l => l.quizCategory == category);
            if (level != null) return level.starsEarned;
        }

        return 0;
    }
}

[System.Serializable]
public enum Difficulty
{
    Beginner,
    Normal,
    Challenging
}

[System.Serializable]
public class DifficultyProgress
{
    public Difficulty difficultyName;
    public List<LevelProgress> levels = new();

    public DifficultyProgress(Difficulty name)
    {
        difficultyName = name;
    }

    // Return the levels based on the difficulty (as a parameter)
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

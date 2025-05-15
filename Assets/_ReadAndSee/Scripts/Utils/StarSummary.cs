using System.Collections.Generic;

public class StarSummary
{
    public string playerName;
    public int totalStars;
    public Dictionary<Difficulty, int> starsByDifficulty;

    public StarSummary(string playerName, int totalStars, Dictionary<Difficulty, int> starsByDifficulty)
    {
        this.playerName = playerName;
        this.totalStars = totalStars;
        this.starsByDifficulty = starsByDifficulty ?? new Dictionary<Difficulty, int>();
    }
}

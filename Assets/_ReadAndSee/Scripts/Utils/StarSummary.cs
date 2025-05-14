using System.Collections.Generic;

public class StarSummary
{
    public string playerName;
    public int totalStars;
    public Dictionary<string, int> starsByDifficulty;

    public StarSummary(string playerName, int totalStars, Dictionary<string, int> starsByDifficulty)
    {
        this.playerName = playerName;
        this.totalStars = totalStars;
        this.starsByDifficulty = starsByDifficulty ?? new Dictionary<string, int>();
    }
}

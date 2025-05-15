using UnityEngine;
using TMPro;

public class ScoreboardSlot : MonoBehaviour
{
    [Header("UI Elements in Prefab")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI totalStarsText;
    public TextMeshProUGUI beginnerStarsText;
    public TextMeshProUGUI normalStarsText;
    public TextMeshProUGUI challengingStarsText;

    public void Setup(StarSummary info)
    {
        playerNameText.text = info.playerName;
        totalStarsText.text = $"Total: {info.totalStars} stars";

        int GetStars(Difficulty difficulty)
        {
            return info.starsByDifficulty.TryGetValue(difficulty, out int stars) ? stars : 0;
        }

        beginnerStarsText.text = $"Beginner: {GetStars(Difficulty.Beginner)} stars";
        normalStarsText.text = $"Normal: {GetStars(Difficulty.Normal)} stars";
        challengingStarsText.text = $"Challenging: {GetStars(Difficulty.Challenging)} stars";
    }
}

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
        beginnerStarsText.text = $"Beginner: {info.starsByDifficulty[DifficultyLevel.Beginner.ToString()]} stars";
        normalStarsText.text = $"Normal: {info.starsByDifficulty[DifficultyLevel.Normal.ToString()]} stars";
        challengingStarsText.text = $"Challenging: {info.starsByDifficulty[DifficultyLevel.Challenging.ToString()]} stars";
    }
}

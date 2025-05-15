using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("UI Setup")]
    [SerializeField] TMPro.TextMeshProUGUI difficultyText;
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] Transform levelsParentContainer;

    private void Start()
    {
        difficultyText.text = GameManager.Instance.CurrentDifficulty.ToString();
        PopulateLevelScreen();
    }

    // Populate the levels container with the level button prefab
    public void PopulateLevelScreen()
    {
        foreach (Transform child in levelsParentContainer) Destroy(child.gameObject);

        QuizCategory[] levels = QuizDatabase.Instance.GetLevels();

        foreach (QuizCategory level in levels)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsParentContainer);
            if (levelButton.TryGetComponent<LevelButton>(out var button)) button.Setup(level.categoryName);
        }
    }
}

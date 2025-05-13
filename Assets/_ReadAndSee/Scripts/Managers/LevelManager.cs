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
        difficultyText.text = GameManager.Instance.currentDifficulty;
        PopulateLevelScreen();
    }

    // Populate the levels container with the level button prefab
    public void PopulateLevelScreen()
    {
        foreach (Transform child in levelsParentContainer)
            Destroy(child.gameObject);

        QuizCategory[] levels = QuizDatabase.Instance.GetLevels();

        foreach (QuizCategory level in levels)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsParentContainer);
            if (levelButton.TryGetComponent<Button>(out var button))
                button.onClick.AddListener(() => OnLevelSelected(level.categoryName));
        }
    }

    public void OnLevelSelected(string categoryName)
    {
        AudioManager.Instance.PlayButtonClickSound();
        GameManager.Instance.StartGame(categoryName);
    }
}

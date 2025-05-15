using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] Transform starsContainer;
    [SerializeField] GameObject withStarPrefab;
    [SerializeField] GameObject noStarPrefab;
    [SerializeField] Button levelButton;

    private string levelName;

    // Check the SaveManager.Instance.currentPlayerData if the loaded level has stars saved
    // Then load the amount of star using the prefabs

    public void Setup(string levelName)
    {
        this.levelName = levelName;

        int stars = SaveManager.Instance.CurrentPlayerData.GetStarsForCategory(levelName);
        DisplayStars(stars);

        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(OnLevelSelected);
    }

    public void DisplayStars(int starCount)
    {
        foreach (Transform child in starsContainer) Destroy(child.gameObject);

        const int totalStars = 3;
        for (int i = 0; i < starCount && i < totalStars; i++) Instantiate(withStarPrefab, starsContainer);
        for (int i = starCount; i < totalStars; i++) Instantiate(noStarPrefab, starsContainer);
    }

    public void OnLevelSelected()
    {
        AudioManager.Instance.PlayButtonClickSound();
        GameManager.Instance.StartGame(levelName);
    }
}

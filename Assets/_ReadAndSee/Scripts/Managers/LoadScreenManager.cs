using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadScreenManager : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject saveSlotUIPrefab;
    public Transform slotsParentContainer;
    public GameObject scoreboardUIPrefab;
    public Transform scoreboardContainer;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI playerName;
    public GameObject levelButtonPrefab;
    public Transform levelsParentContainer;

    [Header("Animation Variables")]
    public float animationTime = 0.4f;

    void Start()
    {
        if (saveSlotUIPrefab == null || slotsParentContainer == null)
        {
            Debug.LogError("LoadScreenManager UI Prefab or Parent Container not set in Inspector!");
            return;
        }

        PopulateLoadScreen();
        PopulateScoreboard();
    }

    [ContextMenu("PopulateLoadScreen")]
    public void PopulateLoadScreen()
    {
        foreach (Transform child in slotsParentContainer)
        {
            Destroy(child.gameObject);
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager Instance not found. Ensure SaveManager is in the scene.");
            return;
        }

        List<SaveSlotInfo> allSlotsMetadata = SaveManager.Instance.GetAllSaveSlotMetadata();

        if (allSlotsMetadata.Count == 0)
        {
            Debug.Log("No save files found.");
            // TODO: Display a message like "No save games found."
            return;
        }

        foreach (SaveSlotInfo slotInfo in allSlotsMetadata)
        {
            GameObject slotUIInstance = Instantiate(saveSlotUIPrefab, slotsParentContainer);
            
            if (slotUIInstance.TryGetComponent<SaveSlot>(out var slotComponent))
            {
                slotComponent.Setup(slotInfo);
            }
            else
            {
                Debug.LogError($"SaveSlotUIPrefab is missing the SaveSlot script component.");
            }
        }
    }

    public void PopulateScoreboard()
    {
        foreach (Transform child in scoreboardContainer)
        {
            Destroy(child.gameObject);
        }

        List<StarSummary> allStarsMetadata = SaveManager.Instance.GetAllSaveSlotStarsMetadata();

        if (allStarsMetadata.Count == 0)
        {
            Debug.Log("No save files found.");
            return;
        }

        foreach (StarSummary summary in allStarsMetadata)
        {
            GameObject scoreboardUIInstance = Instantiate(scoreboardUIPrefab, scoreboardContainer);
            
            if (scoreboardUIInstance.TryGetComponent<ScoreboardSlot>(out var scoreboardComponent))
            {
                scoreboardComponent.Setup(summary);
            }
            else
            {
                Debug.LogError($"ScoreboardUIPrefab is missing the ScoreboardSlot script component.");
            }
        }
    }

    public void PopulateDifficultyScreen()
    {
        labelText.text = "Difficulty";
        playerName.text = $"Player: {SaveManager.Instance.CurrentPlayerData.playerName}";
    }

    public void PopulateLevelScreen()
    {
        labelText.text = GameManager.Instance.CurrentDifficulty.ToString();
        foreach (Transform child in levelsParentContainer) Destroy(child.gameObject);

        QuizLevel[] levels = QuizDatabase.Instance.GetLevels();

        foreach (QuizLevel level in levels)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsParentContainer);
            if (levelButton.TryGetComponent<LevelButton>(out var button)) button.Setup(level.levelName, level.thumbnail);
        }
    }

    public void AnimateIn(GameObject panel)
    {
        panel.SetActive(true);

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        RectTransform rt = panel.GetComponent<RectTransform>();

        cg.alpha = 0;
        rt.anchoredPosition = new Vector2(0, -200);

        LeanTween.alphaCanvas(cg, 1f, animationTime);
        LeanTween.move(rt, Vector2.zero, animationTime).setEaseOutBack();
    }

    public void AnimateOut(GameObject panel)
    {
        if (!panel.activeSelf) return;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        RectTransform rt = panel.GetComponent<RectTransform>();

        LeanTween.alphaCanvas(cg, 0f, animationTime);
        LeanTween.move(rt, new Vector2(0, -200), animationTime).setEaseInBack()
            .setOnComplete(() => panel.SetActive(false));
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScreenManager : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject saveSlotUIPrefab;
    public Transform slotsParentContainer;
    public GameObject scoreboardUIPrefab;
    public Transform scoreboardContainer;
    public string gameSceneName = "GameScene";

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
                slotComponent.Setup(slotInfo, gameSceneName);
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

    // TODO: Add a button on your load screen to go back to a Main Menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [Header("UI Elements in Prefab")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI lastSavedTimeText;
    public Button loadButton;

    private SaveSlotInfo currentSlotInfo;
    private string sceneToLoad;

    public void Setup(SaveSlotInfo info, string gameSceneName)
    {
        currentSlotInfo = info;
        sceneToLoad = gameSceneName;

        playerNameText.text = info.playerName;
        lastSavedTimeText.text = info.lastSavedTime;

        loadButton.onClick.RemoveAllListeners();
        loadButton.onClick.AddListener(OnSlotClicked);
    }

    void OnSlotClicked()
    {
        if (currentSlotInfo == null)
            return;

        if (!currentSlotInfo.isEmpty)
        {
            Debug.Log($"Attempting to load game from file: {currentSlotInfo.fileName}");
            bool loadSuccess = SaveManager.Instance.LoadPlayer(currentSlotInfo.fileName);
            if (loadSuccess)
            {
                SaveManager.Instance.TransitionToScene(sceneToLoad);
            }
            else
            {
                Debug.LogError($"Failed to load data from file {currentSlotInfo.fileName}.");
                // TODO: Display error UI here
            }
        }
        else
        {
            Debug.Log($"Save file {currentSlotInfo.fileName} is empty or corrupted.");
            // TODO: Show a UI prompt to delete or replace
        }
    }
}

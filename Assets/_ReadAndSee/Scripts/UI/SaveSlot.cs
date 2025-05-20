using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SaveSlot : MonoBehaviour
{
    [Header("UI Elements in Prefab")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI lastSavedTimeText;
    public Button loadButton;

    private LoadScreenManager loadScreenManager;
    private SaveSlotInfo currentSlotInfo;

    public void Setup(SaveSlotInfo info)
    {
        currentSlotInfo = info;
        playerNameText.text = info.playerName;
        lastSavedTimeText.text = info.lastSavedTime;

        loadScreenManager = GameObject.Find("LoadScreenManager").GetComponent<LoadScreenManager>();
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
                Transform parent = GameObject.Find("Canvas").transform;
                loadScreenManager.PopulateDifficultyScreen();
                loadScreenManager.AnimateOut(parent.Find("LoadGamePanel").gameObject);
                loadScreenManager.AnimateIn(parent.Find("GameSetupPanel").gameObject);
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

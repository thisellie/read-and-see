using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public PlayerData CurrentPlayerData { get; private set; }

    [Header("Save Slot Configuration")]
    private string baseFileName = "save_slot_";
    private string fileExtension = ".json";
    private string saveFileName = string.Empty;

    [Header("UI References (for Saving)")]
    public TMPro.TMP_InputField playerNameInputFieldForSave;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName + fileExtension);
    }

    private string GenerateUniqueFileName(string playerName)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return baseFileName + playerName + "_" + timestamp;
    }

    public void CreatePlayer()
    {
        if (playerNameInputFieldForSave == null || string.IsNullOrWhiteSpace(playerNameInputFieldForSave.text))
        {
            Debug.LogError("Player name input field is not set or player name is empty. Cannot save.");
            return;
        }

        PlayerData data = new PlayerData
        {
            playerName = playerNameInputFieldForSave.text,
            lastSavedTime = DateTime.Now.ToString("s"), // ISO 8601
            allProgress = new List<DifficultyProgress>
            {
                new(DifficultyLevel.Beginner.ToString()),
                new(DifficultyLevel.Normal.ToString()),
                new(DifficultyLevel.Challenging.ToString())
            }
        };

        string uniqueFileName = GenerateUniqueFileName(data.playerName);
        string saveFileName = GetFilePath(uniqueFileName);

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFileName, json);
            Debug.Log($"Game saved to {saveFileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public void SavePlayer()
    {
        try
        {
            string json = JsonUtility.ToJson(CurrentPlayerData, true);
            File.WriteAllText(saveFileName, json);
            Debug.Log($"Game saved to {saveFileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public bool LoadPlayer(string fileName)
    {
        saveFileName = GetFilePath(fileName);

        if (File.Exists(saveFileName))
        {
            try
            {
                string json = File.ReadAllText(saveFileName);
                CurrentPlayerData = JsonUtility.FromJson<PlayerData>(json);

                if (CurrentPlayerData != null)
                {
                    Debug.Log($"Game loaded from {saveFileName}. Player: {CurrentPlayerData.playerName}");
                    return true;
                }
                else
                {
                    Debug.LogError("Failed to parse JSON data.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"No save file found at {saveFileName}");
            return false;
        }
    }

    // TODO: Delete player function
    //public void DeletePlayer() { }

    public List<SaveSlotInfo> GetAllSaveSlotMetadata()
    {
        List<SaveSlotInfo> slotInfos = new List<SaveSlotInfo>();
        string[] files = Directory.GetFiles(Application.persistentDataPath, baseFileName + "*" + fileExtension);

        foreach (string saveFileName in files)
        {
            try
            {
                string json = File.ReadAllText(saveFileName);
                PlayerData tempData = JsonUtility.FromJson<PlayerData>(json);
                string fileName = Path.GetFileNameWithoutExtension(saveFileName);

                if (tempData != null)
                {
                    slotInfos.Add(new SaveSlotInfo(fileName, tempData.playerName, tempData.lastSavedTime, false));
                }
                else
                {
                    slotInfos.Add(new SaveSlotInfo(fileName, "Corrupted Data", "", true));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading save file {saveFileName}: {e.Message}");
            }
        }

        return slotInfos;
    }

    public void TransitionToScene(string sceneName)
    {
        if (CurrentPlayerData == null && !IsLoadingForNewGame(sceneName))
        {
            Debug.LogWarning("Attempting to transition without loaded data.");
        }
        SceneManager.LoadScene(sceneName);
    }

    private bool IsLoadingForNewGame(string sceneName)
    {
        return false;
    }
}
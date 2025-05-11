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

    public void SaveGame()
    {
        if (playerNameInputFieldForSave == null || string.IsNullOrWhiteSpace(playerNameInputFieldForSave.text))
        {
            Debug.LogError("Player name input field is not set or player name is empty. Cannot save.");
            return;
        }

        PlayerData data = new PlayerData
        {
            playerName = playerNameInputFieldForSave.text,
            lastSavedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        };

        string uniqueFileName = GenerateUniqueFileName(data.playerName);
        string filePath = GetFilePath(uniqueFileName);

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Game saved to {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public bool LoadGame(string fileName)
    {
        string filePath = GetFilePath(fileName);

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                CurrentPlayerData = JsonUtility.FromJson<PlayerData>(json);

                if (CurrentPlayerData != null)
                {
                    Debug.Log($"Game loaded from {filePath}. Player: {CurrentPlayerData.playerName}");
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
            Debug.LogWarning($"No save file found at {filePath}");
            return false;
        }
    }

    public List<SaveSlotInfo> GetAllSaveSlotMetadata()
    {
        List<SaveSlotInfo> slotInfos = new List<SaveSlotInfo>();
        string[] files = Directory.GetFiles(Application.persistentDataPath, baseFileName + "*" + fileExtension);

        foreach (string filePath in files)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                PlayerData tempData = JsonUtility.FromJson<PlayerData>(json);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

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
                Debug.LogError($"Error reading save file {filePath}: {e.Message}");
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

    public void StartNewGameData(string playerName)
    {
        CurrentPlayerData = new PlayerData
        {
            playerName = playerName,
            lastSavedTime = "Never Saved",
        };
        Debug.Log($"Starting new game for player: {playerName}");
    }
}
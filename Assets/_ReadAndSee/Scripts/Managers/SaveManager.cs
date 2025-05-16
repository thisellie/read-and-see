using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditor;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public PlayerData CurrentPlayerData { get; private set; }
    public Action OnPlayerCreated;

    [Header("Save Slot Configuration")]
    private string baseFileName = "save_slot_";
    private string fileExtension = ".json";
    private string saveFileName = string.Empty;

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

    public void CreatePlayer(string name)
    {
        PlayerData data = new()
        {
            playerName = name,
            lastSavedTime = DateTime.Now.ToString("s"), // ISO 8601
            allProgress = new List<DifficultyProgress>
            {
                new(Difficulty.Beginner),
                new(Difficulty.Normal),
                new(Difficulty.Challenging)
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

        LoadPlayer(uniqueFileName);
        OnPlayerCreated?.Invoke();
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

    public List<StarSummary> GetAllSaveSlotStarsMetadata()
    {
        List<StarSummary> starSummaries = new();
        string[] files = Directory.GetFiles(Application.persistentDataPath, baseFileName + "*" + fileExtension);

        foreach (string saveFileName in files)
        {
            try
            {
                string json = File.ReadAllText(saveFileName);

                PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);

                int totalStars = 0;
                Dictionary<Difficulty, int> starsByDifficulty = new();

                foreach (DifficultyProgress difficulty in playerData.allProgress)
                {
                    Debug.Log($"Difficulty: {difficulty.difficultyName}, Levels: {difficulty.levels.Count}");
                    int stars = difficulty.levels.Sum(level => level.starsEarned);
                    starsByDifficulty[difficulty.difficultyName] = stars;
                    totalStars += stars;
                }

                starSummaries.Add(new StarSummary(playerData.playerName, totalStars, starsByDifficulty));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading save file {saveFileName}: {e.Message}");
            }
        }

        return starSummaries;
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
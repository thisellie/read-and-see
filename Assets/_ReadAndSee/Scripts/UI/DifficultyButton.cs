using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    public Difficulty selectedDifficulty;
    public void ApplyDifficulty()
    {
        GameManager.Instance.CurrentDifficulty = selectedDifficulty;
        Debug.Log($"Difficulty selected: {selectedDifficulty}");

        LoadScreenManager loadScreenManager = GameObject.Find("LoadScreenManager").GetComponent<LoadScreenManager>();
        Transform parent = GameObject.Find("Canvas").transform.Find("GameSetupPanel");

        loadScreenManager.AnimateOut(parent.Find("DifficultyPanel").gameObject);
        loadScreenManager.AnimateIn(parent.Find("LevelPanel").gameObject);        
        loadScreenManager.PopulateLevelScreen();
    }
}

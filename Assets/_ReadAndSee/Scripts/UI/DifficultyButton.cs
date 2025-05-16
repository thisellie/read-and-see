using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyButton : MonoBehaviour
{
    public Difficulty selectedDifficulty;
    public void ApplyDifficulty()
    {
        GameManager.Instance.CurrentDifficulty = selectedDifficulty;
        Debug.Log($"Difficulty selected: {selectedDifficulty}");

        // TODO: Load levels based on the selected difficulty and it corresponding level data
        SceneManager.LoadScene("LevelSelect");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizMenu : MonoBehaviour
{
    private string sceneName = string.Empty;

    public void SetScene(string scene)
    {
        sceneName = scene;
        Debug.Log($"Selected scene to load into: {sceneName}");
    }

    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
        else Debug.LogWarning($"No scene was selected: {sceneName}");
    }
}

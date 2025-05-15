using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void TogglePause()
    {
        GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
        Time.timeScale = GameManager.Instance.IsPaused ? 0f : 1f;

        if (GameManager.Instance.IsPaused) UIManager.Instance.PauseUI();
        else UIManager.Instance.ResumeUI();
    }

}

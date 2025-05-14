using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatePlayerButton : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnButtonPress);
    }

    private void OnButtonPress()
    {
        SaveManager.Instance.CreatePlayer();
        SceneManager.LoadScene("DifficultySelect");
    }
}

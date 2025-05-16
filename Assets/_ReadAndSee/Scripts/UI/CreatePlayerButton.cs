using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatePlayerButton : MonoBehaviour
{
    [Header("UI References (for Saving)")]
    [SerializeField] TMPro.TMP_InputField playerNameField;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnButtonPress);
    }

    private void OnButtonPress()
    {
        string name = playerNameField.text;
        if (!string.IsNullOrWhiteSpace(name))
        {
            SaveManager.Instance.OnPlayerCreated += () => SceneManager.LoadScene("DifficultySelect");
            SaveManager.Instance.CreatePlayer(name);
        }
        else
        {
            // TODO: Show UI feedback
            Debug.LogError("Player name is empty. Cannot save.");
            return;
        }
    }
}

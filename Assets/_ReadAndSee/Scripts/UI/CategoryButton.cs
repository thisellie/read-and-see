using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] private string categoryName;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        AudioManager.Instance.PlayButtonClickSound();
        GameManager.Instance.StartGame(categoryName);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ImageOptionButton : MonoBehaviour
{
    [SerializeField] private Image optionImage;
    private Button button;
    private int optionIndex;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Setup(Sprite sprite, int index)
    {
        optionImage.sprite = sprite;
        optionIndex = index;
        button.interactable = true;
    }

    private void OnClick()
    {
        AudioManager.Instance.PlayButtonClickSound();

        foreach (var option in FindObjectsByType<ImageOptionButton>(FindObjectsSortMode.None))
        {
            option.button.interactable = false;
        }

        GameManager.Instance.AnswerQuestion(optionIndex);
    }
}

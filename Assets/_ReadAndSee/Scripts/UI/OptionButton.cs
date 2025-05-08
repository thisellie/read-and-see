using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI optionText;
    [SerializeField] private Button button;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;

    private int optionIndex;
    private Image buttonImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    public void Setup(string text, int index)
    {
        optionText.text = text;
        optionIndex = index;
        buttonImage.color = defaultColor;
        button.interactable = true;
    }

    private void OnButtonClicked()
    {
        AudioManager.Instance.PlayButtonClickSound();

        // Disable all buttons to prevent multiple answers
        foreach (var optionButton in FindObjectsByType<OptionButton>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            optionButton.button.interactable = false;
        }

        Question currentQuestion = GameManager.Instance.GetCurrentQuestion();

        if (currentQuestion != null)
        {
            // Highlight correct and incorrect answers
            foreach (var optionButton in FindObjectsByType<OptionButton>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (optionButton.optionIndex == currentQuestion.correctAnswerIndex)
                {
                    optionButton.buttonImage.color = correctColor;
                }
                else if (optionButton.optionIndex == optionIndex)
                {
                    optionButton.buttonImage.color = incorrectColor;
                }
            }
        }

        GameManager.Instance.AnswerQuestion(optionIndex);
    }
}
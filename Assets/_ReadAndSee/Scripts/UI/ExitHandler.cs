using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExitHandler : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private RectTransform exitPanel;
    [SerializeField] private RectTransform exitDialog;
    [SerializeField] private CanvasGroup exitPanelCanvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeOutQuad;
    [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeInQuad;
    [SerializeField] private Vector2 hiddenPosition = new Vector2(0, -200);
    [SerializeField] private Vector2 visiblePosition = Vector2.zero;

    [Header("Button References")]
    [SerializeField] private Button confirmExitButton;
    [SerializeField] private Button cancelExitButton;

    private InputAction backAction;

    private bool isPanelVisible = false;
    private bool isAnimating = false;

    private void Awake()
    {
        backAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");
        backAction.performed += ctx => HandleBackButton();
    }

    private void OnEnable()
    {
        backAction.Enable();
    }

    private void OnDisable()
    {
        backAction.Disable();
    }

    private void Start()
    {
        // Initial setup - ensure panels are hidden
        if (exitPanel != null)
        {
            exitPanel.gameObject.SetActive(false);
        }

        if (exitDialog != null)
        {
            exitDialog.anchoredPosition = hiddenPosition;

            if (exitPanelCanvasGroup != null)
            {
                exitPanelCanvasGroup.alpha = 0;
                exitPanelCanvasGroup.interactable = false;
                exitPanelCanvasGroup.blocksRaycasts = false;
            }
        }

        // Setup button listeners
        if (confirmExitButton != null)
            confirmExitButton.onClick.AddListener(ExitApplication);

        if (cancelExitButton != null)
            cancelExitButton.onClick.AddListener(HideExitPanel);
    }

    private void HandleBackButton()
    {
        Debug.Log("Pressed Escape");
        if (isAnimating)
            return;

        if (isPanelVisible)
            HideExitPanel();
        else
            ShowExitPanel();
    }

    private void ShowExitPanel()
    {
        if (isAnimating || isPanelVisible || exitDialog == null || exitPanel == null)
            return;

        isAnimating = true;
        exitPanel.gameObject.SetActive(true);

        // Cancel any ongoing tweens
        LeanTween.cancel(exitDialog.gameObject);

        // Animate position
        LeanTween.move(exitDialog, visiblePosition, animationDuration)
            .setEase(easeIn);

        // Animate fade in if we have a canvas group
        if (exitPanelCanvasGroup != null)
        {
            LeanTween.alphaCanvas(exitPanelCanvasGroup, 1f, animationDuration)
                .setEase(easeIn)
                .setOnComplete(() =>
                {
                    exitPanelCanvasGroup.interactable = true;
                    exitPanelCanvasGroup.blocksRaycasts = true;
                    isAnimating = false;
                    isPanelVisible = true;
                });
        }
        else
        {
            LeanTween.delayedCall(animationDuration, () =>
            {
                isAnimating = false;
                isPanelVisible = true;
            });
        }
    }

    private void HideExitPanel()
    {
        if (isAnimating || !isPanelVisible || exitDialog == null || exitPanel == null)
            return;

        isAnimating = true;

        // Cancel any ongoing tweens
        LeanTween.cancel(exitDialog.gameObject);

        // Animate position
        LeanTween.move(exitDialog, hiddenPosition, animationDuration)
            .setEase(easeOut);

        // Animate fade out if we have a canvas group
        if (exitPanelCanvasGroup != null)
        {
            exitPanelCanvasGroup.interactable = false;
            exitPanelCanvasGroup.blocksRaycasts = false;

            LeanTween.alphaCanvas(exitPanelCanvasGroup, 0f, animationDuration)
                .setEase(easeOut)
                .setOnComplete(() =>
                {
                    isAnimating = false;
                    isPanelVisible = false;
                    exitPanel.gameObject.SetActive(false);
                });
        }
        else
        {
            LeanTween.delayedCall(animationDuration, () =>
            {
                isAnimating = false;
                isPanelVisible = false;
                exitPanel.gameObject.SetActive(false);
            });
        }
    }

    private void ExitApplication()
    {
        // Add any cleanup code here before quitting
        Debug.Log("Exiting application");
        Application.Quit();

        // This line is useful for testing in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        if (confirmExitButton != null)
            confirmExitButton.onClick.RemoveListener(ExitApplication);

        if (cancelExitButton != null)
            cancelExitButton.onClick.RemoveListener(HideExitPanel);

        // Clean up input action
        if (backAction != null)
        {
            backAction.Disable();
        }
    }
}

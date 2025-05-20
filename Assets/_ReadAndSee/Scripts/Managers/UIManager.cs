using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Quiz UI References")]
    [SerializeField] VideoPlayer backgroundVideo;
    [SerializeField] VideoPlayer quizVideo;
    [SerializeField] AudioSource audioSource;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] ImageOptionButton[] imageOptionButtons;
    [SerializeField] TextMeshProUGUI questionCounter;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI titleText;

    [Header("Quiz Panel References")]
    [SerializeField] RectTransform quizPanel;
    [SerializeField] GameObject introPanel;
    [SerializeField] Button playButton;

    private Vector2 hiddenPosition;
    private Vector2 centerPosition;

    [Header("Results UI References")]
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI incorrectAtmpTxt;
    [SerializeField] Transform starsContainer;
    [SerializeField] GameObject withStarPrefab;
    [SerializeField] GameObject noStarPrefab;
    [SerializeField] Image levelThumbnail;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "QuizGame")
        {
            InitializePanels();
            UpdateQuizCard();
        }
        else if (SceneManager.GetActiveScene().name == "Results")
        {
            ShowResults();
        }
    }

    public void InitializePanels()
    {
        centerPosition = Vector2.zero;
        float screenWidth = ((RectTransform)quizPanel.parent).rect.width;
        hiddenPosition = new Vector2(screenWidth, 0);
        quizPanel.anchoredPosition = hiddenPosition;
    }

    public void UpdateQuizCard()
    {
        Question currentQuestion = GameManager.Instance.GetCurrentQuestion();

        if (currentQuestion != null)
        {
            // Update question text
            questionText.text = currentQuestion.questionText;

            // Update options
            Sprite[] shuffledOptions = currentQuestion.imageOptions.OrderBy(x => Random.value).ToArray();

            for (int i = 0; i < imageOptionButtons.Length; i++)
            {
                if (i < shuffledOptions.Length)
                {
                    imageOptionButtons[i].gameObject.SetActive(true);
                    int originalIndex = System.Array.IndexOf(currentQuestion.imageOptions, shuffledOptions[i]);
                    imageOptionButtons[i].Setup(shuffledOptions[i], originalIndex);
                }
                else
                {
                    imageOptionButtons[i].gameObject.SetActive(false);
                }
            }

            // Update progress
            int currentIndex = GameManager.Instance.CurrentQuestionIndex + 1;
            int total = GameManager.Instance.TotalQuestions;
            questionCounter.text = $"Question {currentIndex}/{total}";

            // Update score
            titleText.text = GameManager.Instance.CurrentLevel;
            scoreText.text = $"Score: {GameManager.Instance.CorrectAnswers}";
        }
    }

    public IEnumerator PlayVideo(VideoClip video)
    {
        quizPanel.gameObject.SetActive(false);
        quizVideo.gameObject.SetActive(true);
        quizVideo.clip = video;

        if (video != null)
        {
            quizVideo.Play();
            yield return new WaitForSeconds((float)video.length + 1f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.EndGame();
    }

    public void ShowQuizPanel()
    {
        StartCoroutine(PlayClip(GameManager.Instance.Level.titleClip));
    }

    private IEnumerator PlayClip(AudioClip clip)
    {
        playButton.interactable = false;

        if (clip != null)
        {
            AudioManager.Instance.PlaySound(clip);
            yield return new WaitForSeconds(clip.length);
        }
        else
        {
            yield return new WaitForSeconds(1f); // fallback wait
        }

        LeanTween.scale(introPanel, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack);
        LeanTween.move(quizPanel, centerPosition, 0.5f).setEase(LeanTweenType.easeOutExpo);
    }

    private void ShowResults()
    {
        playerName.text = $"Player: {SaveManager.Instance.CurrentPlayerData.playerName}";
        incorrectAtmpTxt.text = $"Incorrect attempts: {GameManager.Instance.TotalQuestions - GameManager.Instance.CorrectAnswers}";
        levelThumbnail.sprite = QuizDatabase.Instance.GetLevel(GameManager.Instance.CurrentLevel).thumbnail;
        int stars = SaveManager.Instance.CurrentPlayerData.GetLevelStars(GameManager.Instance.CurrentLevel);
        DisplayStars(stars);
    }

    public void DisplayStars(int starCount)
    {
        foreach (Transform child in starsContainer) Destroy(child.gameObject);

        const int totalStars = 3;
        for (int i = 0; i < starCount && i < totalStars; i++) Instantiate(withStarPrefab, starsContainer);
        for (int i = starCount; i < totalStars; i++) Instantiate(noStarPrefab, starsContainer);
    }

    public void PauseUI()
    {
        if (backgroundVideo != null && backgroundVideo.isPlaying)
            backgroundVideo.Pause();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    public void ResumeUI()
    {
        if (backgroundVideo != null && !backgroundVideo.isPlaying)
            backgroundVideo.Play();

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Play();
    }
}
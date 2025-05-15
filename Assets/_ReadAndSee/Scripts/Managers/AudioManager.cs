using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip correctAnswerSound;
    [SerializeField] private AudioClip wrongAnswerSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip gameOverSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCorrectSound()
    {
        PlaySound(correctAnswerSound);
    }

    public void PlayWrongSound()
    {
        PlaySound(wrongAnswerSound);
    }

    public void PlayButtonClickSound()
    {
        PlaySound(buttonClickSound);
    }

    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);
    }

    public AudioSource PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        return audioSource;
    }
}
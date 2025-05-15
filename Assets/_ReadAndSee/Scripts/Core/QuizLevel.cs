using UnityEngine;

[CreateAssetMenu(fileName = "QuizLevel", menuName = "Scriptable Objects/QuizLevel")]
public class QuizLevel : ScriptableObject
{
    public string levelName;
    public Difficulty difficulty;
    public Sprite thumbnail;
    public AudioClip titleClip;
    public Question[] questions;
}

[System.Serializable]
public class Question
{
    [TextArea(3, 5)]
    public string questionText;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public Sprite[] imageOptions;
    public int correctAnswerIndex;
}
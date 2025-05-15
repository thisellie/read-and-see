using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuizLevel", menuName = "Quiz/Quiz Level", order = 1)]
[Serializable]
public class QuizLevel : ScriptableObject 
{
    public string levelName;
    public Difficulty difficulty;
    public Sprite thumbnail;
    public AudioClip titleClip;
    public Question[] questions;
}

[Serializable]
public class Question
{
    [TextArea(3, 5)] 
    public string questionText;

    public AudioClip correctClip;
    public AudioClip wrongClip;

    public Sprite[] imageOptions;

    public int correctAnswerIndex;
}

// https://discussions.unity.com/t/unreliable-scriptableobject-assets-associated-script-cannot-be-loaded/627713/5

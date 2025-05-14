using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] imageOptionPaths;
    public Sprite[] imageOptions;
    public int correctAnswerIndex;
}

[System.Serializable]
public class QuizCategory
{
    public string categoryName;
    public string difficulty;
    public Question[] questions;
}
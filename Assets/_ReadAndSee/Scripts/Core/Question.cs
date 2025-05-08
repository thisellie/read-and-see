using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] imageOptionPaths;
    public Sprite[] imageOptions;
    public int correctAnswerIndex;
    public string explanation;
    public int pointValue = 10;
}

[System.Serializable]
public class QuizCategory
{
    public string categoryName;
    public Question[] questions;
}
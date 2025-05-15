using UnityEngine;

[CreateAssetMenu(fileName = "NewQuizCategory", menuName = "Quiz/Quiz Category", order = 1)]
public class QuizCategory : ScriptableObject 
{
    public string categoryName;
    public DifficultyLevel difficulty; 
    public Question[] questions;
}

[System.Serializable]
public class Question
{
    [TextArea(3, 5)] 
    public string questionText;

    public Sprite[] imageOptions;
     public string[] imageOptionPaths;

    public int correctAnswerIndex;
}

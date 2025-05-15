using UnityEngine;

[CreateAssetMenu(fileName = "NewQuizLevel", menuName = "Quiz/Quiz Level", order = 1)]
public class QuizLevel : ScriptableObject 
{
    public string levelName;
    public Difficulty difficulty; 
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

using System.Collections.Generic;
using UnityEngine;

public class MathQuestionStage : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    [SerializeField] private MathCourseBoard board;
    [SerializeField] private MathAnswerPlatform[] answerPlatforms;
    [SerializeField] private GameObject correctBridge;

    private string question;
    private bool solved;

    public int StageNumber => stageNumber;
    public bool Solved => solved;
    public string Question => question;

    private void Awake()
    {
        if (board != null)
            board.Register(this);
    }

    private void Start()
    {
        int correctAnswer = GenerateQuestion();
        int correctSlot = Random.Range(0, answerPlatforms.Length);
        HashSet<int> choices = new() { correctAnswer };

        for (int i = 0; i < answerPlatforms.Length; i++)
        {
            int answer = correctAnswer;
            if (i != correctSlot)
            {
                do
                {
                    answer = Mathf.Max(1, correctAnswer + Random.Range(-10, 11));
                } while (!choices.Add(answer));
            }

            answerPlatforms[i].Configure(this, answer, i == correctSlot);
        }

        if (board != null)
            board.Refresh();
    }

    public void Solve()
    {
        if (solved)
            return;
        solved = true;
        foreach (MathAnswerPlatform answer in answerPlatforms)
            answer.DropIfWrong();
        if (correctBridge != null)
            correctBridge.SetActive(true);
        if (board != null)
            board.Refresh();
    }

    private int GenerateQuestion()
    {
        int a;
        int b;
        int answer;
        switch (Random.Range(0, 4))
        {
            case 0:
                a = Random.Range(2, 13);
                b = Random.Range(2, 13);
                question = $"{a} × {b}";
                return a * b;
            case 1:
                b = Random.Range(2, 13);
                answer = Random.Range(2, 13);
                a = b * answer;
                question = $"{a} ÷ {b}";
                return answer;
            case 2:
                a = Random.Range(10, 51);
                b = Random.Range(2, 31);
                question = $"{a} + {b}";
                return a + b;
            default:
                answer = Random.Range(2, 41);
                b = Random.Range(2, 31);
                a = answer + b;
                question = $"{a} − {b}";
                return answer;
        }
    }
}

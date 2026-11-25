using UnityEngine;
using TMPro;

public class MathShootGate : MonoBehaviour
{
    [Header("Pengaturan Acak Otomatis")]
    public bool autoRandomQuestion = true;
    public TMP_Text questionBoardText;

    [Header("Soal Manual (Jika Acak Dimatikan)")]
    public string manualQuestion = "12 + 8 = ?";
    public string[] manualOptions = new string[3] { "18", "20", "24" };
    [Range(0, 2)] public int manualCorrectIndex = 1;

    [Header("3 Kotak Target & Pintu Penghalang")]
    public MathShootTarget[] answerTargets;
    public Transform gateBarrier;
    public Vector3 openOffset = new Vector3(0f, 5f, 0f);
    public float openSpeed = 4f;

    private bool isSolved = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        if (gateBarrier != null)
        {
            closedPosition = gateBarrier.position;
            openPosition = closedPosition + openOffset;
        }

        if (autoRandomQuestion)
            GenerateRandomQuestion();
        else
            SetupManualQuestion();
    }

    void GenerateRandomQuestion()
    {
        int operation = Random.Range(0, 3);
        int a = 0, b = 0, correctAnswer = 0;
        string generatedText = "";

        if (operation == 0)
        {
            a = Random.Range(2, 21);
            b = Random.Range(2, 21);
            correctAnswer = a + b;
            generatedText = a + " + " + b + " = ?";
        }
        else if (operation == 1)
        {
            b = Random.Range(2, 16);
            correctAnswer = Random.Range(2, 21);
            a = correctAnswer + b;
            generatedText = a + " - " + b + " = ?";
        }
        else
        {
            a = Random.Range(2, 10);
            b = Random.Range(2, 10);
            correctAnswer = a * b;
            generatedText = a + " x " + b + " = ?";
        }

        if (questionBoardText != null)
            questionBoardText.text = generatedText;

        int[] offsets = new int[] { -3, -2, -1, 1, 2, 3, 4, 5 };
        int wrong1 = correctAnswer + offsets[Random.Range(0, offsets.Length)];
        if (wrong1 < 0 || wrong1 == correctAnswer) wrong1 = correctAnswer + 2;

        int wrong2 = correctAnswer + offsets[Random.Range(0, offsets.Length)];
        while (wrong2 < 0 || wrong2 == correctAnswer || wrong2 == wrong1)
            wrong2 += 1;

        int correctSlot = Random.Range(0, answerTargets.Length);
        int[] wrongPool = new int[] { wrong1, wrong2 };
        int wrongUsed = 0;

        for (int i = 0; i < answerTargets.Length; i++)
        {
            if (answerTargets[i] == null) continue;

            if (i == correctSlot)
                answerTargets[i].Setup(this, correctAnswer.ToString(), true);
            else
                answerTargets[i].Setup(this, wrongPool[wrongUsed++].ToString(), false);
        }
    }

    void SetupManualQuestion()
    {
        if (questionBoardText != null)
            questionBoardText.text = manualQuestion;

        for (int i = 0; i < answerTargets.Length; i++)
        {
            if (answerTargets[i] != null)
            {
                string label = i < manualOptions.Length ? manualOptions[i] : "";
                answerTargets[i].Setup(this, label, i == manualCorrectIndex);
            }
        }
    }

    void Update()
    {
        if (isSolved && gateBarrier != null)
            gateBarrier.position = Vector3.MoveTowards(gateBarrier.position, openPosition, openSpeed * Time.deltaTime);
    }

    public void OnTargetShot(MathShootTarget shotTarget, bool isCorrect)
    {
        if (isSolved) return;

        if (isCorrect)
        {
            isSolved = true;
            shotTarget.SetFeedbackColor(Color.green);

            for (int i = 0; i < answerTargets.Length; i++)
            {
                if (answerTargets[i] != null)
                {
                    Collider col = answerTargets[i].GetComponent<Collider>();
                    if (col != null) col.enabled = false;
                }
            }

            Invoke(nameof(HideTargets), 0.35f);
        }
        else
        {
            shotTarget.SetFeedbackColor(Color.red);
        }
    }

    void HideTargets()
    {
        for (int i = 0; i < answerTargets.Length; i++)
        {
            if (answerTargets[i] != null)
                answerTargets[i].gameObject.SetActive(false);
        }
    }
}
using UnityEngine;
using TMPro;

public class MathShootTarget : MonoBehaviour
{
    public TMP_Text optionText;

    private MathShootGate parentGate;
    private bool isCorrectAnswer;
    private Renderer targetRenderer;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    public void Setup(MathShootGate gate, string label, bool isCorrect)
    {
        parentGate = gate;
        isCorrectAnswer = isCorrect;

        if (optionText != null)
            optionText.text = label;
    }

    public void OnHit()
    {
        if (parentGate != null)
            parentGate.OnTargetShot(this, isCorrectAnswer);
    }

    public void SetFeedbackColor(Color color)
    {
        if (targetRenderer != null)
            targetRenderer.material.color = color;
    }
}
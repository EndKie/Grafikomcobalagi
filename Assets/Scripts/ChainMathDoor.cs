using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChainMathDoor : MonoBehaviour
{
    [SerializeField] private TMP_Text answerLabel;

    private ChainMathCourse course;

    public int StageIndex { get; private set; }
    public bool IsCorrect { get; private set; }

    public void Configure(ChainMathCourse owner, int stageIndex, bool correct, int answer)
    {
        course = owner;
        StageIndex = stageIndex;
        IsCorrect = correct;
        if (answerLabel != null)
            answerLabel.text = answer.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.GetComponentInParent<CharacterController>();
        if (!other.CompareTag("Player") && (controller == null || !controller.CompareTag("Player")))
            return;

        Transform player = controller != null ? controller.transform : other.transform;
        if (course != null)
            course.SelectDoor(this, player);
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MathAnswerPlatform : MonoBehaviour
{
    [SerializeField] private TMP_Text answerLabel;
    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Material correctMaterial;
    [SerializeField] private float fallDelay = 0.2f;

    private MathQuestionStage stage;
    private bool isCorrect;
    private bool activated;

    public void Configure(MathQuestionStage owner, int answer, bool correct)
    {
        stage = owner;
        isCorrect = correct;
        activated = false;
        answerLabel.text = answer.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated || (!other.CompareTag("Player") && other.GetComponentInParent<CharacterController>() == null))
            return;

        activated = true;
        if (isCorrect)
        {
            visualRenderer.sharedMaterial = correctMaterial;
            stage.Solve();
            return;
        }

        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = false;
        body.useGravity = true;
    }
}

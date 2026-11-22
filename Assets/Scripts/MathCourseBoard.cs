using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathCourseBoard : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private readonly List<MathQuestionStage> stages = new();

    public void Register(MathQuestionStage stage) => stages.Add(stage);

    public void Refresh()
    {
        label ??= GetComponentInChildren<TMP_Text>(true);
        if (label == null)
        {
            Debug.LogError("MathCourseBoard has no TMP_Text label.", this);
            return;
        }

        MathQuestionStage current = null;
        foreach (MathQuestionStage stage in stages)
            if (!stage.Solved && (current == null || stage.StageNumber < current.StageNumber))
                current = stage;

        if (current == null)
        {
            label.text = "<b>SELESAI</b>";
            return;
        }

        label.text = $"<b>{(current.Question ?? "1+1").Replace(" ", "")}=?</b>";
    }
}

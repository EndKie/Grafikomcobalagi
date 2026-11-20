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
        MathQuestionStage current = null;
        foreach (MathQuestionStage stage in stages)
            if (!stage.Solved && (current == null || stage.StageNumber < current.StageNumber))
                current = stage;

        label.text = current == null
            ? "<b>SEMUA SOAL SELESAI</b>\nLANJUT KE FINISH →"
            : $"<b>SOAL {current.StageNumber}/10</b>\n{current.Question} = ?";
    }
}

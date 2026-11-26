
using UnityEngine;
using UnityEngine.UI;

public class WaterLevelUI : MonoBehaviour
{
    [Header("Water Reference")]
    public Transform waterTransform;

    [Header("UI Reference")]
    public Image waterFill;
    public Text waterLevelText;

    [Header("Water Level Range")]
    public float minimumHeight = -1.5f;
    public float maximumHeight = 10f;

    void Update()
    {
        if (waterTransform == null)
            return;

        float currentHeight = waterTransform.position.y;

        float level = Mathf.InverseLerp(
            minimumHeight,
            maximumHeight,
            currentHeight
        );

        if (waterFill != null)
            waterFill.fillAmount = level;

        if (waterLevelText != null)
        {
            int percentage = Mathf.RoundToInt(level * 100f);
            waterLevelText.text = percentage + "%";
        }
    }
}

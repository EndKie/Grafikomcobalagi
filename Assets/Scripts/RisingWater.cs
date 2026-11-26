
using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [Header("Pengaturan Kenaikan Air")]
    public float riseSpeed = 0.25f;
    public bool isRising = true;

    void Update()
    {
        if (isRising)
        {
            transform.Translate(
                Vector3.up * riseSpeed * Time.deltaTime,
                Space.World
            );
        }
    }
}

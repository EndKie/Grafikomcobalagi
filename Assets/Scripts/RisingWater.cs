using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [Header("Pengaturan Kenaikan Air")]
    public float riseSpeed = 0.25f; // Kecepatan air naik (meter per detik)
    public bool isRising = true;

    void Update()
    {
        if (isRising)
        {
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
        }
    }
}
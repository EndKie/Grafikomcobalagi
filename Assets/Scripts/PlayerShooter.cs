using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Pengaturan Tembakan")]
    public Camera fpsCamera;
    public float shootRange = 50f;
    public LayerMask hitLayers = ~0; // Mendeteksi semua layer secara default

    void Update()
    {
        // Deteksi klik kiri mouse menggunakan Input System baru
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (fpsCamera == null)
        {
            Debug.LogWarning("Slot Fps Camera belum diisi pada PlayerShooter!");
            return;
        }

        // Tembakkan sinar tepat dari titik tengah layar (0.5, 0.5)
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Gambar garis merah di tab Scene selama 1 detik untuk memudahkan debugging
        Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.red, 1.0f);

        // QueryTriggerInteraction.Ignore memastikan tembakan menembus trigger Checkpoint & Water
        if (Physics.Raycast(ray, out hit, shootRange, hitLayers, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Tembakan mengenai: " + hit.collider.name);

            // Cek apakah objek yang ditembak memiliki komponen ShootableTarget
            ShootableTarget target = hit.collider.GetComponent<ShootableTarget>();
            if (target != null)
            {
                target.OnHit();
            }
        }
        else
        {
            Debug.Log("Tembakan meleset.");
        }
    }
}
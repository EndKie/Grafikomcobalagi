using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Pengaturan Senjata & Peluru")]
    public Camera fpsCamera;
    public Transform firePoint;          // Objek FirePoint di ujung laras
    public GameObject projectilePrefab;  // Prefab peluru (BulletPrefab)
    public float maxAimRange = 100f;
    public LayerMask aimLayers = ~0;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (fpsCamera == null || firePoint == null || projectilePrefab == null)
        {
            Debug.LogWarning("Slot Fps Camera, Fire Point, atau Projectile Prefab belum diisi!");
            return;
        }

        // 1. Cari titik bidik tepat di tengah layar
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimRange, aimLayers, QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxAimRange);
        }

        // 2. Arahkan peluru dari ujung laras menuju titik tengah layar
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        // 3. Munculkan peluru fisik di FirePoint
        Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
    }
}
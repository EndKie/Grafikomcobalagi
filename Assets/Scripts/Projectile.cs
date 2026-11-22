using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Pengaturan Peluru")]
    public float speed = 40f;
    public float lifeTime = 3f;

    void Start()
    {
        // Hancurkan peluru otomatis setelah 3 detik jika meleset
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Gerakkan peluru lurus ke depan mengikuti arah sumbu Z
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Abaikan area trigger milik Player, Checkpoint, dan Water
        if (other.CompareTag("Player") || other.CompareTag("Checkpoint") || other.CompareTag("Water"))
            return;

        // Deteksi komponen ShootableTarget pada objek yang terkena tembakan
        ShootableTarget target = other.GetComponentInParent<ShootableTarget>();
        if (target != null)
        {
            target.OnHit();
        }

        // Hancurkan peluru saat menyentuh target, dinding, atau platform
        Destroy(gameObject);
    }
}
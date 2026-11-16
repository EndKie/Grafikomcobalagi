using UnityEngine;

public class ShootableTarget : MonoBehaviour
{
    public void OnHit()
    {
        Debug.Log("TARGET BERHASIL DITEMBAK: " + gameObject.name);

        // Ubah warna target menjadi hijau sebagai tanda visual sudah tertembak
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.green;
        }
    }
}

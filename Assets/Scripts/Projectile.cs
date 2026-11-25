using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Checkpoint") || other.CompareTag("Water"))
            return;

        ShootableTarget target = other.GetComponentInParent<ShootableTarget>();
        if (target != null)
            target.OnHit();

        MathShootTarget mathTarget = other.GetComponentInParent<MathShootTarget>();
        if (mathTarget != null)
            mathTarget.OnHit();

        Destroy(gameObject);
    }
}
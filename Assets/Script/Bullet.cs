using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private float lifeTime;
    private float damage;

    private Vector3 moveDirection;

    private LayerMask hitMask;

    public void Initialize(
        Vector3 dir,
        float bulletSpeed,
        float bulletLifeTime,
        float bulletDamage,
        LayerMask bulletHitMask)
    {
        moveDirection = dir.normalized;

        speed = bulletSpeed;
        lifeTime = bulletLifeTime;
        damage = bulletDamage;

        hitMask = bulletHitMask;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position +=
            moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask) != 0)
        {
            Destroy(gameObject);
        }
    }
}
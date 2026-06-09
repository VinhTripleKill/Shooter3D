using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private float lifeTime;
    private float damage;

    private Vector3 moveDirection;

    public void Initialize(
        Vector3 dir,
        float bulletSpeed,
        float bulletLifeTime,
        float bulletDamage
    )
    {
        moveDirection = dir.normalized;

        speed = bulletSpeed;
        lifeTime = bulletLifeTime;
        damage = bulletDamage;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
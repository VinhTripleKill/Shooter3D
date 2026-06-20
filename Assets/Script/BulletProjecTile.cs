using UnityEngine;
public class BulletProjecTile : MonoBehaviour
{
    public enum BulletType{ bullet, missile }
    private float speed;
    private float lifeTime;
    private float damage;
    public BulletType bulletType;
    private Vector3 moveDirection;
    [SerializeField]
    private float rotateSpeed = 180f;

    private Transform target;
    private Rigidbody rb;
    private LayerMask hitMask;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Initialize(
        Vector3 dir,
        float bulletSpeed,
        float bulletLifeTime,
        float bulletDamage,
        LayerMask bulletHitMask,
        Transform targetTransform = null)
    {
        moveDirection = dir.normalized;

        speed = bulletSpeed;
        lifeTime = bulletLifeTime;
        damage = bulletDamage;

        hitMask = bulletHitMask;
        target = targetTransform;

        Destroy(gameObject, lifeTime);
    }
    private void FixedUpdate()
    {
        switch (bulletType)
        {
            case BulletType.bullet:
                UpdateNormalBullet();
                break;

            case BulletType.missile:
                UpdateMissile();
                break;
        }
    }
    private void UpdateNormalBullet()
    {
        if (rb != null)
        {
            rb.linearVelocity =
                moveDirection * speed;
        }
        else
        {
            transform.position +=
                moveDirection *
                speed *
                Time.fixedDeltaTime;
        }
    }
    private void UpdateMissile()
    {
        if (target == null)
        {
            if (rb != null)
                rb.linearVelocity =
                    transform.forward * speed;

            return;
        }

        Vector3 direction =
            (target.position - transform.position)
            .normalized;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.fixedDeltaTime);

        if (rb != null)
        {
            rb.linearVelocity =
                transform.forward * speed;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask) == 0)
            return;

        IDamageable damageable =
     other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
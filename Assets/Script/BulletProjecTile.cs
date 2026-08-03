using UnityEngine;
using System;
public class BulletProjectile : MonoBehaviour
{
  
    private float speed;
    private float damage;
    private Vector3 moveDirection;
    private float maxDistance;
    private Vector3 startPosition;
    private Rigidbody rb;
    private LayerMask hitMask;
    public static event Action OnSuccessfulHit;
    private LayerMask interactionMask;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Initialize(
    Vector3 dir,
    float bulletSpeed,
    float bulletDamage,
    float bulletRange,
    LayerMask bulletHitMask,
    LayerMask bulletInteractionMask)
{
    moveDirection = dir.normalized;

    speed = bulletSpeed;
    damage = bulletDamage;

    maxDistance = bulletRange;
    startPosition = transform.position;

    hitMask = bulletHitMask;
    interactionMask = bulletInteractionMask;
}
    private void FixedUpdate()
    {
        UpdateNormalBullet();
        if ((transform.position - startPosition).sqrMagnitude >= maxDistance * maxDistance)
{
    Destroy(gameObject);
}
    }
    private void UpdateNormalBullet()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }
        else
        {
            transform.position += moveDirection * speed * Time.fixedDeltaTime;
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

        if (((1 << other.gameObject.layer) & interactionMask) != 0)
        {
            OnSuccessfulHit?.Invoke();
        }
    }

    Destroy(gameObject);
}
   
}
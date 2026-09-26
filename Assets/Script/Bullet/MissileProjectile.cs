using UnityEngine;
using System;

public class MissileProjectile : MonoBehaviour
{
    private float speed;
    private float lifeTime;
    private float damage;

    [SerializeField]
    private float rotateSpeed = 180f;

    private Transform target;
    private Rigidbody rb;

    private LayerMask hitMask;
    private LayerMask interactionMask;

    public static event Action OnSuccessfulHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(
        float missileSpeed,
        float missileLifeTime,
        float missileDamage,
        LayerMask missileHitMask,
        LayerMask missileInteractionMask,
        Transform targetTransform)
    {
        speed = missileSpeed;
        lifeTime = missileLifeTime;
        damage = missileDamage;

        hitMask = missileHitMask;
        interactionMask = missileInteractionMask;
        target = targetTransform;

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            if (rb != null)
                rb.linearVelocity = transform.forward * speed;

            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.fixedDeltaTime);

        if (rb != null)
            rb.linearVelocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask) == 0)
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            if (((1 << other.gameObject.layer) & interactionMask) != 0)
                OnSuccessfulHit?.Invoke();
        }

        Destroy(gameObject);
    }
}
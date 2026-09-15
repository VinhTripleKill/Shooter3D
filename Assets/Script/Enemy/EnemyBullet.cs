using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed;
    private float damage;
    private float maxDistance;

    private Vector3 moveDirection;
    private Vector3 startPosition;

    private Rigidbody rb;

    private LayerMask hitMask;
    private LayerMask obstacleMask;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize(
        Vector3 dir,
        float bulletSpeed,
        float bulletDamage,
        float bulletRange,
        LayerMask bulletHitMask,
        LayerMask bulletObstacleMask)
    {
        moveDirection =
            dir.normalized;

        speed =
            bulletSpeed;

        damage =
            bulletDamage;

        maxDistance =
            bulletRange;

        startPosition =
            transform.position;

        hitMask =
            bulletHitMask;

        obstacleMask =
            bulletObstacleMask;
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void FixedUpdate()
    {
        MoveBullet();

        // =====================================================
        // BULLET RANGE
        // =====================================================

        if (
            (transform.position - startPosition).sqrMagnitude
            >= maxDistance * maxDistance
        )
        {
            Destroy(gameObject);
        }
    }

    // =========================================================
    // MOVE
    // =========================================================

    private void MoveBullet()
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

    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        int otherLayerMask =
            1 << other.gameObject.layer;

        // =====================================================
        // OBSTACLE
        // =====================================================

        if ((otherLayerMask & obstacleMask) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // =====================================================
        // PLAYER
        // =====================================================

        if ((otherLayerMask & hitMask) == 0)
            return;

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(
                damage
            );
        }

        Destroy(gameObject);
    }
}
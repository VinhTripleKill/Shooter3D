using System.Collections;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private GrenadeSkillData data;

    private Rigidbody rb;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private bool exploded;
    private bool initialized;

    // =============================================================
    // INITIALIZE
    // =============================================================

    public void Initialize(GrenadeSkillData skillData,Vector3 start,Vector3 target)
    {
        data = skillData;

        startPosition = start;
        targetPosition = target;

        exploded = false;
        initialized = true;

        // =========================================================
        // GET RIGIDBODY
        // =========================================================

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("[GRENADE] Grenade prefab phải có Rigidbody!");

            Destroy(gameObject);

            return;
        }

        // =========================================================
        // PHẢI DÙNG PHYSICS THẬT
        // =========================================================

        rb.isKinematic = false;

        // Bạn có thể tự bật/tắt Use Gravity trong prefab.
        // Nhưng với hệ thống này nên để Use Gravity = ON.

        // =========================================================
        // COLLISION DETECTION
        // =========================================================

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // =========================================================
        // TÍNH INITIAL VELOCITY
        // =========================================================

        Vector3 initialVelocity = CalculateLaunchVelocity( startPosition, targetPosition );

        rb.linearVelocity = initialVelocity;

        // =========================================================
        // EXPLODE DELAY
        // =========================================================

        StartCoroutine( ExplodeDelayRoutine() );

        Debug.Log( $"[GRENADE] Launch velocity = " + $"{initialVelocity}" );
    }

    // =============================================================
    // TÍNH VẬN TỐC NÉM
    // =============================================================

    private Vector3 CalculateLaunchVelocity( Vector3 start, Vector3 target)
    {
        Vector3 displacement = target - start;

        // =========================================================
        // GRAVITY
        // =========================================================

        float gravity = Mathf.Abs( Physics.gravity.y );

        gravity = Mathf.Max( gravity, 0.01f );

        // =========================================================
        // TÍNH KHOẢNG CÁCH NGANG
        // =========================================================

        Vector3 horizontal = new Vector3( displacement.x, 0f, displacement.z );

        float horizontalDistance = horizontal.magnitude;

        // =========================================================
        // TÍNH ĐỘ CAO ĐỈNH PARABOL
        // =========================================================

        float arcHeight = Mathf.Max( data.minArcHeight, horizontalDistance * data.arcHeightPerMeter );

        // Đỉnh quỹ đạo nằm cao hơn điểm bắt đầu
        // khoảng arcHeight.

        float apexHeight = start.y + arcHeight;

        // =========================================================
        // VẬN TỐC Y ĐỂ ĐẠT ĐỈNH
        // =========================================================

        float verticalVelocity = Mathf.Sqrt( 2f * gravity * Mathf.Max( 0.01f, apexHeight - start.y ) );

        // =========================================================
        // THỜI GIAN LÊN ĐẾN ĐỈNH
        // =========================================================

        float timeUp = verticalVelocity / gravity;

        // =========================================================
        // TÍNH THỜI GIAN TỪ ĐỈNH XUỐNG TARGET
        // =========================================================

        float heightFromApexToTarget = apexHeight - target.y;

        heightFromApexToTarget = Mathf.Max( heightFromApexToTarget, 0.01f );

        float timeDown = Mathf.Sqrt( 2f * heightFromApexToTarget / gravity );

        float totalFlightTime = timeUp + timeDown;

        totalFlightTime = Mathf.Max( totalFlightTime, 0.01f );

        // =========================================================
        // VẬN TỐC NGANG
        // =========================================================

        Vector3 horizontalVelocity = horizontal / totalFlightTime;

        // =========================================================
        // FINAL VELOCITY
        // =========================================================

        Vector3 launchVelocity = horizontalVelocity;

        launchVelocity.y = verticalVelocity;

        return launchVelocity;
    }

    // =============================================================
    // EXPLODE DELAY
    // =============================================================

    private IEnumerator ExplodeDelayRoutine()
    {
        yield return new WaitForSeconds( data.explodeDelay );

        if (exploded) yield break;

        Debug.Log( "[GRENADE] Explode bởi explodeDelay." );

        Explode();
    }

    // =============================================================
    // COLLISION
    // =============================================================

    private void OnCollisionEnter(Collision collision)
    {
        if (!initialized) return;

        if (exploded) return;

        // =========================================================
        // CHECK HIT MASK
        // =========================================================

        int layerBit = 1 << collision.gameObject.layer;

        bool isHitMask = (data.hitMask.value & layerBit) != 0;

        // =========================================================
        // KHÔNG THUỘC HIT MASK
        // =========================================================

        if (!isHitMask)
        {
            Debug.Log($"[GRENADE] Va vào " +$"{collision.gameObject.name} " +$"nhưng không thuộc hitMask.");

            // Không explode.
            //
            // Rigidbody sẽ tự:
            // - bounce
            // - rơi
            // - nằm xuống
            //
            // Sau explodeDelay sẽ tự explode.

            return;
        }

        // =========================================================
        // THUỘC HIT MASK
        // =========================================================

        Debug.Log( $"[GRENADE] Hit hitMask: " + $"{collision.gameObject.name}" );

        Explode();
    }

    // =============================================================
    // EXPLODE
    // =============================================================

    private void Explode()
    {
        if (exploded) return;

        exploded = true;

        StopAllCoroutines();

        Vector3 explosionPosition = transform.position;

        Debug.Log( $"[GRENADE] EXPLODE tại " + $"{explosionPosition}" );

        // =========================================================
        // SPAWN EXPLOSION
        // =========================================================

        if (data.explosionPrefab != null)
        {
            GameObject explosion = Instantiate( data.explosionPrefab, explosionPosition, Quaternion.identity );

            ExplosionEffect effect = explosion.GetComponent<ExplosionEffect>();

            if (effect != null)
            {
                effect.Initialize(data);
            }
        }
        Destroy(gameObject);
    }
}
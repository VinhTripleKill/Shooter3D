using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrenadeProjectile : MonoBehaviour
{
    private GrenadeSkillData data;

    private Rigidbody rb;

    private Vector3 startPoint;
    private Vector3 targetPoint;

    private float fireTime;
    private float flightDuration;

    private float progress;
    private float arcHeight;

    private bool initialized;
    private bool exploded;
    private bool bouncedFromWall;

    // =============================================================
    // INITIALIZE
    // =============================================================

    public void Initialize( GrenadeSkillData skillData, Vector3 start, Vector3 target)
    {
        data = skillData;

        startPoint = start;
        targetPoint = target;

        exploded = false;
        bouncedFromWall = false;
        initialized = true;

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Destroy(gameObject);
            return;
        }

        // =========================================================
        // KINEMATIC PARABOLA MODE
        // =========================================================

        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = startPoint;

        // =========================================================
        // TÍNH PARABOLA
        // =========================================================

        float distance = Vector3.Distance( startPoint, targetPoint );

        float distance01 = Mathf.Clamp01( distance / Mathf.Max(data.distanceForMaxArc, 0.01f ) );

        arcHeight = Mathf.Lerp( data.arcHeightNear, data.arcHeightFar, distance01 );

        // =========================================================
        // THỜI GIAN BAY
        // =========================================================
     
        // Không ép grenade phải tới target trong 1s / 2s.
        //
        // Khoảng cách càng xa -> thời gian càng lâu.
        // moveSpeed chỉ điều chỉnh tốc độ.
        //
        // moveSpeed = 8
        // moveSpeed = 16 -> bay nhanh gấp 2
        //
        // Quỹ đạo vẫn giữ nguyên.
        // =========================================================

        float speed = Mathf.Max( data.moveSpeed, 0.01f );

        flightDuration = distance / speed;

        flightDuration = Mathf.Max( flightDuration, 0.01f );

        fireTime = Time.time;
        progress = 0f;

        // =========================================================
        // EXPLOSION DELAY
        // =========================================================

        StartCoroutine( ExplodeDelayRoutine() );
    }

    // =============================================================
    // FIXED UPDATE
    // =============================================================

    private void FixedUpdate()
    {
        if (!initialized) return;

        float previousProgress = progress;

        progress += Time.fixedDeltaTime / flightDuration;

        progress = Mathf.Clamp01(progress);

        Vector3 previousPosition = GetParabolaPosition( previousProgress );

        Vector3 nextPosition = GetParabolaPosition( progress );

        Vector3 movement = nextPosition - previousPosition;

        float distance = movement.magnitude;

        // =========================================================
        // WALL CHECK
        // =========================================================

        if (distance > 0f)
        {
            if (Physics.Raycast(
                    previousPosition,
                    movement.normalized,
                    out RaycastHit wallHit,
                    distance + 0.05f,
                    data.wallMask,
                    QueryTriggerInteraction.Ignore))
            {
                HitWall( wallHit, movement.normalized );

                return;
            }
        }

        // =========================================================
        // MOVE
        // =========================================================

        rb.MovePosition( nextPosition );

        // =========================================================
        // ROTATION
        // =========================================================

        rb.MoveRotation(
            rb.rotation *
            Quaternion.Euler(
                data.speedRotX *
                    Time.fixedDeltaTime,

                data.speedRotY *
                    Time.fixedDeltaTime,

                data.speedRotZ *
                    Time.fixedDeltaTime
            )
        );

        // =========================================================
        // ARRIVE TARGET
        // =========================================================

        if (progress >= 1f)
        {
            ArriveAtTarget();
        }
    }

    // =============================================================
    // PARABOLA
    // =============================================================

    private Vector3 GetParabolaPosition(float t)
    {
        Vector3 position =
            Vector3.Lerp(
                startPoint,
                targetPoint,
                t
            );

        /*
         * 4t(1-t)
         *
         * t = 0   -> 0
         * t = 0.5 -> 1
         * t = 1   -> 0
         *
         * Vì vậy:
         *
         * Start -> chính xác
         * Target -> chính xác
         */

        float height = 4f * t * (1f - t) * arcHeight;

        position.y += height;

        return position;
    }

    // =============================================================
    // ARRIVE TARGET
    // =============================================================

    private void ArriveAtTarget()
    {
        if (exploded)
            return;

        initialized = false;

        /*
         * Đảm bảo grenade nằm chính xác
         * tại điểm Ground đã raycast.
         */
        rb.position = targetPoint;

        Explode();
    }

    // =============================================================
    // HIT WALL
    // =============================================================

    private void HitWall(
        RaycastHit wallHit,
        Vector3 direction)
    {
        if (exploded) return;

        /*
         * Từ đây grenade không còn đi theo
         * parabola nữa.
         *
         * Nó chuyển sang Physics thật.
         */

        initialized = false;
        bouncedFromWall = true;

        float bounceSpeed =
            Mathf.Max(
                data.moveSpeed *
                    data.wallBounceForce,

                data.minimumBounceSpeed
            );

        Vector3 reflectedDirection =
            Vector3.Reflect(
                direction,
                wallHit.normal
            );

        Vector3 bounceDirection =
            (
                reflectedDirection +
                Vector3.up *
                data.wallUpwardForce
            ).normalized;

        /*
         * Đẩy grenade ra khỏi wall
         * để tránh bị kẹt trong collider.
         */

        rb.position =
            wallHit.point +
            wallHit.normal * 0.02f;

        // =========================================================
        // ENABLE PHYSICS
        // =========================================================

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.linearVelocity =
            bounceDirection *
            bounceSpeed;

        rb.angularVelocity =
            Vector3.zero;
    }

    // =============================================================
    // EXPLOSION DELAY
    // =============================================================

    private IEnumerator ExplodeDelayRoutine()
    {
        yield return new WaitForSeconds(
            data.explodeDelay
        );

        if (exploded)
            yield break;

        Explode();
    }

    // =============================================================
    // COLLISION SAU KHI PHẢN TƯỜNG
    // =============================================================

    private void OnCollisionEnter(
        Collision collision)
    {
        if (exploded)
            return;

        /*
         * Trong giai đoạn parabola:
         *
         * collision không phải cơ chế chính.
         * Wall được detect bằng Raycast.
         *
         * Sau khi bounce:
         * Physics sẽ tự xử lý việc grenade
         * rơi / đập xuống / nằm trên mặt đất.
         *
         * KHÔNG explode khi chạm ground.
         */

        if (!bouncedFromWall)
            return;

        /*
         * Không làm gì cả.
         *
         * Grenade tiếp tục tồn tại cho tới
         * explodeDelay.
         */
    }

    // =============================================================
    // EXPLODE
    // =============================================================

    private void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        StopAllCoroutines();

        Vector3 explosionPosition =
            transform.position;

        // =========================================================
        // SPAWN EXPLOSION
        // =========================================================

        if (data.explosionPrefab != null)
        {
            GameObject explosion =
                Instantiate(
                    data.explosionPrefab,
                    explosionPosition,
                    Quaternion.identity
                );

            ExplosionEffect effect =
                explosion.GetComponent<ExplosionEffect>();

            if (effect != null)
            {
                effect.Initialize(data);
            }
        }

        Destroy(gameObject);
    }

    // =============================================================
    // GIZMOS
    // =============================================================

    private void OnDrawGizmosSelected()
    {
        if (!initialized)
            return;

        Gizmos.color =
            Color.green;

        Vector3 previous =
            GetParabolaPosition(0f);

        const int segments = 30;

        for (int i = 1; i <= segments; i++)
        {
            float t =
                i / (float)segments;

            Vector3 current =
                GetParabolaPosition(t);

            Gizmos.DrawLine(
                previous,
                current
            );

            previous = current;
        }

        Gizmos.color =
            Color.red;

        Gizmos.DrawSphere(
            targetPoint,
            0.1f
        );
    }
}
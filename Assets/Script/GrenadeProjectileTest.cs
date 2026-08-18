using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrenadeProjectileTest : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float arriveDistance = 0.01f;

    [Header("Parabola")]
    [SerializeField] private float arcHeightNear = 0.2f;
    [SerializeField] private float arcHeightFar = 4f;
    [SerializeField] private float distanceForMaxArc = 15f;

    [Header("Rotation")]
    [SerializeField] private float speedRotX = 30f;
    [SerializeField] private float speedRotY = 60f;
    [SerializeField] private float speedRotZ = 90f;

    [Header("Wall")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallBounceForce = 1f;
    [SerializeField] private float wallUpwardForce = 0.25f;
    [SerializeField] private float minimumBounceSpeed = 1f;

    private Rigidbody rb;

    private Vector3 startPoint;
    private Vector3 targetPoint;

    private float fireTime;
    private float flightDuration;

    private float progress;
    private float arcHeight;

    private bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void Initialize(
        Vector3 start,
        Vector3 target)
    {
        startPoint = start;
        targetPoint = target;

        rb.isKinematic = true;
        rb.useGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = startPoint;

        fireTime = Time.time;
        progress = 0f;
        initialized = true;

        float distance =
            Vector3.Distance(startPoint, targetPoint);

        /*
         * Càng xa -> parabola càng cao.
         */
        float distance01 = Mathf.Clamp01(
            distance / distanceForMaxArc
        );

        arcHeight = Mathf.Lerp(
            arcHeightNear,
            arcHeightFar,
            distance01
        );

        /*
         * Thời gian bay dựa trên khoảng cách.
         */
        flightDuration =
            distance / moveSpeed;

        flightDuration =
            Mathf.Max(flightDuration, 0.01f);
    }

    private void FixedUpdate()
    {
        if (!initialized)
            return;

        float previousProgress = progress;

        progress +=
            Time.fixedDeltaTime / flightDuration;

        progress = Mathf.Clamp01(progress);

        Vector3 previousPosition =
            GetParabolaPosition(previousProgress);

        Vector3 nextPosition =
            GetParabolaPosition(progress);

        Vector3 direction =
            nextPosition - previousPosition;

        float distance =
            direction.magnitude;

        if (distance > 0f)
        {
            /*
             * Check Wall theo đúng đoạn parabola.
             */
            if (Physics.Raycast(
                previousPosition,
                direction.normalized,
                out RaycastHit wallHit,
                distance + 0.05f,
                wallMask,
                QueryTriggerInteraction.Ignore))
            {
                HitWall(
                    wallHit,
                    direction.normalized
                );

                return;
            }
        }

        /*
         * Di chuyển theo parabola.
         */
        rb.MovePosition(nextPosition);

        /*
         * Xoay grenade khi đang bay.
         */
        rb.MoveRotation(
            rb.rotation *
            Quaternion.Euler(
                speedRotX * Time.fixedDeltaTime,
                speedRotY * Time.fixedDeltaTime,
                speedRotZ * Time.fixedDeltaTime
            )
        );

        if (progress >= 1f)
        {
            ArriveAtTarget();
        }
    }

    private Vector3 GetParabolaPosition(float t)
    {
        /*
         * Đường cơ bản:
         *
         * Start ---------------- Target
         *
         * Sau đó cộng độ cao ở trục Y.
         */
        Vector3 position =
            Vector3.Lerp(
                startPoint,
                targetPoint,
                t
            );

        /*
         * 4t(1-t):
         *
         * t = 0   -> 0
         * t = 0.5 -> 1
         * t = 1   -> 0
         *
         * Vì vậy parabola bắt đầu và kết thúc
         * đúng tại Start/Target.
         */
        float height =
            4f * t * (1f - t) * arcHeight;

        position.y += height;

        return position;
    }

    private void ArriveAtTarget()
    {
        initialized = false;

        rb.position = targetPoint;

        float totalFlightTime =
            Time.time - fireTime;

        Debug.Log(
            $"<color=yellow>[Grenade]</color> " +
            $"Đã tới Target. " +
            $"Thời gian bay: {totalFlightTime:F3}s"
        );

        Destroy(gameObject);
    }

    private void HitWall(
        RaycastHit wallHit,
        Vector3 direction)
    {
        initialized = false;

        float bounceSpeed =
            Mathf.Max(
                moveSpeed * wallBounceForce,
                minimumBounceSpeed
            );

        Vector3 reflectedDirection =
            Vector3.Reflect(
                direction,
                wallHit.normal
            );

        Vector3 bounceDirection =
            (
                reflectedDirection +
                Vector3.up * wallUpwardForce
            ).normalized;

        rb.position =
            wallHit.point +
            wallHit.normal * 0.02f;

        /*
         * Từ đây giao cho Physics.
         */
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.linearVelocity =
            bounceDirection * bounceSpeed;

        rb.angularVelocity = Vector3.zero;

        Debug.Log(
            $"<color=orange>[Grenade]</color> " +
            $"HIT WALL | " +
            $"Speed: {moveSpeed:F2} | " +
            $"Bounce: {bounceSpeed:F2}"
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (!initialized)
            return;

        /*
         * Vẽ parabola trong Scene.
         */
        Gizmos.color = Color.green;

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

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(
            targetPoint,
            0.1f
        );
    }
}
using System.Collections;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private GrenadeSkillData data;

    private Rigidbody rb;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float totalFlightTime;
    private float currentFlightTime;

    private bool exploded;
    private bool initialized;

    // =============================================================
    // INITIALIZE
    // =============================================================

    public void Initialize(
        GrenadeSkillData skillData,
        Vector3 start,
        Vector3 target)
    {
        data = skillData;

        startPosition = start;
        targetPosition = target;

        exploded = false;
        initialized = true;
        currentFlightTime = 0f;

        // =========================================================
        // GET RIGIDBODY
        // =========================================================

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError(
                "[GRENADE] Grenade prefab phải có Rigidbody!"
            );

            Destroy(gameObject);
            return;
        }

        // =========================================================
        // PHYSICS SETUP
        // =========================================================

        rb.isKinematic = true;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousSpeculative;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        // =========================================================
        // TÍNH THỜI GIAN BAY GỐC
        // =========================================================

        totalFlightTime =
            CalculateFlightTime(
                startPosition,
                targetPosition
            );

        totalFlightTime =
            Mathf.Max(totalFlightTime, 0.01f);

        // =========================================================
        // SPEED
        //
        // 1x  -> thời gian gốc
        // 2x  -> thời gian còn một nửa
        // 0.5 -> thời gian gấp đôi
        // =========================================================

        float speedMultiplier =
            Mathf.Max(data.speedGrenadeMove, 0.01f);

        totalFlightTime /= speedMultiplier;

        // =========================================================
        // EXPLODE DELAY
        // =========================================================

        StartCoroutine(
            ExplodeDelayRoutine()
        );

        Debug.Log(
            $"[GRENADE] Flight Time = {totalFlightTime:F2}s | " +
            $"Speed = {data.speedGrenadeMove:F2}x"
        );
    }

    // =============================================================
    // UPDATE
    // =============================================================

    private void Update()
    {
        if (!initialized || exploded)
            return;

        currentFlightTime += Time.deltaTime;

        float normalizedTime =
            Mathf.Clamp01(
                currentFlightTime / totalFlightTime
            );

        Vector3 position =
            CalculatePosition(
                startPosition,
                targetPosition,
                normalizedTime
            );

        rb.MovePosition(position);

        // =========================================================
        // ĐÃ TỚI TARGET
        // =========================================================

        if (normalizedTime >= 1f)
        {
            rb.MovePosition(targetPosition);

            Explode();
        }
    }

    // =============================================================
    // CALCULATE FLIGHT TIME
    // =============================================================

    private float CalculateFlightTime(
        Vector3 start,
        Vector3 target)
    {
        float gravity =
            Mathf.Abs(Physics.gravity.y);

        gravity =
            Mathf.Max(gravity, 0.01f);

        Vector3 displacement =
            target - start;

        Vector3 horizontal =
            new Vector3(
                displacement.x,
                0f,
                displacement.z
            );

        float horizontalDistance =
            horizontal.magnitude;

        // =========================================================
        // APEX
        // =========================================================

        float arcHeight =
            Mathf.Max(
                data.minArcHeight,
                horizontalDistance *
                data.arcHeightPerMeter
            );

        float apexHeight =
            start.y + arcHeight;

        // =========================================================
        // VELOCITY Y
        // =========================================================

        float verticalVelocity =
            Mathf.Sqrt(
                2f *
                gravity *
                Mathf.Max(
                    0.01f,
                    apexHeight - start.y
                )
            );

        // =========================================================
        // TIME UP
        // =========================================================

        float timeUp =
            verticalVelocity / gravity;

        // =========================================================
        // TIME DOWN
        // =========================================================

        float heightFromApexToTarget =
            apexHeight - target.y;

        heightFromApexToTarget =
            Mathf.Max(
                heightFromApexToTarget,
                0.01f
            );

        float timeDown =
            Mathf.Sqrt(
                2f *
                heightFromApexToTarget /
                gravity
            );

        return timeUp + timeDown;
    }

    // =============================================================
    // CALCULATE POSITION
    // =============================================================

    private Vector3 CalculatePosition(
        Vector3 start,
        Vector3 target,
        float t)
    {
        float gravity =
            Mathf.Abs(Physics.gravity.y);

        gravity =
            Mathf.Max(gravity, 0.01f);

        Vector3 displacement =
            target - start;

        Vector3 horizontal =
            new Vector3(
                displacement.x,
                0f,
                displacement.z
            );

        float horizontalDistance =
            horizontal.magnitude;

        // =========================================================
        // SAME ARC AS ORIGINAL SYSTEM
        // =========================================================

        float arcHeight =
            Mathf.Max(
                data.minArcHeight,
                horizontalDistance *
                data.arcHeightPerMeter
            );

        float apexHeight =
            start.y + arcHeight;

        // =========================================================
        // ORIGINAL VERTICAL VELOCITY
        // =========================================================

        float verticalVelocity =
            Mathf.Sqrt(
                2f *
                gravity *
                Mathf.Max(
                    0.01f,
                    apexHeight - start.y
                )
            );

        // =========================================================
        // ORIGINAL FLIGHT TIME
        //
        // QUAN TRỌNG:
        // Ta tính lại flight time nguyên bản ở đây.
        // speedGrenadeMove chỉ thay đổi tốc độ chạy
        // qua normalized time.
        // =========================================================

        float timeUp =
            verticalVelocity / gravity;

        float heightFromApexToTarget =
            Mathf.Max(
                apexHeight - target.y,
                0.01f
            );

        float timeDown =
            Mathf.Sqrt(
                2f *
                heightFromApexToTarget /
                gravity
            );

        float originalFlightTime =
            timeUp + timeDown;

        originalFlightTime =
            Mathf.Max(
                originalFlightTime,
                0.01f
            );

        // =========================================================
        // TÍNH VỊ TRÍ NGANG
        // =========================================================

        Vector3 horizontalPosition =
            Vector3.Lerp(
                start,
                target,
                t
            );

        // Chỉ giữ X/Z
        horizontalPosition.y = start.y;

        // =========================================================
        // TÍNH Y THEO PARABOLA GỐC
        // =========================================================

        float time =
            t * originalFlightTime;

        float y =
            start.y +
            verticalVelocity * time -
            0.5f * gravity * time * time;

        horizontalPosition.y = y;

        return horizontalPosition;
    }

    // =============================================================
    // EXPLODE DELAY
    // =============================================================

    private IEnumerator ExplodeDelayRoutine()
    {
        yield return new WaitForSeconds(
            data.explodeDelay
        );

        if (exploded)
            yield break;

        Debug.Log(
            "[GRENADE] Explode bởi explodeDelay."
        );

        Explode();
    }

    // =============================================================
    // COLLISION
    // =============================================================

    private void OnCollisionEnter(Collision collision)
    {
        if (!initialized)
            return;

        if (exploded)
            return;

        int layerBit =
            1 << collision.gameObject.layer;

        bool isHitMask =
            (data.hitMask.value & layerBit) != 0;

        if (!isHitMask)
        {
            Debug.Log(
                $"[GRENADE] Va vào " +
                $"{collision.gameObject.name} " +
                $"nhưng không thuộc hitMask."
            );

            return;
        }

        Debug.Log(
            $"[GRENADE] Hit hitMask: " +
            $"{collision.gameObject.name}"
        );

        Explode();
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

        Debug.Log(
            $"[GRENADE] EXPLODE tại " +
            $"{explosionPosition}"
        );

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
}
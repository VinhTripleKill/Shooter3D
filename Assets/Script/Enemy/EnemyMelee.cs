using UnityEngine;

public class EnemyMelee : EnemyAttack
{
    [Header("Melee Settings")]
    [SerializeField, Range(0f, 360f)]
    private float attackAngle = 90f;

    public float AttackAngle => attackAngle;

    // =========================================================
    // ATTACK
    // =========================================================

    public override void UpdateAttack()
    {
        if (player == null)
            return;

        FacePlayer();

        if (!CanAttack)
            return;

        StartAttack();
    }

    // =========================================================
    // FACE PLAYER
    // =========================================================

    private void FacePlayer()
    {
        Vector3 lookPosition =
            new Vector3(
                player.position.x,
                transform.position.y,
                player.position.z
            );

        Vector3 direction =
            lookPosition -
            transform.position;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        transform.LookAt(lookPosition);
    }

    // =========================================================
    // ANIMATION EVENT
    // =========================================================

    public void PerformMeleeDamage()
    {
        if (player == null)
            return;

        Vector3 origin =
            transform.position +
            Vector3.up * 1f;

        Vector3 toPlayer =
            player.position -
            origin;

        Vector3 flatToPlayer =
            Vector3.ProjectOnPlane(
                toPlayer,
                Vector3.up
            );

        Vector3 flatForward =
            Vector3.ProjectOnPlane(
                transform.forward,
                Vector3.up
            );

        float distance =
            flatToPlayer.magnitude;

        float angle =
            Vector3.Angle(
                flatForward,
                flatToPlayer
            );

        // =====================================================
        // RANGE
        // =====================================================

        if (distance > atkRange)
            return;

        // =====================================================
        // CONE
        // =====================================================

        if (angle > attackAngle * 0.5f)
            return;

        // =====================================================
        // DAMAGE
        // =====================================================

        IDamageable damageable =
            player.GetComponentInChildren<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(
                atkDamage
            );
        }
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    public override void Initialize(
        EnemyBehaviour behaviour,
        EnemyAnim enemyAnim)
    {
        base.Initialize(
            behaviour,
            enemyAnim
        );

        if (this.enemyAnim != null)
        {
            this.enemyAnim.OnAttackHit -= PerformMeleeDamage;

            this.enemyAnim.OnAttackHit +=
                PerformMeleeDamage;
        }
    }

    // =========================================================
    // DISABLE
    // =========================================================

    protected override void OnDisable()
    {
        if (enemyAnim != null)
        {
            enemyAnim.OnAttackHit -=
                PerformMeleeDamage;
        }

        base.OnDisable();
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color =
            new Color(
                1f,
                0.2f,
                0f,
                0.35f
            );

        Vector3 origin =
            transform.position +
            Vector3.up * 1f;

        DrawConeGizmo(
            origin,
            transform.forward,
            atkRange,
            attackAngle
        );
    }

    private void DrawConeGizmo(
        Vector3 origin,
        Vector3 direction,
        float range,
        float angle)
    {
        int segments = 12;

        float halfAngle =
            angle * 0.5f * Mathf.Deg2Rad;

        // =====================================================
        // RAYS
        // =====================================================

        for (int i = 0;
             i <= segments;
             i++)
        {
            float t =
                (float)i / segments;

            float currentAngle =
                Mathf.Lerp(
                    -halfAngle,
                    halfAngle,
                    t
                );

            Quaternion rotation =
                Quaternion.Euler(
                    0f,
                    currentAngle * Mathf.Rad2Deg,
                    0f
                );

            Vector3 dir =
                rotation * direction;

            Gizmos.DrawRay(
                origin,
                dir * range
            );
        }

        // =====================================================
        // ARC
        // =====================================================

        for (int i = 0;
             i < segments;
             i++)
        {
            float a1 =
                Mathf.Lerp(
                    -halfAngle,
                    halfAngle,
                    (float)i / segments
                );

            float a2 =
                Mathf.Lerp(
                    -halfAngle,
                    halfAngle,
                    (float)(i + 1) / segments
                );

            Quaternion r1 =
                Quaternion.Euler(
                    0f,
                    a1 * Mathf.Rad2Deg,
                    0f
                );

            Quaternion r2 =
                Quaternion.Euler(
                    0f,
                    a2 * Mathf.Rad2Deg,
                    0f
                );

            Vector3 p1 =
                origin +
                r1 * direction * range;

            Vector3 p2 =
                origin +
                r2 * direction * range;

            Gizmos.DrawLine(
                p1,
                p2
            );
        }
    }
}
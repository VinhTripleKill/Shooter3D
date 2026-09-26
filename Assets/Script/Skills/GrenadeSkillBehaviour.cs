using UnityEngine;

public class GrenadeSkillBehaviour : SkillBehaviour
{
    [SerializeField]
    private GrenadeSkillData skillData;

    public override bool Execute(PlayerSkill playerSkill)
    {
        if (skillData == null)
            return false;

        if (skillData.grenadePrefab == null)
        {
            Debug.LogWarning(
                "GrenadeSkill: grenadePrefab chưa được gán!"
            );

            return false;
        }

        Transform firePoint =
            playerSkill.GetFirePoint();

        if (firePoint == null)
        {
            Debug.LogWarning(
                "GrenadeSkill: PlayerSkill chưa có FirePoint!"
            );

            return false;
        }

        bool autoAim =
            playerSkill.IsAutoAim();

        Vector3 targetPosition;

        // =========================================================
        // AUTO AIM
        // =========================================================

        if (autoAim)
        {
            Transform target =
                CombatTargetFinder.GetNearestTarget(
                    firePoint.position,
                    skillData.rangeRadius
                );

            if (target == null)
            {
                return false;
            }

            targetPosition =
                target.position;
        }

        // =========================================================
        // MANUAL AIM
        // =========================================================

        else
        {
            if (!playerSkill.TryGetSkillTargetPosition(
                    out Vector3 selectedTarget))
            {
                return false;
            }

            targetPosition =
                selectedTarget;
        }

        // =========================================================
        // TARGET -> GROUND
        // =========================================================

        if (!TryFindGroundPosition(
                targetPosition,
                out Vector3 groundPosition))
        {
            return false;
        }

        // =========================================================
        // PLAYER FACE TARGET
        // =========================================================

        Vector3 lookDirection =
            groundPosition -
            playerSkill.transform.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            playerSkill.transform.forward =
                lookDirection.normalized;
        }

        // =========================================================
        // CREATE GRENADE
        // =========================================================

        GameObject grenade =
            Instantiate(
                skillData.grenadePrefab,
                firePoint.position,
                firePoint.rotation
            );

        GrenadeProjectile projectile =
            grenade.GetComponent<GrenadeProjectile>();

        if (projectile == null)
        {
            Destroy(grenade);
            return false;
        }

        // =========================================================
        // INITIALIZE
        // =========================================================

        projectile.Initialize(
            skillData,
            firePoint.position,
            groundPosition
        );

        return true;
    }

    // =============================================================
    // TARGET -> GROUND RAYCAST
    // =============================================================

    private bool TryFindGroundPosition(
        Vector3 targetPosition,
        out Vector3 groundPosition)
    {
        const float rayHeight = 50f;
        const float rayDistance = 100f;

        Vector3 rayStart =
            targetPosition +
            Vector3.up * rayHeight;

        if (Physics.Raycast(
                rayStart,
                Vector3.down,
                out RaycastHit hit,
                rayDistance,
                skillData.groundMask,
                QueryTriggerInteraction.Ignore))
        {
            groundPosition =
                hit.point;

            return true;
        }

        groundPosition =
            targetPosition;

        return false;
    }

    public override SkillData GetSkillData()
    {
        return skillData;
    }
}
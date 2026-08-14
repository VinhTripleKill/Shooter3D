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

    // =========================================================
    // AIM MODE
    // =========================================================

    bool autoAim =
        playerSkill.IsAutoAim();

    Vector3 targetPosition;

    // =========================================================
    // AUTO AIM
    //
    // Chỉ được phép auto nếu có enemy trong range.
    //
    // KHÔNG CÓ ENEMY:
    // -> Không ném
    // -> Không fallback ra phía trước
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
            Debug.Log(
                "No enemy in skill range"
            );

            return false;
        }

        targetPosition =
            target.position;

        Debug.Log(
            $"[GRENADE] AUTO AIM -> " +
            $"Target: {target.name} | " +
            $"Position: {targetPosition}"
        );
    }

    // =========================================================
    // MANUAL AIM
    //
    // Bắt buộc lấy vị trí grenadeIndicator.
    //
    // TUYỆT ĐỐI KHÔNG:
    //
    // direction * rangeRadius
    //
    // Vì khoảng cách thực tế phụ thuộc vào joystick.
    // =========================================================

    else
    {
        if (!playerSkill.TryGetSkillTargetPosition(
                out Vector3 selectedTarget))
        {
            Debug.LogWarning(
                "[GRENADE] Manual aim nhưng " +
                "không có target position!"
            );

            return false;
        }

        targetPosition =
            selectedTarget;

        Debug.Log(
            $"[GRENADE] MANUAL AIM -> " +
            $"Selected Target: {targetPosition}"
        );
    }

    // =========================================================
    // GROUND
    // =========================================================

    if (!TryFindGroundPosition(
            targetPosition,
            out Vector3 groundPosition))
    {
        Debug.LogWarning(
            "[GRENADE] Không tìm thấy Ground tại target position!"
        );

        return false;
    }

    // =========================================================
    // PLAYER QUAY VỀ HƯỚNG GRENADE
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


    GameObject grenade =
        Instantiate(
            skillData.grenadePrefab,
            firePoint.position,
            Quaternion.identity
        );


    GrenadeProjectile projectile =
        grenade.GetComponent<GrenadeProjectile>();

    if (projectile == null)
    {
        Destroy(grenade);

        return false;
    }


    projectile.Initialize(
        skillData,
        firePoint.position,
        groundPosition
    );

    return true;
}

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
                skillData.hitMask,
                QueryTriggerInteraction.Ignore))
        {
            groundPosition = hit.point;

            return true;
        }

        groundPosition = targetPosition;

        return false;
    }

    public override SkillData GetSkillData()
    {
        return skillData;
    }
}
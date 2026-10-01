using UnityEngine;

public class DashSkillBehaviour : SkillBehaviour
{
    [SerializeField]
    private DashSkillData skillData;

    public override bool Execute(PlayerSkill playerSkill)
    {
        if (skillData == null)
            return false;

        PlayerController player =
            playerSkill.GetComponent<PlayerController>();

        if (player == null)
            return false;

        Vector3 direction;

        // =========================================================
        // AUTO AIM
        // =========================================================

        if (playerSkill.IsAutoAim())
        {
            // Nếu player đang giữ Move
            // => dash theo hướng đang di chuyển
            if (player.IsMoving())
            {
                direction =
                    player.GetMoveDirection();

                Debug.Log(
                    $"DASH AUTO + MOVE | Direction: {direction}"
                );
            }
            else
            {
                direction =
                    player.transform.forward;

                Debug.Log(
                    $"DASH AUTO + NO MOVE | Forward: {direction}"
                );
            }
        }

        // =========================================================
        // MANUAL AIM
        // =========================================================

        else
        {
            direction =
                playerSkill.GetAimDirection();

            if (direction.sqrMagnitude < 0.001f)
            {
                direction =
                    player.transform.forward;

                Debug.Log(
                    "DASH MANUAL | No Aim Direction -> Forward"
                );
            }
            else
            {
                Debug.Log(
                    $"DASH MANUAL | Aim Direction: {direction}"
                );
            }
        }

        // =========================================================
        // CLEAN DIRECTION
        // =========================================================

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return false;

        direction.Normalize();

        // =========================================================
        // START DASH
        // =========================================================

        bool success =
            player.StartDash(
                direction,
                skillData.dashDistance,
                skillData.timeDash,
                skillData.timeNextDash,
                skillData.obstacleMask
            );

        // =========================================================
        // DASH SFX
        // =========================================================

        if (success)
        {
            PlayDashSFX(
                player.transform.position
            );
        }

        return success;
    }

    // =============================================================
    // PLAY DASH SFX
    // =============================================================

    private void PlayDashSFX(Vector3 position)
    {
        if (skillData.skillSFXPrefab == null)
        {
            Debug.LogWarning(
                "DashSkillBehaviour | " +
                "skillSFXPrefab chưa được gán!"
            );

            return;
        }

        GameObject dashSFX =
            Instantiate(
                skillData.skillSFXPrefab,
                position,
                Quaternion.identity
            );

        DashSFX sfx =
            dashSFX.GetComponent<DashSFX>();

        if (sfx != null)
        {
            sfx.Initialize(
                skillData.skillSFX
            );
        }
        else
        {
            Debug.LogWarning(
                "DashSkillBehaviour | " +
                "skillSFXPrefab không có component DashSFX!"
            );

            Destroy(dashSFX);
        }
    }

    // =============================================================
    // SKILL DATA
    // =============================================================

    public override SkillData GetSkillData()
    {
        return skillData;
    }
}
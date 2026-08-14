




using UnityEngine;

public class DashSkillBehaviour : SkillBehaviour
{
    [SerializeField] private DashSkillData skillData;

    public override bool Execute(PlayerSkill playerSkill)
    {
        if (skillData == null)return false;

        PlayerController player = playerSkill.GetComponent<PlayerController>();

        if (player == null)return false;

        Vector3 direction;
        if (playerSkill.IsAutoAim())
        {
            // Nếu player đang giữ Move
            // => dash theo hướng đang di chuyển
            if (player.IsMoving())
            {
                direction = player.GetMoveDirection();

                Debug.Log($"DASH AUTO + MOVE | Direction: {direction}");
            }
            else
            {
                direction = player.transform.forward;

                Debug.Log($"DASH AUTO + NO MOVE | Forward: {direction}" );
            }
        }
        else
        {
            direction = playerSkill.GetAimDirection();

            if (direction.sqrMagnitude < 0.001f)
            {
                direction = player.transform.forward;

                Debug.Log("DASH MANUAL | No Aim Direction -> Forward");
            }
            else
            {
                Debug.Log($"DASH MANUAL | Aim Direction: {direction}");
            }
        }


        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return false;

        direction.Normalize();


        return player.StartDash(
            direction,
            skillData.dashDistance ,
            skillData.timeDash,
            skillData.timeNextDash,
            skillData.obstacleMask
        );
    }

    public override SkillData GetSkillData()
    {
        return skillData;
    }
}





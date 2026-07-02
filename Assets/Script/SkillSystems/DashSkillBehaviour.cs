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

    if (!player.IsMoving())
        return false;

    return player.StartDash(
        skillData.timeDash,
        skillData.speedDash,
        skillData.timeNextDash);
        
}

    public override SkillData GetSkillData()
    {
        return skillData;
    }
}
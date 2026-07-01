using UnityEngine;

public abstract class SkillBehaviour : MonoBehaviour
{
    public abstract void Execute(PlayerSkill playerSkill);

    public abstract SkillData GetSkillData();
}
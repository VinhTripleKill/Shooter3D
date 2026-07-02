using UnityEngine;

public abstract class SkillBehaviour : MonoBehaviour
{
    public abstract bool Execute(PlayerSkill playerSkill);

    public abstract SkillData GetSkillData();
}
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Dash Skill")]
public class DashSkillData : SkillData
{
    public float timeDash = 0.2f;

    public float speedDash = 20f;

    public float timeNextDash = 0.05f;
}
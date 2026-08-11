


using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Dash Skill")]
public class DashSkillData : SkillData
{
    [Header("Dash")]
    public float dashDistance = 3f;
    public float timeDash = 0.2f;
    public float timeNextDash = 0.05f;

    [Header("Collision")]
    public LayerMask obstacleMask;
}




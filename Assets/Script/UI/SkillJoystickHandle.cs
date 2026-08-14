using UnityEngine;

public class SkillJoystickHandle : MonoBehaviour
{
    public enum SkillType { None, Dash, Grenade, Other }

    [SerializeField] private SkillType currentSkillType;
    private SkillIndicatorUI skillIndicator;

    public SkillType CurrentSkillType => currentSkillType;

    public void SetSkill(SkillData skillData) => currentSkillType = GetSkillType(skillData);
    public void SetIndicator(SkillIndicatorUI indicator) => skillIndicator = indicator;

    public void Begin() => skillIndicator?.ShowAutoPreview();

    public void UpdateDirection(Vector2 direction, float distance)
    {
        if (direction.sqrMagnitude < 0.001f) return;
        skillIndicator?.UpdateDirection(direction, distance);
    }

    public bool RequiresDrag() => currentSkillType == SkillType.Grenade;

    public bool TryGetGrenadeTarget(out Vector3 target)
    {
        target = Vector3.zero;
        return currentSkillType == SkillType.Grenade &&
               skillIndicator != null &&
               skillIndicator.TryGetGrenadeTarget(out target);
    }

    private SkillType GetSkillType(SkillData data)
    {
        if (data == null) return SkillType.None;
        if (data is DashSkillData) return SkillType.Dash;
        if (data is GrenadeSkillData) return SkillType.Grenade;
        return SkillType.Other;
    }
}
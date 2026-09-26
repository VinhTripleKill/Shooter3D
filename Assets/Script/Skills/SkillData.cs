using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [Header("Info")]
    public string skillName;
    public Sprite icon;

    [Header("Stat")]
    public float cooldown;
    public int maxStack;
    public float rangeRadius;
    public float skillCostMana; // Mana cần để sử dụng skill
    
    [Header("Behaviour")]
    public SkillBehaviour skillBehaviourPrefab;
}
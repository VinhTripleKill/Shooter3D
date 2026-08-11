
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
    
    [Header("Behaviour")]
    public SkillBehaviour skillBehaviourPrefab;   // ← Thêm cái này
}






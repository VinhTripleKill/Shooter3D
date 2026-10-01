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
    public float skillCostMana;

    [Header("Behaviour")]
    public SkillBehaviour skillBehaviourPrefab;

    // =====================================================
    // SKILL AUDIO / SFX
    // =====================================================

    [Header("Skill SFX")]
    [Tooltip("Prefab chứa AudioSource để phát âm thanh của skill.")]
    public GameObject skillSFXPrefab;

    [Tooltip("Âm thanh được phát bởi skillSFXPrefab.")]
    public AudioClip skillSFX;
}
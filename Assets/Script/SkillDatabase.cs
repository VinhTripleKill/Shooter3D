using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SkillDatabase", menuName = "Dynher/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    [SerializeField] private List<SkillData> skills = new();

    public IReadOnlyList<SkillData> Skills => skills;
}
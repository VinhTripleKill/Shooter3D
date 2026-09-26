using UnityEngine;

[CreateAssetMenu( fileName = "CharacterData", menuName = "Dynher/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Info")]
    public string characterName;
    public Sprite icon;
    [TextArea]
    public string descriptionCharacter;

    public GameObject modelPrefab;

    [Header("Stats")]
    public CharacterStats CharacterStats;

}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase",menuName = "Dynher/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private List<CharacterData> characters = new();

    public IReadOnlyList<CharacterData> Characters => characters;
}
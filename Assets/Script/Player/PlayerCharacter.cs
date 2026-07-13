using UnityEngine;
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;

    public CharacterData Data => characterData;

    public void SetCharacter(CharacterData data)
    {
        characterData = data;
    }
}
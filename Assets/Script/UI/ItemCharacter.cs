using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCharacterUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Button button;

    [Header("State")]
    [SerializeField] private GameObject notSelect;

    private CharacterData data;

    public CharacterData Data => data;

    public void Initialize( CharacterData characterData, System.Action<ItemCharacterUI> onClick)
    {
        data = characterData;

        icon.sprite = data.icon;
        characterName.text = data.characterName;

        SetSelected(false);

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            onClick?.Invoke(this);
        });
    }

    public void SetSelected(bool selected)
    {
        if (notSelect != null)
            notSelect.SetActive(!selected);
    }
}
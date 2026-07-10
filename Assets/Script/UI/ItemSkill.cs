using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkill : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text skillName;
    [SerializeField] private Button button;

    [Header("State")]
    [SerializeField] private GameObject notSelect;

    private SkillData data;

    public SkillData Data => data;

    public void Initialize( SkillData skillData, System.Action<ItemSkill> onClick)
    {
        data = skillData;

        icon.sprite = data.icon;
        skillName.text = data.skillName;

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
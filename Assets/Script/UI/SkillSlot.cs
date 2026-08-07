using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image skillIcon;
    //[SerializeField] private Button skillButton;
    [SerializeField] private Image skillCD;
    [SerializeField] private TextMeshProUGUI skillCount;

    public void Initialize(Sprite icon, int currentStack, int maxStack)
    {
        skillIcon.sprite = icon;

        UpdateSkillStack(currentStack);

        skillCD.gameObject.SetActive(currentStack < maxStack);

        skillCD.fillAmount = 0f;
    }

    public void UpdateSkillStack(int currentStack)
    {
        skillCount.text = currentStack.ToString();
    }

    public void UpdateSkillCooldown(float timer, float cooldown)
    {
        if (cooldown <= 0f)
        {
            skillCD.fillAmount = 0f;
            return;
        }

        skillCD.fillAmount = 1f - (timer / cooldown);
    }

    public void ShowSkillCooldown(bool show)
    {
        skillCD.gameObject.SetActive(show);

        if (!show)
            skillCD.fillAmount = 0f;
    }

    //public Button GetSkillButton()=> skillButton;
    
}
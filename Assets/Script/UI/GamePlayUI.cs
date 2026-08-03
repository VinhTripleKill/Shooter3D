using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GamePlayUI : MonoBehaviour
{
    [Header("Sprint")]
    [SerializeField] private Image sprintBar;
    [SerializeField] private Button sprintButton;
    [SerializeField] private Image sprintLock;
    [Header("Skill")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private Button skillButton;
    [SerializeField] private Image skillCD;
    [SerializeField] private TextMeshProUGUI skillCount;
    [Header("Mana")]
    [SerializeField] private Image manaBar;
    [SerializeField] private TextMeshProUGUI manaText;
    [Header("HpBar")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;
    [Header("Level")] 
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image levelBarProgress;
    
    [Header("Coin")]
    [SerializeField] private TextMeshProUGUI coinText;
    [Header("Ammo")]
    [SerializeField] private Image statusAmmo;
    [SerializeField] private Image ammoI;
    [SerializeField] private Button reloadButton;
    [SerializeField] private float reloadRotateSpeed = 360f;
    [Header("Pick Up")]
    [SerializeField] private Button pickUp;
    private bool isReloading;

    private void Awake()
    {
        sprintLock.gameObject.SetActive(false);
        pickUp.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isReloading) return;
      
        ammoI.rectTransform.Rotate( 0f,0f,-reloadRotateSpeed * Time.deltaTime);
    }
    public void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;

        hpText.text = $"{Mathf.CeilToInt(currentHp)}/{Mathf.CeilToInt(maxHp)}";
    }

    public void UpdateManaBar(float currentMana, float maxMana)
    {
        manaBar.fillAmount = currentMana / maxMana;
        manaText.text = $"{Mathf.CeilToInt(currentMana)}/{Mathf.CeilToInt(maxMana)}";
    }
    public void ShowPickUp()
    {
        pickUp.gameObject.SetActive(true);
    }

    public void HidePickUp()
    {
        pickUp.gameObject.SetActive(false);
    }

    public Button GetPickUpButton()
    {
        return pickUp;
    }
    public void StartReloadVisual()
    {
        isReloading = true;
    }

    public void StopReloadVisual()
    {
        isReloading = false;

        ammoI.rectTransform.localEulerAngles = Vector3.zero;
    }

    public void UpdateSprintBar( float currentSprint,float maxSprint)
    {
        sprintBar.fillAmount =currentSprint / maxSprint;
    }

    public void UpdateAmmoBar(int currentAmmo, int maxAmmo)
    {
        if (maxAmmo <= 0)
        {
            statusAmmo.fillAmount = 0f;
            return;
        }

        statusAmmo.fillAmount = (float)currentAmmo / maxAmmo;
    }

    public void ResetAmmoBar()
    {
        statusAmmo.fillAmount = 1f;
    }
    public Button GetReloadButton()
    {
        return reloadButton;
    }

    public void ShowSprintLock()
    {
        sprintLock.gameObject.SetActive(true);
    }

    public void HideSprintLock()
    {
        sprintLock.gameObject.SetActive(false);
    }

    public Button GetSprintButton()
    {
        return sprintButton;
    }

    public void InitializeSkillUI(Sprite icon, int currentStack, int maxStack)
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
    public Button GetSkillButton()
    {
        return skillButton;
    }
    public void UpdateLevelText(PlayerProgress progress)
    {
        levelText.text = progress.CurrentLevel.ToString();
    }
    public void UpdateLevelBar(PlayerProgress progress)
    {
        if (progress.CurrentLevel >= progress.MaxLevel)
        {
            levelBarProgress.fillAmount = 1f;
            return;
        }
        if (progress.CurrentLevel <= 0)
        {
            levelBarProgress.fillAmount = 0f;
            return;
        }
    
        levelBarProgress.fillAmount = (float)progress.CurrentExp / progress.RequiredExp;
    }
}
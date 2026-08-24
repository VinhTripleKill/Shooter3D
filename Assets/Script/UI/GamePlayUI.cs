using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class GamePlayUI : MonoBehaviour
{
    [Header("Sprint")]
    [SerializeField] private Image sprintBar;
    [SerializeField] private Button sprintButton;
    [SerializeField] private Image sprintLock;
    [SerializeField] private SkillSlot skillSlot;
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
    private PlayerProgress playerProgress;
    [Header("Ammo")]
    [SerializeField] private Image statusAmmo;
    [SerializeField] private Image ammoI;
    [Header("Reload")]
    [SerializeField] private Button reloadButton;
    [SerializeField] private float reloadRotateSpeed = 360f;
    [SerializeField] private Image reloadLock;
    [SerializeField] private TextMeshProUGUI reloadText;
    private Coroutine reloadCoroutine;
    [Header("Pick Up")]
    [SerializeField] private Button pickUp;
    private bool isReloading;

    private void Awake()
    {
        sprintLock.gameObject.SetActive(false);
        reloadLock.gameObject.SetActive(false);
        pickUp.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isReloading) return;
      
        ammoI.rectTransform.Rotate( 0f,0f,-reloadRotateSpeed * Time.deltaTime);
    }
    public void UpdateHpBar(float currentHp, float maxHp)
{
    hpBar.fillAmount = maxHp > 0f
        ? currentHp / maxHp
        : 0f;

    hpText.text =
        $"{FormatStatValue(currentHp)}/{FormatStatValue(maxHp)}";
}

public void UpdateManaBar(float currentMana, float maxMana)
{
    manaBar.fillAmount = maxMana > 0f
        ? currentMana / maxMana
        : 0f;

    manaText.text =
        $"{FormatStatValue(currentMana)}/{FormatStatValue(maxMana)}";
}

    private string FormatStatValue(float value)
{
    // Nếu là số nguyên thì không hiển thị phần thập phân
    if (Mathf.Approximately(value, Mathf.Round(value)))
    {
        return Mathf.RoundToInt(value).ToString();
    }

    // Nếu có phần thập phân thì hiển thị tối đa 2 chữ số
    return value.ToString("0.##");
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
    public void StartReloadVisual(float reloadTime)
    {
    isReloading = true;

    reloadLock.gameObject.SetActive(true);

    if (reloadCoroutine != null)
        StopCoroutine(reloadCoroutine);

    reloadCoroutine = StartCoroutine(ReloadCountdown(reloadTime));
    }

    public void StopReloadVisual()
    {
        isReloading = false;
    
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    
        reloadLock.gameObject.SetActive(false);
    
        reloadText.text = "";
    
        ammoI.rectTransform.localEulerAngles = Vector3.zero;
    }
    private IEnumerator ReloadCountdown(float duration)
    {
        float timer = duration;
    
        while (timer > 0f)
        {
            if (timer >= 1f)
            {
                reloadText.text = Mathf.CeilToInt(timer).ToString();
            }
            else
            {
                float value = Mathf.Max(0f, timer);
                reloadText.text = value.ToString("F1");
            }
    
            timer -= Time.deltaTime;
            yield return null;
        }
    
        reloadText.text = "0";
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
    skillSlot.Initialize(icon, currentStack, maxStack);
}

public void UpdateSkillStack(int currentStack)
{
    skillSlot.UpdateSkillStack(currentStack);
}

public void UpdateSkillCooldown(float timer, float cooldown)
{
    skillSlot.UpdateSkillCooldown(timer, cooldown);
}

public void ShowSkillCooldown(bool show)
{
    skillSlot.ShowSkillCooldown(show);
}

// public Button GetSkillButton()
// {
//     return skillSlot.GetSkillButton();
// }   
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
public void SetPlayerProgress(PlayerProgress progress)
{
    // Hủy đăng ký Player cũ
    if (playerProgress != null)
    {
        playerProgress.OnCoinChanged -= UpdateCoinUI;
    }

    playerProgress = progress;

    if (playerProgress == null)
    {
        coinText.text = "0";
        return;
    }

    // Đăng ký Player mới
    playerProgress.OnCoinChanged += UpdateCoinUI;

    // Cập nhật ngay giá trị hiện tại
    UpdateCoinUI(playerProgress);
}

private void UpdateCoinUI(PlayerProgress progress)
{
    if (progress == null)
        return;

    coinText.text = progress.CurrentCoins.ToString();
}
}

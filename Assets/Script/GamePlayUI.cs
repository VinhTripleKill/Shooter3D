using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GamePlayUI : MonoBehaviour
{
    [Header("Sprint")]
    [SerializeField] private Image sprintBar;
    [SerializeField] private Button sprintButton;
    [SerializeField] private Image sprintLock;
    [Header("Ultimate")]
    [SerializeField] private Image ultimateBar;
    [SerializeField] private Button ultimateButton;
    [Header("Skill")]
    [SerializeField] private Button skillButton;
    [SerializeField] private Image skillCD;
    [SerializeField] private TextMeshProUGUI skillCount;
    [Header("Mana")]
    [SerializeField] private Image manaBar;
    [Header("HpBar")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;
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
    public Button GetUltimateButton()
    {
        return ultimateButton;
    }
    public void UpdateManaBar(float currentMana, float maxMana)
    {
        manaBar.fillAmount = currentMana / maxMana;
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
}
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    [Header("Sprint")]
    [SerializeField] private Image sprintBar;
    [SerializeField] private Button sprintButton;
    [SerializeField] private Image sprintLock;

    [Header("Ammo")]
    [SerializeField] private Image statusAmmo;
    [SerializeField] private Image ammoI;
    [SerializeField] private Button reloadButton;
    [SerializeField] private float reloadRotateSpeed = 360f;

    private bool isReloading;

    private void Awake()
    {
        sprintLock.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isReloading)
            return;

        ammoI.rectTransform.Rotate(
            0f,
            0f,
            -reloadRotateSpeed * Time.deltaTime);
    }

    public void StartReloadVisual()
    {
        isReloading = true;
    }

    public void StopReloadVisual()
    {
        isReloading = false;

        ammoI.rectTransform.localEulerAngles =
            Vector3.zero;
    }

    public void UpdateSprintBar(
        float currentSprint,
        float maxSprint)
    {
        sprintBar.fillAmount =
            currentSprint / maxSprint;
    }

    public void UpdateAmmoBar(
        int currentAmmo,
        int maxAmmo)
    {
        if (maxAmmo <= 0)
        {
            statusAmmo.fillAmount = 0f;
            return;
        }

        statusAmmo.fillAmount =
            (float)currentAmmo / maxAmmo;
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
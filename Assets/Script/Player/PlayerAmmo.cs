using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    // =====================================================
    // REFERENCES
    // =====================================================

    private PlayerWeapon playerWeapon;

    [Header("UI")]
    [SerializeField] private GamePlayUI gameplayUI;

    // =====================================================
    // STATE
    // =====================================================

    private int currentAmmo;

    public int CurrentAmmo => currentAmmo;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    // =====================================================
    // SETUP
    // =====================================================

    public void SetGameplayUI(GamePlayUI ui)
    {
        gameplayUI = ui;
    }

    // =====================================================
    // INITIALIZE AMMO
    // =====================================================

    public void InitializeAmmo(
        GunData gunData,
        int ammo = -1)
    {
        if (gunData == null)
        {
            currentAmmo = 0;
            UpdateAmmoUI();
            return;
        }

        currentAmmo =
            ammo < 0
                ? gunData.maxCountShot
                : Mathf.Clamp(
                    ammo,
                    0,
                    gunData.maxCountShot
                );

        UpdateRuntimeAmmo();
        UpdateAmmoUI();
    }

    // =====================================================
    // HAS AMMO
    // =====================================================

    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    // =====================================================
    // CONSUME AMMO
    // =====================================================

    public bool ConsumeAmmo()
    {
        if (playerWeapon == null)
            return false;

        if (!playerWeapon.HasGun)
            return false;

        if (currentAmmo <= 0)
            return false;

        currentAmmo--;

        UpdateRuntimeAmmo();
        UpdateAmmoUI();

        return true;
    }

    // =====================================================
    // SET FULL AMMO
    // =====================================================

    public void SetFullAmmo()
    {
        if (playerWeapon == null)
            return;

        GunData gunData =
            playerWeapon.CurrentGunData;

        if (gunData == null)
            return;

        currentAmmo =
            gunData.maxCountShot;

        UpdateRuntimeAmmo();
        UpdateAmmoUI();
    }

    // =====================================================
    // SET AMMO
    // =====================================================

    public void SetAmmo(int ammo)
    {
        if (playerWeapon == null)
            return;

        GunData gunData =
            playerWeapon.CurrentGunData;

        if (gunData == null)
        {
            currentAmmo = 0;
            return;
        }

        currentAmmo =
            Mathf.Clamp(
                ammo,
                0,
                gunData.maxCountShot
            );

        UpdateRuntimeAmmo();
        UpdateAmmoUI();
    }

    // =====================================================
    // RUNTIME DATA
    // =====================================================

    private void UpdateRuntimeAmmo()
    {
        if (playerWeapon == null)
            return;

        GameObject currentGun =
            playerWeapon.CurrentGun;

        if (currentGun == null)
            return;

        GunRuntimeData runtime =
            currentGun.GetComponent<GunRuntimeData>();

        if (runtime != null)
        {
            runtime.currentAmmo =
                currentAmmo;
        }
    }

    // =====================================================
    // UI
    // =====================================================

    public void UpdateAmmoUI()
    {
        if (playerWeapon == null)
            return;

        GunData gunData =
            playerWeapon.CurrentGunData;

        if (gunData == null)
            return;

        gameplayUI?.UpdateAmmoBar(
            currentAmmo,
            gunData.maxCountShot
        );
    }

    // =====================================================
    // RESET
    // =====================================================

    public void ResetAmmo()
    {
        currentAmmo = 0;

        gameplayUI?.ResetAmmoBar();
    }
}
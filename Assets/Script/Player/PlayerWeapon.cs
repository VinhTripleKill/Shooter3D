using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Holder")]
    [SerializeField] private Transform gunHolder;

    [Header("Current Weapon")]
    private GameObject currentGun;
    private GunVisual currentGunVisual;

    private bool hasGun;

    [Header("Gun State")]
    private int currentShotCount;
    private bool isReloading;

    [Header("References")]
    private PlayerController playerController;
    private PlayerAnim playerAnim;

    [Header("UI")]
    public GamePlayUI gameplayUI;

    public bool HasGun => hasGun;

    public GameObject CurrentGun => currentGun;

    public GunVisual CurrentGunVisual => currentGunVisual;

    public GunData CurrentGunData => currentGunVisual != null ? currentGunVisual.gunData : null;

    public int CurrentAmmo => currentShotCount;

    public bool IsReloading => isReloading;

    public Transform GunHolder => gunHolder;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAnim = GetComponent<PlayerAnim>();
    }

    // =====================================================
    // SETUP
    // =====================================================

    public void SetGunHolder(Transform newGunHolder)
    {
        gunHolder = newGunHolder;

        Debug.Log("PlayerWeapon | GunHolder đã được cập nhật.");
    }

    public void SetGameplayUI(GamePlayUI ui)
    {
        gameplayUI = ui;

        if (gameplayUI != null)
        {
            gameplayUI.ResetAmmoBar();
        }
    }

    // =====================================================
    // EQUIP
    // =====================================================

    public void EquipGun( GameObject gunPrefab, int ammo = -1, bool wasReloading = false)
    {
        if (gunPrefab == null)
        {
            Debug.LogWarning("PlayerWeapon | gunPrefab null.");

            return;
        }

        if (gunHolder == null)
        {
            Debug.LogError("PlayerWeapon | GunHolder chưa được gán.");

            return;
        }

        SetReloadingState(false);

        // ---------------------------------------------
        // DROP WEAPON CŨ
        // ---------------------------------------------

        if (currentGun != null)
        {
            GunData oldGunData = currentGunVisual != null ? currentGunVisual.gunData : null;

            if (oldGunData != null)
            {
                SpawnDroppedGun( oldGunData, currentShotCount, isReloading
                );
            }

            Destroy(currentGun);

            currentGun = null;
            currentGunVisual = null;
        }

        // ---------------------------------------------
        // EQUIP WEAPON MỚI
        // ---------------------------------------------

        currentGun = Instantiate(
            gunPrefab,
            gunHolder
        );

        currentGun.transform.localPosition =
            Vector3.zero;

        currentGun.transform.localRotation =
            Quaternion.identity;

        currentGunVisual =
            currentGun.GetComponent<GunVisual>();

        if (currentGunVisual == null)
        {
            Debug.LogError(
                "PlayerWeapon | GunPrefab không có GunVisual."
            );

            Destroy(currentGun);
            currentGun = null;

            return;
        }

        // ---------------------------------------------
        // RUNTIME DATA
        // ---------------------------------------------

        GunRuntimeData runtime =
            currentGun.GetComponent<GunRuntimeData>();

        if (runtime == null)
        {
            runtime =
                currentGun.AddComponent<GunRuntimeData>();
        }

        runtime.Initialize(
            currentGunVisual.gunData
        );

        currentShotCount =
            ammo < 0
                ? currentGunVisual.gunData.maxCountShot
                : ammo;

        runtime.currentAmmo = currentShotCount;
        runtime.isReloading = wasReloading;

        // ---------------------------------------------
        // STATE
        // ---------------------------------------------

        isReloading = false;
        hasGun = true;

        gameplayUI?.UpdateAmmoBar(
            currentShotCount,
            currentGunVisual.gunData.maxCountShot
        );

        gameplayUI?.StopReloadVisual();

        playerAnim?.SetHoldGun(true);
    }

    // =====================================================
    // STARTING GUN
    // =====================================================

    public void EquipStartingGun(GunData gunData)
    {
        if (gunData == null)
        {
            Debug.LogWarning(
                "PlayerWeapon | Starting GunData null."
            );

            return;
        }

        EquipGun(
            gunData.gunVisualPrefab
        );
    }

    // =====================================================
    // DROP
    // =====================================================

    public void DropGun()
    {
        SetReloadingState(false);

        if (!hasGun || currentGun == null)
            return;

        GunData gunData =
            currentGunVisual != null
                ? currentGunVisual.gunData
                : null;

        if (gunData != null)
        {
            SpawnDroppedGun(
                gunData,
                currentShotCount,
                isReloading
            );
        }

        Destroy(currentGun);

        currentGun = null;
        currentGunVisual = null;

        hasGun = false;

        gameplayUI?.ResetAmmoBar();

        playerAnim?.SetHoldGun(false);
    }

    // =====================================================
    // SPAWN DROPPED GUN
    // =====================================================

    public void SpawnDroppedGun(GunData gunData,int ammo,bool wasReloading)
    {
        if (gunData == null) return;

        if (gunData.gunItemPrefab == null)
        {
            Debug.LogWarning("PlayerWeapon | GunData không có gunItemPrefab.");

            return;
        }

        Vector3 spawnPos =
            transform.position +
            transform.forward * 1f +
            Vector3.up * 0.5f;

        GameObject droppedGun =
            Instantiate(
                gunData.gunItemPrefab,
                spawnPos,
                Quaternion.identity
            );

        GunItem gunItem =
            droppedGun.GetComponent<GunItem>();

        if (gunItem != null)
        {
            gunItem.Initialize();
            gunItem.SetGunData(gunData);
        }

        GunPickup pickup =
            droppedGun.GetComponent<GunPickup>();

        if (pickup != null)
        {
            pickup.currentAmmo = ammo;
            pickup.isReloading = wasReloading;
        }

        Rigidbody rb =
            droppedGun.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDir =
                transform.forward +
                Vector3.up * 0.7f;

            rb.AddForce(
                throwDir.normalized * 3f,
                ForceMode.Impulse
            );
        }
    }

    public bool HasAmmo()
    {
        return currentShotCount > 0;
    }

    public bool ConsumeAmmo()
    {
        if (!hasGun)
            return false;

        if (currentShotCount <= 0)
            return false;

        currentShotCount--;

        UpdateRuntimeAmmo();
        UpdateAmmoUI();

        return true;
    }

    private void UpdateRuntimeAmmo()
    {
        if (currentGun == null)
            return;

        GunRuntimeData runtime =
            currentGun.GetComponent<GunRuntimeData>();

        if (runtime != null)
        {
            runtime.currentAmmo =
                currentShotCount;
        }
    }

   

public void SetReloadingState(bool value)
{
    isReloading = value;

    GunRuntimeData runtime =
        currentGun != null
            ? currentGun.GetComponent<GunRuntimeData>()
            : null;

    if (runtime != null)
    {
        runtime.isReloading = value;
    }
}


public void SetFullAmmo()
{
    GunData gunData = CurrentGunData;

    if (gunData == null) return;

    currentShotCount = gunData.maxCountShot;

    UpdateRuntimeAmmo();
    UpdateAmmoUI();
}


public void UpdateAmmoUI()
{
    GunData gunData = CurrentGunData;

    if (gunData == null) return;

    gameplayUI?.UpdateAmmoBar( currentShotCount, gunData.maxCountShot);
}


}
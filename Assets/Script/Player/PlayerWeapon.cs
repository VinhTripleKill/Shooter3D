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

    [Header("Character Combat Stats")]
    private CharacterStats characterStats;

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
    private PlayerAmmo playerAmmo;
    public CharacterStats CharacterStats => characterStats;

    public float CritRate => characterStats != null ? characterStats.critRate : 0f;

    public float CritDamage => characterStats != null ? characterStats.critDamage : 1f;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAnim = GetComponent<PlayerAnim>();
        playerAmmo = GetComponent<PlayerAmmo>();
    }

    public void SetCharacterStats(CharacterStats stats)
    {
        characterStats = stats;

        if (characterStats == null)
        {
            Debug.LogWarning("PlayerWeapon | CharacterStats null.");
            return;
        }

        Debug.Log( $"PlayerWeapon | " + $"CritRate: {characterStats.critRate} | " + $"CritDamage: {characterStats.critDamage}" );
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

    public void EquipGun(GameObject gunPrefab,int ammo = -1,bool wasReloading = false)
    {
        if (gunPrefab == null)
        {
            Debug.LogWarning( "PlayerWeapon | gunPrefab null." );

            return;
        }

        if (gunHolder == null)
        {
            Debug.LogError("PlayerWeapon | GunHolder chưa được gán." );

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
                SpawnDroppedGun( oldGunData, playerAmmo != null ? playerAmmo.CurrentAmmo : 0, isReloading );
            }

            Destroy(currentGun);

            currentGun = null;
            currentGunVisual = null;
        }

        // ---------------------------------------------
        // EQUIP WEAPON MỚI
        // ---------------------------------------------

        currentGun = Instantiate( gunPrefab, gunHolder );

        currentGun.transform.localPosition = Vector3.zero;

        currentGun.transform.localRotation = Quaternion.identity;

        currentGunVisual = currentGun.GetComponent<GunVisual>();

        if (currentGunVisual == null)
        {
            Debug.LogError( "PlayerWeapon | GunPrefab không có GunVisual." );

            Destroy(currentGun);

            currentGun = null;

            return;
        }

        // ---------------------------------------------
        // RUNTIME DATA
        // ---------------------------------------------

        GunRuntimeData runtime = currentGun.GetComponent<GunRuntimeData>();

        if (runtime == null)
        {
            runtime = currentGun.AddComponent<GunRuntimeData>();
        }

        runtime.Initialize( currentGunVisual.gunData );

        playerAmmo?.InitializeAmmo( currentGunVisual.gunData, ammo );

        runtime.isReloading = wasReloading;

        // ---------------------------------------------
        // STATE
        // ---------------------------------------------

        isReloading = false;

        hasGun = true;

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
            Debug.LogWarning( "PlayerWeapon | Starting GunData null." );

            return;
        }

        EquipGun( gunData.gunVisualPrefab );
    }

    // =====================================================
    // DROP
    // =====================================================

    public void DropGun()
    {
        SetReloadingState(false);

        if (!hasGun || currentGun == null) return;

        GunData gunData = currentGunVisual != null ? currentGunVisual.gunData : null;

        if (gunData != null)
        {
            SpawnDroppedGun(gunData, playerAmmo != null ? playerAmmo.CurrentAmmo : 0,isReloading);
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

        Vector3 spawnPos = transform.position + transform.forward * 1f + Vector3.up * 0.5f;

        GameObject droppedGun = Instantiate( gunData.gunItemPrefab, spawnPos, Quaternion.identity );

        GunItem gunItem = droppedGun.GetComponent<GunItem>();

        if (gunItem != null)
        {
            gunItem.Initialize();
            gunItem.SetGunData(gunData);
        }

        GunPickup pickup = droppedGun.GetComponent<GunPickup>();

        if (pickup != null)
        {
            pickup.currentAmmo = ammo;
            pickup.isReloading = wasReloading;
        }

        Rigidbody rb = droppedGun.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDir = transform.forward + Vector3.up * 0.7f;

            rb.AddForce( throwDir.normalized * 3f, ForceMode.Impulse );
        }
    }





    public void SetReloadingState(bool value)
    {
        isReloading = value;

        GunRuntimeData runtime =  currentGun != null ? currentGun.GetComponent<GunRuntimeData>() : null;

        if (runtime != null)
        {
            runtime.isReloading = value;
        }
    }

}
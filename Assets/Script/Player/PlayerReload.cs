
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerReload : MonoBehaviour
{
    // =====================================================
    // REFERENCES
    // =====================================================

    private PlayerController playerController;
    private PlayerWeapon playerWeapon;
private PlayerAmmo playerAmmo;

    private InputAction reloadAction;

    private Button reloadButton;

    // =====================================================
    // STATE
    // =====================================================

    private bool isReloading;
    private Coroutine reloadCoroutine;

    public bool IsReloading => isReloading;

private void Awake()
{
    playerController =
        GetComponent<PlayerController>();

    playerWeapon =
        GetComponent<PlayerWeapon>();

    playerAmmo =
        GetComponent<PlayerAmmo>();

    PlayerInput playerInput =
        GetComponent<PlayerInput>();

    if (playerInput != null)
    {
        reloadAction =
            playerInput.actions["Reload"];
    }
}
    // =====================================================
    // ENABLE / DISABLE
    // =====================================================

    private void OnEnable()
    {
        if (reloadAction != null)
        {
            reloadAction.performed +=
                ReloadPerformed;
        }

        if (reloadButton != null)
        {
            reloadButton.onClick.AddListener(
                ManualReload
            );
        }
    }

    private void OnDisable()
    {
        if (reloadAction != null)
        {
            reloadAction.performed -=
                ReloadPerformed;
        }

        if (reloadButton != null)
        {
            reloadButton.onClick.RemoveListener(
                ManualReload
            );
        }
    }

    // =====================================================
    // INPUT ACTION
    // =====================================================

    private void ReloadPerformed(
        InputAction.CallbackContext ctx)
    {
        ManualReload();
    }

    // =====================================================
    // SET RELOAD BUTTON
    // =====================================================

    public void SetReloadButton(
        Button button)
    {
        // Hủy button cũ
        if (reloadButton != null)
        {
            reloadButton.onClick.RemoveListener(
                ManualReload
            );
        }

        reloadButton = button;

        // Gắn button mới
        if (reloadButton != null)
        {
            reloadButton.onClick.AddListener(
                ManualReload
            );
        }
    }

    // =====================================================
    // MANUAL RELOAD
    // =====================================================

    public void ManualReload()
    {
        if (playerController == null)
            return;

        if (playerWeapon == null)
            return;

        // Player chết
        if (playerController.IsDead())
            return;

        // Không có súng
        if (!playerWeapon.HasGun)
            return;

        // Đang reload
        if (isReloading)
            return;

        GunData gunData =
            playerWeapon.CurrentGunData;

        if (gunData == null)
            return;

if (playerAmmo == null)
    return;

if (playerAmmo.CurrentAmmo >=
    gunData.maxCountShot)
{
    return;
}

        // Bắt đầu reload
        reloadCoroutine =
            StartCoroutine(
                ReloadRoutine()
            );
    }

    // =====================================================
    // CANCEL RELOAD
    // =====================================================

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(
                reloadCoroutine
            );

            reloadCoroutine = null;
        }

        isReloading = false;

        if (playerWeapon != null)
        {
            playerWeapon.SetReloadingState(
                false
            );
        }

        if (playerWeapon != null &&
            playerWeapon.gameplayUI != null)
        {
            playerWeapon.gameplayUI
                .StopReloadVisual();
        }
    }

    // =====================================================
    // RELOAD ROUTINE
    // =====================================================

    private IEnumerator ReloadRoutine()
    {
        if (isReloading)
            yield break;

        GunData gunData =
            playerWeapon.CurrentGunData;

        if (gunData == null)
            yield break;

        // ---------------------------------------------
        // START
        // ---------------------------------------------

        isReloading = true;

        playerWeapon.SetReloadingState(
            true
        );

        playerWeapon.gameplayUI?.StartReloadVisual(
            gunData.timeReload
        );

        // ---------------------------------------------
        // WAIT
        // ---------------------------------------------

        yield return new WaitForSeconds(
            gunData.timeReload
        );

        // ---------------------------------------------
        // CHECK WEAPON
        // ---------------------------------------------

        if (!playerWeapon.HasGun)
        {
            FinishReload();
            yield break;
        }

        if (playerWeapon.CurrentGunData !=
            gunData)
        {
            FinishReload();
            yield break;
        }

        // ---------------------------------------------
        // FULL AMMO
        // ---------------------------------------------

        playerAmmo.SetFullAmmo();

        // ---------------------------------------------
        // FINISH
        // ---------------------------------------------

        FinishReload();
    }

    // =====================================================
    // FINISH
    // =====================================================

    private void FinishReload()
    {
        isReloading = false;

        reloadCoroutine = null;

        if (playerWeapon != null)
        {
            playerWeapon.SetReloadingState(
                false
            );

            playerAmmo.UpdateAmmoUI();
        }

        if (playerWeapon != null &&
            playerWeapon.gameplayUI != null)
        {
            playerWeapon.gameplayUI
                .StopReloadVisual();
        }
    }
}

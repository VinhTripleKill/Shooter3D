
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerWeapon playerWeapon;
    private PlayerReload playerReload;
    private PlayerBullet playerBullet;
    private PlayerAnim playerAnim;
    private PlayerAmmo playerAmmo;

    [Header("Aim")]

    [SerializeField]
    private float autoAimRange = 15f;

    public float AutoAimRange => autoAimRange;

    private bool useAutoAim = true;
    private bool useMouseAim;

    private Vector2 manualAimDirection;

    private Transform currentTarget;

    [Header("Input")]

    private InputAction attackAutoAction;
    private InputAction attackManualAction;

    private bool isHoldingAttack;

    [Header("Shoot")]

    private float nextShotTime;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        playerWeapon = GetComponent<PlayerWeapon>();

        playerReload = GetComponent<PlayerReload>();

        playerBullet = GetComponent<PlayerBullet>();

        playerAnim = GetComponent<PlayerAnim>();

        playerAmmo = GetComponent<PlayerAmmo>();

        PlayerInput playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            attackAutoAction = playerInput.actions["AttackAuto"];

            attackManualAction = playerInput.actions["AttackManual"];
        }
    }


    public void SetAutoAimRange(float range)
    {
        if (range <= 0f)
        {
            Debug.LogWarning("PlayerShoot | AttackRange không hợp lệ. " + "Giữ giá trị AutoAimRange hiện tại." );

            return;
        }

        autoAimRange = range;

        Debug.Log( $"PlayerShoot | AutoAimRange = {autoAimRange}" );
    }

    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        if (attackAutoAction != null)
        {
            attackAutoAction.started += AttackAutoStarted;

            attackAutoAction.canceled += AttackCanceled;
        }

        if (attackManualAction != null)
        {
            attackManualAction.started += AttackManualStarted;

            attackManualAction.canceled += AttackCanceled;
        }
    }
    private void OnDisable()
    {
        if (attackAutoAction != null)
        {
            attackAutoAction.started -= AttackAutoStarted;

            attackAutoAction.canceled -= AttackCanceled;
        }

        if (attackManualAction != null)
        {
            attackManualAction.started -= AttackManualStarted;

            attackManualAction.canceled -= AttackCanceled;
        }
    }

    private void Update()
    {
        if (playerController == null) return;

        if (playerController.IsDead()) return;

        if (playerWeapon == null) return;

        if (!playerWeapon.HasGun) return;

        if (!useAutoAim && useMouseAim)
        {
            UpdateMouseAimDirection();
        }

        // ---------------------------------------------
        // KHÔNG GIỮ BẮN
        // ---------------------------------------------

        if (!isHoldingAttack)
            return;

        // ---------------------------------------------
        // AIM
        // ---------------------------------------------

        if (useAutoAim)
        {
            AimNearestEnemy();
        }
        else
        {
            ManualAim();
        }

        // ---------------------------------------------
        // SHOOT
        // ---------------------------------------------

        TryShoot();
    }

    // =====================================================
    // INPUT
    // =====================================================

    private void AttackAutoStarted(InputAction.CallbackContext ctx)
    {
        useAutoAim = true;
        useMouseAim = false;

        isHoldingAttack = true;
    }

    private void AttackManualStarted(InputAction.CallbackContext ctx)
    {
        useAutoAim = false;
        useMouseAim = true;

        isHoldingAttack = true;
    }

    private void AttackCanceled(InputAction.CallbackContext ctx)
    {
        isHoldingAttack = false;
    }

    // =====================================================
    // EXTERNAL ATTACK STATE
    // =====================================================

    public void SetAttackState(bool value)
    {
        isHoldingAttack = value;
    }

    public void SetAutoAim(bool value)
    {
        useAutoAim = value;
    }

    public void SetManualAimDirection(Vector2 direction)
    {
        manualAimDirection = direction;
    }

    public void SetJoystickManualAim( Vector2 direction)
    {
        useAutoAim = false;
        useMouseAim = false;

        if (direction.sqrMagnitude <= 0.001f)
        {
            manualAimDirection = Vector2.zero;

            return;
        }

        manualAimDirection = direction.normalized;
    }

    // =====================================================
    // MOUSE AIM
    // =====================================================

    private void UpdateMouseAimDirection()
    {
        Camera cam = Camera.main;

        if (cam == null) return;

        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = cam.ScreenPointToRay( mousePos );

        Plane groundPlane = new Plane( Vector3.up, Vector3.zero );

        if (!groundPlane.Raycast( ray, out float distance))
        {
            return;
        }

        Vector3 hitPoint =
            ray.GetPoint(distance);

        Vector3 direction =
            hitPoint -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <=
            0.001f)
        {
            return;
        }

        manualAimDirection =
            new Vector2(
                direction.x,
                direction.z
            ).normalized;
    }

    // =====================================================
    // MANUAL AIM
    // =====================================================

    private void ManualAim()
    {
        if (manualAimDirection.sqrMagnitude <
            0.01f)
        {
            return;
        }

        Vector3 direction =
            new Vector3(
                manualAimDirection.x,
                0f,
                manualAimDirection.y
            );

        if (direction.sqrMagnitude <=
            0.001f)
        {
            return;
        }

        transform.forward =
            direction.normalized;
    }

    // =====================================================
    // AUTO AIM
    // =====================================================

    private void AimNearestEnemy()
    {
        currentTarget =
            CombatTargetFinder
                .RotateToNearestTarget(
                    transform,
                    autoAimRange
                );
    }

    // =====================================================
    // TRY SHOOT
    // =====================================================

    private void TryShoot()
    {
        if (playerController == null) return;

        if (playerWeapon == null) return;

        if (playerController.IsDead()) return;

        if (!playerWeapon.HasGun) return;

        if (playerController.IsSprintingPublic()) return;

        if (playerWeapon.IsReloading) return;

        GunData gunData = playerWeapon.CurrentGunData;

        if (gunData == null) return;


if (playerAmmo == null)
    return;

if (!playerAmmo.HasAmmo())
{
    if (playerReload != null)
    {
        playerReload.ManualReload();
    }

    return;
}


        if (Time.time < nextShotTime) return;

        Shot();

        nextShotTime = Time.time + gunData.timeBetweenShots;
    }

    private void Shot()
    {
        GunData gunData = playerWeapon.CurrentGunData;

        if (gunData == null) return;

        Transform firePoint = playerWeapon.CurrentGunVisual != null ? playerWeapon.CurrentGunVisual.GetFirePoint(): null;

        if (firePoint == null)
        {
            Debug.LogWarning("PlayerShoot | Không tìm thấy FirePoint."); return;
        }

        if (playerAmmo == null)
    return;

if (!playerAmmo.ConsumeAmmo())
    return;

        if (playerBullet != null)
        {
            playerBullet.Fire( gunData, firePoint, currentTarget );
        }


        if (gunData.recoilForce > 0f)
        {
            CharacterController cc = GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.Move(-transform.forward *gunData.recoilForce);
            }
        }
    }
}

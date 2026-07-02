using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerAnim playerAnim;

    [SerializeField] private float autoAimRange = 15f;

    private InputAction attackAutoAction;
    private InputAction attackManualAction;
    private InputAction reloadAction;

    [Header("Gun")]
    public Transform gunHolder;
    private GunVisual currentGunVisual;
    private GameObject currentGun;
    private bool hasGun = false;
    private bool useAutoAim = true;
    private Vector2 manualAimDirection;
    private int currentShotCount;
    private bool isHoldingAttack;
    private bool isReloading;
    private Transform currentTarget;
    private bool useMouseAim;

    [Header("Aim Visual")]
    [SerializeField] private GunRayDebug gunRayDebug;

    [Header("UI")]
    [SerializeField] private GamePlayUI gameplayUI;

    private Coroutine reloadCoroutine;
    private float nextShotTime;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAnim = GetComponent<PlayerAnim>();

        var playerInput = GetComponent<PlayerInput>();
        attackAutoAction = playerInput.actions["AttackAuto"];
        attackManualAction = playerInput.actions["AttackManual"];
        reloadAction = playerInput.actions["Reload"];
        BulletProjecTile.OnSuccessfulHit += RecoverManaByProjectileHit;
        gameplayUI?.ResetAmmoBar();
        if (gameplayUI != null)
        {
            gameplayUI.GetReloadButton().onClick.AddListener(OnReloadButtonClicked);
        }
    }

    public bool HasGun() => hasGun;

    public void SetAttackState(bool value) => isHoldingAttack = value;
    public void SetAutoAim(bool value) => useAutoAim = value;
    public void SetManualAimDirection(Vector2 direction) => manualAimDirection = direction;
    private void AttackCanceled(InputAction.CallbackContext ctx) => isHoldingAttack = false;

    private void OnDestroy()
    {
        if (gameplayUI != null)
            gameplayUI.GetReloadButton().onClick.RemoveListener(OnReloadButtonClicked);
            BulletProjecTile.OnSuccessfulHit -= RecoverManaByProjectileHit;
    }

    private void OnReloadButtonClicked() => ManualReload();

    private void ManualReload()
    {
        if (playerController.IsDead() || !hasGun || isReloading) return;

        GunData gunData = currentGunVisual.gunData;

        if (currentShotCount >= gunData.maxCountShot) return;

        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private void ReloadPerformed(InputAction.CallbackContext ctx)
    {
        if (!playerController.IsDead()) ManualReload();
    }

    private void OnEnable()
    {
        attackAutoAction.started += AttackAutoStarted;
        attackAutoAction.canceled += AttackCanceled;
        attackManualAction.started += AttackManualStarted;
        attackManualAction.canceled += AttackCanceled;
        reloadAction.performed += ReloadPerformed;
    }

    private void OnDisable()
    {
        attackAutoAction.started -= AttackAutoStarted;
        attackAutoAction.canceled -= AttackCanceled;
        attackManualAction.started -= AttackManualStarted;
        attackManualAction.canceled -= AttackCanceled;
        reloadAction.performed -= ReloadPerformed;
    }

    private void AttackAutoStarted(InputAction.CallbackContext ctx)
    {
        useAutoAim = true;
        isHoldingAttack = true;
    }

    private void AttackManualStarted(InputAction.CallbackContext ctx)
    {
        useAutoAim = false;
        useMouseAim = true;
        isHoldingAttack = true;
    }

    public void SetJoystickManualAim(Vector2 direction)
    {
        useAutoAim = false;
        useMouseAim = false;
        manualAimDirection = direction.normalized;
    }

    private void Update()
    {
        if (playerController.IsDead()) return;

        if (hasGun && gunRayDebug != null)
            gunRayDebug.UpdateAimLines();

        if (!useAutoAim && useMouseAim)
            UpdateMouseAimDirection();

        if (isHoldingAttack)
        {
            if (useAutoAim)
                AimNearestEnemy();
            else
                ManualAim();

            TryShoot();
        }
    }
    private void UpdateMouseAimDirection()
    {
        Camera cam = Camera.main;

        if (cam == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = cam.ScreenPointToRay(mousePos);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 direction = hitPoint - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                manualAimDirection = new Vector2( direction.x, direction.z);
            }
        }
    }

    private void ManualAim()
    {
        if (manualAimDirection.sqrMagnitude < 0.01f) return;

        Vector3 direction = new Vector3( manualAimDirection.x, 0f, manualAimDirection.y);

        transform.forward = direction.normalized;
    }
    private void AimNearestEnemy()
    {
        currentTarget = CombatTargetFinder.RotateToNearestTarget( transform, autoAimRange);
    }

    private void TryShoot()
    {
        if (playerController.IsDead())return;
        if (!hasGun) return;
        if (playerController.IsSprintingPublic()) return;
        if (isReloading) return;


        GunData gunData = currentGunVisual.gunData;

        if (Time.time < nextShotTime) return; // cần khai báo nextShotTime

        if (currentShotCount <= 0)
        {
            if (!isReloading)
            {
                reloadCoroutine = StartCoroutine(ReloadRoutine());
            }

            return;
        }

        Shoot();
        currentShotCount--;
        currentGun.GetComponent<GunRuntimeData>().currentAmmo = currentShotCount;
        gameplayUI?.UpdateAmmoBar(currentShotCount,gunData.maxCountShot);
        nextShotTime = Time.time + gunData.timeBetweenShots;
    }

    private void Shoot()
    {
        GunData gunData = currentGunVisual.gunData;

        if (gunData.fireType == GunData.GunFireType.Raycast)
        {
            ShootRaycast(gunData);
        }
        else
        {
            ShootProjectile(gunData);
        }

        if (gunData.recoilForce > 0)
        {
            CharacterController cc = GetComponent<CharacterController>();

            cc.Move( -transform.forward * gunData.recoilForce);
        }
    }
    private void ShootProjectile( GunData gunData)
    {
        Transform firePoint = currentGunVisual.GetFirePoint();

        int pelletCount = gunData.pelletCount;

        float angleStep = gunData.angleBetweenBullets;

        float startAngle = -(angleStep * (pelletCount - 1)) / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;

            Quaternion fixedSpread = Quaternion.Euler( 0, currentAngle, 0);

            Quaternion randomSpread =
                Quaternion.Euler(
                    Random.Range(
                        -gunData.randomSpreadX,
                        gunData.randomSpreadX),
                    Random.Range(
                        -gunData.randomSpreadY,
                        gunData.randomSpreadY),
                    Random.Range(
                        -gunData.randomSpreadZ,
                        gunData.randomSpreadZ));

            Quaternion finalRotation = firePoint.rotation * fixedSpread * randomSpread;

            GameObject bulletObj = Instantiate( gunData.bulletPrefab, firePoint.position, finalRotation);

            BulletProjecTile bullet = bulletObj.GetComponent<BulletProjecTile>();
            bullet.Initialize(
                finalRotation * Vector3.forward,
                gunData.bulletSpeed,
                gunData.bulletLifeTime,
                gunData.damage,
                gunData.hitMask,
                gunData.interactionMask,
                currentTarget);
        }
    }


    private void RecoverManaByProjectileHit()
    {
         if (!hasGun) return;

         playerController.RecoverMana(currentGunVisual.gunData.manaRecoveryByHit);
    }
  private void ProcessRaycastHit(GunData gunData, Collider hitCollider)
{
    if (((1 << hitCollider.gameObject.layer) & gunData.interactionMask) == 0)
        return;

    playerController.RecoverMana(gunData.manaRecoveryByHit);
}

    private void ShootRaycast(GunData gunData)
    {
        Transform firePoint = currentGunVisual.GetFirePoint();

        int pelletCount = gunData.pelletCount;
        float angleStep = gunData.angleBetweenBullets;
        float startAngle = -(angleStep * (pelletCount - 1)) / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;

            Quaternion spread =
                Quaternion.Euler(
                    Random.Range(-gunData.randomSpreadX, gunData.randomSpreadX),
                    Random.Range(-gunData.randomSpreadY, gunData.randomSpreadY),
                    Random.Range(-gunData.randomSpreadZ, gunData.randomSpreadZ)
                );

            Vector3 direction = (firePoint.rotation *Quaternion.Euler(0, currentAngle, 0) * spread) * Vector3.forward;

            Vector3 startPos = firePoint.position;
            Vector3 endPos;

            RaycastHit hit;

            if (Physics.SphereCast(
         startPos,
         gunData.hitRadius,
         direction,
         out hit,
         gunData.raycastDistance,
         gunData.hitMask,
         QueryTriggerInteraction.Ignore))
            {
                endPos = hit.point;

                Debug.Log($"Hit: {hit.collider.name}");

                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

                if (damageable != null)
                {
                   damageable.TakeDamage(gunData.damage);
                   ProcessRaycastHit(gunData,hit.collider);
                }

            }
            else
            {
                endPos = startPos + direction * gunData.raycastDistance;
            }

            SpawnTrail(startPos, endPos, gunData);
        }
    }
    private void SpawnTrail(Vector3 start, Vector3 end, GunData gunData)
    {
        GameObject trailObj = Instantiate(gunData.bulletPrefab, start, Quaternion.identity);

        BulletRayTrail trail = trailObj.GetComponent<BulletRayTrail>();

        float travelTime = Vector3.Distance(start, end) / gunData.bulletSpeed;

        trail.Initialize(start, end, gunData.bulletSpeed);
    }

    public void EquipGun(GameObject gunPrefab, int ammo = -1, bool wasReloading = false)
    {
        CancelReload();

        if (currentGun != null)
        {
            GunData oldGunData = currentGunVisual.gunData;
            SpawnDroppedGun(oldGunData, currentShotCount, isReloading);   // ← SỬA Ở ĐÂY
            Destroy(currentGun);
        }

        currentGun = Instantiate(gunPrefab, gunHolder);
        currentGun.transform.localPosition = Vector3.zero;
        currentGun.transform.localRotation = Quaternion.identity;

        currentGunVisual = currentGun.GetComponent<GunVisual>();

        GunRuntimeData runtime = currentGun.GetComponent<GunRuntimeData>() ?? currentGun.AddComponent<GunRuntimeData>();
        runtime.Initialize(currentGunVisual.gunData);

        if (gunRayDebug != null)
            gunRayDebug.Initialize(currentGunVisual.gunData, currentGunVisual.GetFirePoint());

        currentShotCount = (ammo < 0) ? currentGunVisual.gunData.maxCountShot : ammo;
        runtime.currentAmmo = currentShotCount;
        runtime.isReloading = wasReloading;

        isReloading = false;
        hasGun = true;

        gameplayUI?.UpdateAmmoBar(currentShotCount, currentGunVisual.gunData.maxCountShot);
        playerAnim.SetHoldGun(true);
        gameplayUI?.StopReloadVisual();
    }

    public void DropGun()
    {
        CancelReload();
        if (!hasGun || currentGun == null) return;

        GunData gunData = currentGunVisual.gunData;
        SpawnDroppedGun(gunData, currentShotCount, isReloading);   // ← SỬA Ở ĐÂY

        Destroy(currentGun);
        currentGun = null;
        currentGunVisual = null;
        hasGun = false;

        if (gunRayDebug != null) gunRayDebug.Cleanup();
        gameplayUI?.ResetAmmoBar();
        playerAnim.SetHoldGun(false);
    }
    public void SpawnDroppedGun(GunData gunData, int ammo, bool wasReloading)
    {
        Vector3 spawnPos = transform.position + transform.forward * 1f + Vector3.up * 0.5f;
        GameObject droppedGun = Instantiate(gunData.gunItemPrefab, spawnPos, Quaternion.identity);
        GunPickup pickup = droppedGun.GetComponent<GunPickup>();
        pickup.currentAmmo = ammo;
        pickup.isReloading = wasReloading;

        Rigidbody rb = droppedGun.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwDir = transform.forward + Vector3.up * 0.7f;
            rb.AddForce(throwDir.normalized * 3f, ForceMode.Impulse);
        }
    }

    private void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        isReloading = false;
        isHoldingAttack = false;
        gameplayUI?.StopReloadVisual();
    }


    private IEnumerator ReloadRoutine()
    {
        if (isReloading)
            yield break;

        isReloading = true;

        gameplayUI?.StartReloadVisual();

        GunData gunData = currentGunVisual.gunData;

        yield return new WaitForSeconds(gunData.timeReload);

        currentShotCount = gunData.maxCountShot;
        currentGun.GetComponent<GunRuntimeData>().currentAmmo =currentShotCount;
        isReloading = false;

        gameplayUI?.StopReloadVisual();

        gameplayUI?.UpdateAmmoBar(currentShotCount,gunData.maxCountShot);
    }

}
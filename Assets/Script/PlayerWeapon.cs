using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerAnim playerAnim;
    [SerializeField]
    private float autoAimRange = 15f;
    private InputAction attackAction;
    private InputAction reloadAction;
    private InputAction dropAction;
    private InputAction interactionAction;

    [Header("Gun")]
    public Transform gunHolder;
    private GunVisual currentGunVisual;
    private GameObject currentGun;
    private GunPickup nearbyGun;
    private bool hasGun = false;
    private bool useAutoAim = true;
    private Vector2 manualAimDirection;
    private int currentShotCount;
    private bool isHoldingAttack;
    private bool isReloading;
    [Header("UI")]
    [SerializeField] private GamePlayUI gameplayUI;
    private Coroutine reloadCoroutine;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAnim = GetComponent<PlayerAnim>();

        var playerInput = GetComponent<PlayerInput>();

        attackAction = playerInput.actions["Attack"];
        reloadAction = playerInput.actions["Reload"];
        dropAction = playerInput.actions["Drop"];
        interactionAction = playerInput.actions["Interaction"];
        gameplayUI?.ResetAmmoBar();
        if (gameplayUI != null)
            gameplayUI.GetReloadButton().onClick.AddListener(OnReloadButtonClicked);
    }

    public bool HasGun()
    {
        return hasGun;
    }

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

    private void OnDestroy()
    {
        if (gameplayUI != null)
            gameplayUI.GetReloadButton().onClick.RemoveListener(OnReloadButtonClicked);
    }
    private void OnReloadButtonClicked()
    {
        ManualReload();
    }

    private void ManualReload()
    {
        if (!hasGun || isReloading)
            return;

        GunData gunData = currentGunVisual.gunData;

        if (currentShotCount >= gunData.maxCountShot)
            return;

        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }
    private void ReloadPerformed(InputAction.CallbackContext ctx)
    {
        ManualReload();
    }
    private void OnEnable()
    {
        attackAction.started += AttackStarted;
        attackAction.canceled += AttackCanceled;
        reloadAction.performed += ReloadPerformed;
        dropAction.performed += DropPerformed;
        interactionAction.performed += InteractionPerformed;
    }

    private void OnDisable()
    {
        attackAction.started -= AttackStarted;
        attackAction.canceled -= AttackCanceled;
        reloadAction.performed -= ReloadPerformed;
        dropAction.performed -= DropPerformed;
        interactionAction.performed -= InteractionPerformed;
    }

    private void Update()
    {
        if (isHoldingAttack)
        {
            if (useAutoAim)
            {
                AimNearestEnemy();
            }
            else
            {
                ManualAim();
            }

            TryShoot();
        }
    }
    private void ManualAim()
    {
        if (manualAimDirection.sqrMagnitude < 0.01f)
            return;

        Vector3 direction =
            new Vector3(
                manualAimDirection.x,
                0f,
                manualAimDirection.y);

        transform.forward =
            direction.normalized;
    }
    private void AimNearestEnemy()
    {
        Transform target =
            GetNearestEnemy();

        if (target == null)
            return;

        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        transform.forward =
            direction.normalized;
    }

    private Transform GetNearestEnemy()
    {
        Transform nearestEnemy = null;

        float nearestDistance =
            Mathf.Infinity;

        foreach (Enemy enemy in Enemy.AllEnemies)
        {
            if (enemy == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    enemy.transform.position);

            if (distance > autoAimRange)
                continue;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy =
                    enemy.transform;
            }
        }

        return nearestEnemy;
    }

    private void TryShoot()
    {
        if (!hasGun) return;
        if (playerController.IsSprintingPublic()) return;
        if (isReloading) return;

        GunData gunData = currentGunVisual.gunData;

        if (Time.time < nextShotTime) return; // cần khai báo nextShotTime

            if (currentShotCount <= 0)
            {
                if (!isReloading)
                {
                    reloadCoroutine =
                        StartCoroutine(ReloadRoutine());
                }

                return;
            }
        

        Shoot();
        currentShotCount--;
        gameplayUI?.UpdateAmmoBar(
    currentShotCount,
    gunData.maxCountShot);
        nextShotTime = Time.time + gunData.timeBetweenShots;
    }

    private float nextShotTime;

    private void Shoot()
    {
        GunData gunData = currentGunVisual.gunData;
        Transform firePoint = currentGunVisual.GetFirePoint();
        int pelletCount = gunData.pelletCount;
        float angleStep = gunData.angleBetweenBullets;
        float startAngle = -(angleStep * (pelletCount - 1)) / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion fixedSpread = Quaternion.Euler(0, currentAngle, 0);
            Quaternion randomSpread = Quaternion.Euler(
                Random.Range(-gunData.randomSpreadX, gunData.randomSpreadX),
                Random.Range(-gunData.randomSpreadY, gunData.randomSpreadY),
                Random.Range(-gunData.randomSpreadZ, gunData.randomSpreadZ)
            );

            Quaternion finalRotation = firePoint.rotation * fixedSpread * randomSpread;

            GameObject bulletObj = Instantiate(gunData.bulletPrefab, firePoint.position, finalRotation);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Initialize(finalRotation * Vector3.forward, gunData.bulletSpeed, gunData.bulletLifeTime, gunData.damage);
        }

        // Recoil
        if (gunData.recoilForce > 0)
        {
            CharacterController cc = GetComponent<CharacterController>();
            cc.Move(-transform.forward * gunData.recoilForce);
        }
    }

    public void EquipGun(GameObject gunPrefab)
    {
        CancelReload();
        if (currentGun != null)
        {
            GunData oldGunData = currentGunVisual.gunData;
            SpawnDroppedGun(oldGunData);
            Destroy(currentGun);
        }

        currentGun = Instantiate(gunPrefab, gunHolder);
        currentGun.transform.localPosition = Vector3.zero;
        currentGun.transform.localRotation = Quaternion.identity;

        currentGunVisual = currentGun.GetComponent<GunVisual>();
        currentShotCount = currentGunVisual.gunData.maxCountShot;
        isReloading = false;
        hasGun = true;
        currentShotCount = currentGunVisual.gunData.maxCountShot;

        gameplayUI?.UpdateAmmoBar(
            currentShotCount,
            currentGunVisual.gunData.maxCountShot);
        playerAnim.SetHoldGun(true);
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
       
        isReloading = false;

        gameplayUI?.StopReloadVisual();

        gameplayUI?.UpdateAmmoBar(
            currentShotCount,
            gunData.maxCountShot);
    }

    private void AttackStarted(InputAction.CallbackContext ctx) => isHoldingAttack = true;
    private void AttackCanceled(InputAction.CallbackContext ctx) => isHoldingAttack = false;

    private void InteractionPerformed(InputAction.CallbackContext ctx)
    {
        if (nearbyGun != null)
            nearbyGun.Pickup(this);   // Truyền PlayerWeapon thay vì PlayerController
    }

    private void DropPerformed(InputAction.CallbackContext ctx) => DropGun();

    private void DropGun()
    {
        CancelReload();
        if (!hasGun || currentGun == null)
            return;

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        isReloading = false;
        isHoldingAttack = false;

        gameplayUI?.StopReloadVisual();

        GunData gunData = currentGunVisual.gunData;

        SpawnDroppedGun(gunData);

        Destroy(currentGun);

        currentGun = null;
        currentGunVisual = null;
        hasGun = false;

        gameplayUI?.ResetAmmoBar();

        playerAnim.SetHoldGun(false);
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

    private void SpawnDroppedGun(GunData gunData)
    {
        Vector3 spawnPos = transform.position + transform.forward * 1f + Vector3.up * 0.5f;
        GameObject droppedGun = Instantiate(gunData.gunItemPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = droppedGun.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwDir = transform.forward + Vector3.up * 0.7f;
            rb.AddForce(throwDir.normalized * 3f, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out GunPickup gun))
            nearbyGun = gun;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GunPickup gun) && nearbyGun == gun)
            nearbyGun = null;
    }
}
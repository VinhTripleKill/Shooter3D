using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerWeapon playerWeapon;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        playerWeapon = GetComponent<PlayerWeapon>();

        BulletProjectile.OnSuccessfulHit += RecoverManaByProjectileHit;

        MissileProjectile.OnSuccessfulHit += RecoverManaByProjectileHit;
    }

    private void OnDestroy()
    {
        BulletProjectile.OnSuccessfulHit -= RecoverManaByProjectileHit;

        MissileProjectile.OnSuccessfulHit -= RecoverManaByProjectileHit;
    }

    // =====================================================
    // MAIN FIRE
    // =====================================================

    public void Fire(
        GunData gunData,
        Transform firePoint,
        Transform currentTarget)
    {
        if (gunData == null)
        {
            Debug.LogWarning(
                "PlayerBullet | GunData null." );

            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning(
                "PlayerBullet | FirePoint null." );

            return;
        }

        switch (gunData.fireType)
        {
            case GunData.GunFireType.Raycast:

                ShootRaycast(
                    gunData,
                    firePoint
                );

                break;

            case GunData.GunFireType.Bullet:

                ShootBullet(
                    gunData,
                    firePoint
                );

                break;

            case GunData.GunFireType.Missile:

                ShootMissile(
                    gunData,
                    firePoint,
                    currentTarget
                );

                break;

            default:

                Debug.LogWarning(
                    $"PlayerBullet | FireType chưa được xử lý: {gunData.fireType}"
                );

                break;
        }
    }

    // =====================================================
    // MISSILE
    // =====================================================

    private void ShootMissile(
        GunData gunData,
        Transform firePoint,
        Transform currentTarget)
    {
        int pelletCount = Mathf.Max(1, gunData.pelletCount);

        float angleStep = gunData.angleBetweenBullets;

        float startAngle = -(angleStep * (pelletCount - 1)) / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle =
                startAngle +
                angleStep * i;

            Quaternion fixedSpread =
                Quaternion.Euler(
                    0f,
                    currentAngle,
                    0f
                );

            Quaternion randomSpread =
                Quaternion.Euler(
                    Random.Range(
                        -gunData.randomSpreadX,
                        gunData.randomSpreadX
                    ),

                    Random.Range(
                        -gunData.randomSpreadY,
                        gunData.randomSpreadY
                    ),

                    Random.Range(
                        -gunData.randomSpreadZ,
                        gunData.randomSpreadZ
                    )
                );

            Quaternion finalRotation =
                firePoint.rotation *
                fixedSpread *
                randomSpread;

            GameObject missileObj =
                Instantiate(
                    gunData.missilePrefab,
                    firePoint.position,
                    finalRotation
                );

            if (missileObj == null)
                continue;

            MissileProjectile missile =
                missileObj.GetComponent<MissileProjectile>();

            if (missile == null)
            {
                Debug.LogWarning(
                    "PlayerBullet | missilePrefab không có MissileProjectile."
                );

                continue;
            }

            missile.Initialize(
                gunData.bulletSpeed,
                gunData.missileLifeTime,
                gunData.damage,
                gunData.hitMask,
                gunData.interactionMask,
                currentTarget
            );
        }
    }

    // =====================================================
    // BULLET PROJECTILE
    // =====================================================

    private void ShootBullet(
        GunData gunData,
        Transform firePoint)
    {
        int pelletCount = Mathf.Max(1, gunData.pelletCount);

        float angleStep = gunData.angleBetweenBullets;

        float startAngle = -(angleStep * (pelletCount - 1)) / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle =
                startAngle +
                angleStep * i;

            Quaternion fixedSpread =
                Quaternion.Euler(
                    0f,
                    currentAngle,
                    0f
                );

            Quaternion randomSpread =
                Quaternion.Euler(
                    Random.Range(
                        -gunData.randomSpreadX,
                        gunData.randomSpreadX
                    ),

                    Random.Range(
                        -gunData.randomSpreadY,
                        gunData.randomSpreadY
                    ),

                    Random.Range(
                        -gunData.randomSpreadZ,
                        gunData.randomSpreadZ
                    )
                );

            Quaternion finalRotation =
                firePoint.rotation *
                fixedSpread *
                randomSpread;

            GameObject bulletObj =
                Instantiate(
                    gunData.bulletPrefab,
                    firePoint.position,
                    finalRotation
                );

            if (bulletObj == null)
                continue;

            BulletProjectile bullet =
                bulletObj.GetComponent<BulletProjectile>();

            if (bullet == null)
            {
                Debug.LogWarning(
                    "PlayerBullet | bulletPrefab không có BulletProjectile."
                );

                continue;
            }

            bullet.Initialize(
                finalRotation * Vector3.forward,
                gunData.bulletSpeed,
                gunData.damage,
                gunData.rangeAttack,
                gunData.hitMask,
                gunData.interactionMask
            );
        }
    }

    // =====================================================
    // RAYCAST
    // =====================================================

    private void ShootRaycast(
        GunData gunData,
        Transform firePoint)
    {
        int pelletCount = Mathf.Max(1, gunData.pelletCount);

        float angleStep = gunData.angleBetweenBullets;

        float startAngle = -(angleStep * (pelletCount - 1)) / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle =
                startAngle +
                angleStep * i;

            Quaternion spread =
                Quaternion.Euler(
                    Random.Range(
                        -gunData.randomSpreadX,
                        gunData.randomSpreadX
                    ),

                    Random.Range(
                        -gunData.randomSpreadY,
                        gunData.randomSpreadY
                    ),

                    Random.Range(
                        -gunData.randomSpreadZ,
                        gunData.randomSpreadZ
                    )
                );

            Vector3 direction =
                (
                    firePoint.rotation *
                    Quaternion.Euler(
                        0f,
                        currentAngle,
                        0f
                    ) *
                    spread
                ) *
                Vector3.forward;

            Vector3 startPos =
                firePoint.position;

            Vector3 endPos;

            RaycastHit hit;

            if (Physics.SphereCast(
                startPos,
                gunData.hitRadius,
                direction,
                out hit,
                gunData.rangeAttack,
                gunData.hitMask,
                QueryTriggerInteraction.Ignore))
            {
                endPos = hit.point;

                IDamageable damageable =
                    hit.collider
                        .GetComponentInParent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(
                        gunData.damage
                    );

                    ProcessRaycastHit(
                        gunData,
                        hit.collider
                    );
                }
            }
            else
            {
                endPos =
                    startPos +
                    direction *
                    gunData.rangeAttack;
            }

            SpawnTrail(
                startPos,
                endPos,
                gunData
            );
        }
    }

    // =====================================================
    // RAYCAST HIT
    // =====================================================

    private void ProcessRaycastHit(
        GunData gunData,
        Collider hitCollider)
    {
        if (playerController == null) return;

        if ( (
                (1 << hitCollider.gameObject.layer)
                &
                gunData.interactionMask ) == 0)
        {
            return;
        }

        playerController.RecoverMana( gunData.manaRecoveryByHit
        );
    }

    // =====================================================
    // TRAIL
    // =====================================================

    private void SpawnTrail(
        Vector3 start,
        Vector3 end,
        GunData gunData)
    {
        if (gunData.bulletPrefab == null) return;

        GameObject trailObj = Instantiate(
                gunData.bulletPrefab,
                start,
                Quaternion.identity );

        if (trailObj == null) return;

        BulletRayTrail trail = trailObj.GetComponent<BulletRayTrail>();

        if (trail == null) return;

        trail.Initialize( start, end, gunData.bulletSpeed
        );
    }

    // =====================================================
    // PROJECTILE HIT -> MANA
    // =====================================================

    private void RecoverManaByProjectileHit()
    {
        if (playerWeapon == null) return;

        if (!playerWeapon.HasGun) return;

        GunData gunData = playerWeapon.CurrentGunData;

        if (gunData == null) return;

        playerController.RecoverMana( gunData.manaRecoveryByHit );
    }
}
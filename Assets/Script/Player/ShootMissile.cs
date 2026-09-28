using UnityEngine;

public class ShootMissile : MonoBehaviour
{
    private PlayerBullet playerBullet;

    // =====================================================
    // INITIALIZE
    // =====================================================

    public void Initialize(
        PlayerBullet owner)
    {
        playerBullet = owner;
    }

    // =====================================================
    // SHOOT
    // =====================================================

    public void Shoot(
        GunData gunData,
        Transform firePoint,
        Transform currentTarget)
    {
        if (gunData == null)
            return;

        if (firePoint == null)
            return;

        if (gunData.missilePrefab == null)
        {
            Debug.LogWarning(
                "ShootMissile | " +
                "GunData chưa có missilePrefab."
            );

            return;
        }

        int pelletCount =
            Mathf.Max(
                1,
                gunData.pelletCount
            );

        float angleStep =
            gunData.angleBetweenBullets;

        float startAngle =
            -(angleStep *
              (pelletCount - 1)) /
            2f;

        // =================================================
        // MISSILES
        // =================================================

        for (
            int i = 0;
            i < pelletCount;
            i++
        )
        {
            float currentAngle =
                startAngle +
                angleStep * i;

            // ---------------------------------------------
            // FIXED SPREAD
            // ---------------------------------------------

            Quaternion fixedSpread =
                Quaternion.Euler(
                    0f,
                    currentAngle,
                    0f
                );

            // ---------------------------------------------
            // RANDOM SPREAD
            // ---------------------------------------------

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

            // ---------------------------------------------
            // FINAL ROTATION
            // ---------------------------------------------

            Quaternion finalRotation =
                firePoint.rotation *
                fixedSpread *
                randomSpread;

            // ---------------------------------------------
            // CREATE MISSILE
            // ---------------------------------------------

            GameObject missileObj =
                Instantiate(
                    gunData.missilePrefab,
                    firePoint.position,
                    finalRotation
                );

            if (missileObj == null)
                continue;

            MissileProjectile missile =
                missileObj.GetComponent<
                    MissileProjectile
                >();

            if (missile == null)
            {
                Debug.LogWarning(
                    "ShootMissile | " +
                    "missilePrefab không có MissileProjectile."
                );

                Destroy(missileObj);

                continue;
            }

            // ---------------------------------------------
            // CRITICAL
            // ---------------------------------------------

            bool isCritical;

            float finalDamage =
                playerBullet != null
                    ? playerBullet.GetShotDamage(
                        gunData.damage,
                        out isCritical
                    )
                    : gunData.damage;

            // ---------------------------------------------
            // INITIALIZE MISSILE
            // ---------------------------------------------

            missile.Initialize(
                gunData.bulletSpeed,
                gunData.missileLifeTime,
                finalDamage,
                gunData.hitMask,
                gunData.interactionMask,
                currentTarget
            );
        }
    }
}
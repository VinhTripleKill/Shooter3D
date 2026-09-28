using UnityEngine;

public class ShootBullet : MonoBehaviour
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
        Transform firePoint)
    {
        if (gunData == null)
            return;

        if (firePoint == null)
            return;

        if (gunData.bulletPrefab == null)
        {
            Debug.LogWarning(
                "ShootBullet | GunData chưa có bulletPrefab."
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
        // PELLETS
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
            // CREATE BULLET
            // ---------------------------------------------

            GameObject bulletObj =
                Instantiate(
                    gunData.bulletPrefab,
                    firePoint.position,
                    finalRotation
                );

            if (bulletObj == null)
                continue;

            BulletProjectile bullet =
                bulletObj.GetComponent<
                    BulletProjectile
                >();

            if (bullet == null)
            {
                Debug.LogWarning(
                    "ShootBullet | " +
                    "bulletPrefab không có BulletProjectile."
                );

                Destroy(bulletObj);

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
            // INITIALIZE
            // ---------------------------------------------

            Vector3 direction =
                finalRotation *
                Vector3.forward;

            bullet.Initialize(
                direction,
                gunData.bulletSpeed,
                finalDamage,
                gunData.rangeAttack,
                gunData.hitMask,
                gunData.interactionMask
            );
        }
    }
}
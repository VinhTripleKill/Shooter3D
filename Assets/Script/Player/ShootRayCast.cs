using UnityEngine;

public class ShootRayCast : MonoBehaviour
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
            // DIRECTION
            // ---------------------------------------------

            Vector3 direction =
                (
                    firePoint.rotation *
                    Quaternion.Euler(
                        0f,
                        currentAngle,
                        0f
                    ) *
                    randomSpread
                ) *
                Vector3.forward;

            Vector3 startPos =
                firePoint.position;

            Vector3 endPos;

            // =================================================
            // SPHERE CAST
            // =================================================

            if (
                Physics.SphereCast(
                    startPos,
                    gunData.hitRadius,
                    direction,
                    out RaycastHit hit,
                    gunData.rangeAttack,
                    gunData.hitMask,
                    QueryTriggerInteraction.Ignore
                )
            )
            {
                endPos =
                    hit.point;

                // ---------------------------------------------
                // DAMAGE
                // ---------------------------------------------

                IDamageable damageable =
                    hit.collider
                        .GetComponentInParent<
                            IDamageable
                        >();

                if (damageable != null)
                {
                    bool isCritical;

                    float finalDamage =
                        playerBullet != null
                            ? playerBullet.GetShotDamage(
                                gunData.damage,
                                out isCritical
                            )
                            : gunData.damage;

                    damageable.TakeDamage(
                        finalDamage
                    );

                    // -----------------------------------------
                    // MANA
                    // -----------------------------------------

                    if (playerBullet != null)
                    {
                        playerBullet
                            .RecoverManaByRaycastHit(
                                gunData,
                                hit.collider
                            );
                    }
                }
            }
            else
            {
                endPos =
                    startPos +
                    direction *
                    gunData.rangeAttack;
            }

            // =================================================
            // TRAIL
            // =================================================

            SpawnTrail(
                startPos,
                endPos,
                gunData
            );
        }
    }

    // =====================================================
    // TRAIL
    // =====================================================

    private void SpawnTrail(
        Vector3 start,
        Vector3 end,
        GunData gunData)
    {
        if (gunData == null)
            return;

        if (gunData.bulletPrefab == null)
            return;

        GameObject trailObj =
            Instantiate(
                gunData.bulletPrefab,
                start,
                Quaternion.identity
            );

        if (trailObj == null)
            return;

        BulletRayTrail trail =
            trailObj.GetComponent<
                BulletRayTrail
            >();

        if (trail == null)
        {
            Destroy(trailObj);

            return;
        }

        trail.Initialize(
            start,
            end,
            gunData.bulletSpeed
        );
    }
}
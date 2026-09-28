using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // =====================================================
    // REFERENCES
    // =====================================================

    private PlayerController playerController;
    private PlayerWeapon playerWeapon;

    // =====================================================
    // SHOOT MODULES
    // =====================================================

    private ShootBullet shootBullet;
    private ShootRayCast shootRayCast;
    private ShootMissile shootMissile;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        playerController =
            GetComponent<PlayerController>();

        playerWeapon =
            GetComponent<PlayerWeapon>();

        // -------------------------------------------------
        // LẤY CÁC MODULE BẮN
        // -------------------------------------------------

        shootBullet =
            GetComponent<ShootBullet>();

        shootRayCast =
            GetComponent<ShootRayCast>();

        shootMissile =
            GetComponent<ShootMissile>();

        // -------------------------------------------------
        // TỰ ADD NẾU PLAYER PREFAB CHƯA CÓ
        // -------------------------------------------------

        if (shootBullet == null)
        {
            shootBullet =
                gameObject.AddComponent<ShootBullet>();
        }

        if (shootRayCast == null)
        {
            shootRayCast =
                gameObject.AddComponent<ShootRayCast>();
        }

        if (shootMissile == null)
        {
            shootMissile =
                gameObject.AddComponent<ShootMissile>();
        }

        // -------------------------------------------------
        // GÁN REFERENCES
        // -------------------------------------------------

        shootBullet.Initialize(
            this
        );

        shootRayCast.Initialize(
            this
        );

        shootMissile.Initialize(
            this
        );

        // -------------------------------------------------
        // PROJECTILE HIT -> MANA
        // -------------------------------------------------

        BulletProjectile.OnSuccessfulHit +=
            RecoverManaByProjectileHit;

        MissileProjectile.OnSuccessfulHit +=
            RecoverManaByProjectileHit;
    }

    // =====================================================
    // DESTROY
    // =====================================================

    private void OnDestroy()
    {
        BulletProjectile.OnSuccessfulHit -=
            RecoverManaByProjectileHit;

        MissileProjectile.OnSuccessfulHit -=
            RecoverManaByProjectileHit;
    }

    // =====================================================
    // CRITICAL HIT
    // =====================================================

    public bool RollCriticalHit()
    {
        if (playerWeapon == null)
            return false;

        float critRate =
            playerWeapon.CritRate;

        critRate =
            Mathf.Clamp(
                critRate,
                0f,
                100f
            );

        float randomValue =
            Random.Range(
                0f,
                100f
            );

        return randomValue < critRate;
    }

    // =====================================================
    // FINAL DAMAGE
    // =====================================================

    public float CalculateFinalDamage(
        float baseDamage,
        bool isCritical)
    {
        if (!isCritical)
            return baseDamage;

        float critDamage =
            playerWeapon != null
                ? playerWeapon.CritDamage
                : 100f;

        critDamage =
            Mathf.Max(
                0f,
                critDamage
            );

        return baseDamage *
               (critDamage / 100f);
    }

    // =====================================================
    // SHOT DAMAGE
    // =====================================================

    public float GetShotDamage(
        float baseDamage,
        out bool isCritical)
    {
        isCritical =
            RollCriticalHit();

        float finalDamage =
            CalculateFinalDamage(
                baseDamage,
                isCritical
            );

        if (isCritical)
        {
            Debug.Log(
                $"PLAYER CRITICAL HIT | " +
                $"Base: {baseDamage} | " +
                $"CritMultiplier: {playerWeapon.CritDamage} | " +
                $"Final: {finalDamage}"
            );
        }

        return finalDamage;
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
                "PlayerBullet | GunData null."
            );

            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning(
                "PlayerBullet | FirePoint null."
            );

            return;
        }

        // =================================================
        // CHỌN MODULE THEO FIRE TYPE
        // =================================================

        switch (gunData.fireType)
        {
            case GunData.GunFireType.Bullet:

                if (shootBullet != null)
                {
                    shootBullet.Shoot(
                        gunData,
                        firePoint
                    );
                }

                break;

            case GunData.GunFireType.Raycast:

                if (shootRayCast != null)
                {
                    shootRayCast.Shoot(
                        gunData,
                        firePoint
                    );
                }

                break;

            case GunData.GunFireType.Missile:

                if (shootMissile != null)
                {
                    shootMissile.Shoot(
                        gunData,
                        firePoint,
                        currentTarget
                    );
                }

                break;

            default:

                Debug.LogWarning(
                    $"PlayerBullet | FireType chưa được xử lý: " +
                    $"{gunData.fireType}"
                );

                break;
        }
    }

    // =====================================================
    // RAYCAST MANA
    // =====================================================

    public void RecoverManaByRaycastHit(
        GunData gunData,
        Collider hitCollider)
    {
        if (playerController == null)
            return;

        if (gunData == null)
            return;

        if (hitCollider == null)
            return;

        if (
            (
                (1 << hitCollider.gameObject.layer)
                &
                gunData.interactionMask
            ) == 0
        )
        {
            return;
        }

        playerController.RecoverMana(
            gunData.manaRecoveryByHit
        );
    }

    // =====================================================
    // PROJECTILE HIT -> MANA
    // =====================================================

    private void RecoverManaByProjectileHit()
    {
        if (playerWeapon == null)
            return;

        if (!playerWeapon.HasGun)
            return;

        GunData gunData =
            playerWeapon.CurrentGunData;

        if (gunData == null)
            return;

        if (playerController == null)
            return;

        playerController.RecoverMana(
            gunData.manaRecoveryByHit
        );
    }
}
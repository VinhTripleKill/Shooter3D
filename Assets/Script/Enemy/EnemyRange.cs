using UnityEngine;

public class EnemyRange : EnemyAttack
{
    [Header("Range Attack")]
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    [SerializeField] private float bulletRange = 5f;
    [SerializeField] private float speed = 10f;

    [Header("Bullet")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleBullet;

    // =========================================================
    // INITIALIZE
    // =========================================================

    public override void Initialize(
        EnemyBehaviour behaviour,
        EnemyAnim enemyAnim)
    {
        base.Initialize(
            behaviour,
            enemyAnim
        );

        if (this.enemyAnim != null)
        {
            this.enemyAnim.OnEnemyShoot -=
                EnemyShoot;

            this.enemyAnim.OnEnemyShoot +=
                EnemyShoot;
        }
    }

    // =========================================================
    // ATTACK
    // =========================================================

    public override void UpdateAttack()
    {
        if (player == null)
            return;

        base.UpdateAttack();
    }

    // =========================================================
    // START ATTACK
    // =========================================================

    protected override void StartAttack()
    {
        nextAttackTime =
            Time.time + atkCD;

        if (enemyAnim != null)
        {
            enemyAnim.PlayAttack();
        }
    }

    // =========================================================
    // ANIMATION EVENT
    // =========================================================

    private void EnemyShoot()
    {
        if (player == null)
            return;

        if (bulletPrefab == null)
        {
            Debug.LogWarning(
                $"[{name}] EnemyBullet Prefab chưa được gán!"
            );

            return;
        }

        if (bulletSpawnPoint == null)
        {
            Debug.LogWarning(
                $"[{name}] Bullet Spawn Point chưa được gán!"
            );

            return;
        }

        // =====================================================
        // LẤY HƯỚNG TỚI PLAYER
        // =====================================================

        Vector3 direction =
            (
                player.position -
                bulletSpawnPoint.position
            ).normalized;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        // =====================================================
        // TẠO BULLET
        // =====================================================

        EnemyBullet bullet =
            Instantiate(
                bulletPrefab,
                bulletSpawnPoint.position,
                Quaternion.LookRotation(direction)
            );

        // =====================================================
        // INITIALIZE BULLET
        // =====================================================

        bullet.Initialize(
            direction,
            speed,
            atkDamage,
            bulletRange,
            playerLayer,
            obstacleBullet
        );
    }

    // =========================================================
    // STOP
    // =========================================================

    public override void StopAttack()
    {
    }

    // =========================================================
    // DISABLE
    // =========================================================

    protected override void OnDisable()
    {
        if (enemyAnim != null)
        {
            enemyAnim.OnEnemyShoot -=
                EnemyShoot;
        }

        base.OnDisable();
    }
}
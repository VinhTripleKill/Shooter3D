
using UnityEngine;

public class EnemyBomber : EnemyAttack
{
    [Header("Explosion")]
    [SerializeField] private float explosionRange = 5f;
    [SerializeField] private float explosionDelay = 1f;
    [SerializeField] private float explosionCommit = 0.8f;
    private EnemyExplosionVisual explosionVisual;
    // Không cần SerializeField
    private float explosionTimer;

    private bool isCountingExplosion;
    private bool isExplosionCommitted;
    private bool hasExploded;

    private BaseEnemy baseEnemy;

    public float ExplosionRange => explosionRange;

    public float ExplosionTimer => explosionTimer;

    public bool IsExplosionCommitted =>
        isExplosionCommitted;

    // =========================================================
    // KEEP ATTACK STATE
    // =========================================================

    public override bool ShouldKeepAttackState
    {
        get
        {
            return isExplosionCommitted;
        }
    }

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

    baseEnemy = GetComponent<BaseEnemy>();

    explosionVisual =
        GetComponentInChildren<EnemyExplosionVisual>();

    if (explosionVisual != null)
    {
        explosionVisual.Initialize(
            explosionRange,
            explosionDelay
        );
    }

    if (this.enemyAnim != null)
    {
        this.enemyAnim.OnExplosionHit -= ExplosionHitEvent;
        this.enemyAnim.OnExplosionHit += ExplosionHitEvent;
    }
}

    // =========================================================
    // ATTACK UPDATE
    // =========================================================

    public override void UpdateAttack()
    {
        if (player == null)
            return;

        if (hasExploded)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // =====================================================
        // CHƯA BẮT ĐẦU COUNTDOWN
        // =====================================================

        if (!isCountingExplosion)
        {
            if (distance <= AttackRange)
            {
                StartExplosionCountdown();
            }

            return;
        }

        // =====================================================
        // ĐANG COUNTDOWN
        // =====================================================

        explosionTimer += Time.deltaTime;
if (explosionVisual != null)
{
    explosionVisual.UpdateCountdown(
        explosionTimer
    );
}
        // =====================================================
        // TRƯỜNG HỢP COMMIT NHỎ HƠN DELAY
        // =====================================================
        //
        // Ví dụ:
        // Commit = 0.8
        // Delay  = 1.0
        //
        // Đạt 0.8 => COMMIT
        // Từ đây Player chạy đi cũng không cancel.
        //
        // =====================================================

        if (!isExplosionCommitted &&
            explosionCommit < explosionDelay)
        {
            if (explosionTimer >= explosionCommit)
            {
                isExplosionCommitted = true;
            }
        }

        // =====================================================
        // CHƯA COMMIT
        // =====================================================

        if (!isExplosionCommitted)
        {
            // -------------------------------------------------
            // CHƯA ĐẾN EXPLOSION DELAY
            // -------------------------------------------------

            if (explosionTimer <= explosionDelay)
            {
                // Player chạy ra ngoài AttackRange
                // => Cancel và quay lại Chase.
                if (distance > AttackRange)
                {
                    CancelExplosion();
                }

                return;
            }

            // -------------------------------------------------
            // ĐÃ VƯỢT EXPLOSION DELAY
            // -------------------------------------------------
            //
            // Trường hợp:
            //
            // explosionCommit >= explosionDelay
            //
            // Bomber KHÔNG được tự động nổ chỉ vì timer đủ.
            //
            // Player bắt buộc phải còn trong AttackRange.
            //
            // -------------------------------------------------

            if (distance <= AttackRange)
            {
                Explode();
            }
            else
            {
                CancelExplosion();
            }

            return;
        }

        // =====================================================
        // ĐÃ COMMIT
        // =====================================================
        //
        // Player có chạy ra ngoài AttackRange
        // cũng KHÔNG được cancel.
        //
        // =====================================================

        if (explosionTimer > explosionDelay)
        {
            Explode();
        }
    }

    // =========================================================
    // START COUNTDOWN
    // =========================================================

private void StartExplosionCountdown()
{
    isCountingExplosion = true;

    explosionTimer = 0f;
    isExplosionCommitted = false;

    // =====================================================
    // START EXPLOSION VISUAL
    // =====================================================

    if (explosionVisual != null)
    {
        explosionVisual.StartCountdown();
    }
}

    // =========================================================
    // CANCEL
    // =========================================================

private void CancelExplosion()
{
    isCountingExplosion = false;

    explosionTimer = 0f;

    isExplosionCommitted = false;

    if (explosionVisual != null)
    {
        explosionVisual.Hide();
    }
}

  private void Explode()
{
    if (hasExploded)
        return;

    if (baseEnemy != null &&
        baseEnemy.IsDead())
    {
        return;
    }

    hasExploded = true;

    isCountingExplosion = false;
    isExplosionCommitted = true;


    if (explosionVisual != null)
    {
        explosionVisual.Hide();
    }

    if (baseEnemy != null)
    {
        baseEnemy.SelfDestruct();
    }
    else if (enemyAnim != null)
    {
        enemyAnim.PlayDead(null);
    }
}
    public void ExplosionHitEvent()
    {
        if (!hasExploded) return;

        PerformExplosionDamage();
    }

    private void PerformExplosionDamage()
    {
        if (player == null) return;

        float distance = Vector3.Distance( transform.position, player.position );

        if (distance > explosionRange) return;

        IDamageable damageable = player.GetComponentInChildren<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(atkDamage);
        }
    }

    public override void StopAttack()
    {
        if (isExplosionCommitted)
            return;

        CancelExplosion();
    }


    protected override void OnDisable()
    {
        if (enemyAnim != null)
        {
            enemyAnim.OnExplosionHit -= ExplosionHitEvent;
        }

        base.OnDisable();
    }
}


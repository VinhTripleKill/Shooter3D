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

    public float ExplosionRange =>
        explosionRange;

    public float ExplosionTimer =>
        explosionTimer;

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

        baseEnemy =
            GetComponent<BaseEnemy>();

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
            this.enemyAnim.OnExplosionHit -=
                ExplosionHitEvent;

            this.enemyAnim.OnExplosionHit +=
                ExplosionHitEvent;
        }
    }


    private void Update()
    {
        if (baseEnemy == null)
            return;

        if (baseEnemy.IsDead())
        {
            HideExplosionVisual();
        }
    }


    public override void UpdateAttack()
    {
        if (player == null)
            return;

        if (hasExploded)
            return;


        UpdateFacePlayer();

        float distance =
            Vector3.Distance(
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

        explosionTimer +=
            Time.deltaTime;

        if (explosionVisual != null)
        {
            explosionVisual.UpdateCountdown(
                explosionTimer
            );
        }

        // =====================================================
        // TRƯỜNG HỢP COMMIT < DELAY
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
            // CHƯA ĐẾN DELAY
            // -------------------------------------------------

            if (explosionTimer <= explosionDelay)
            {
                // Player chạy ra ngoài AttackRange
                // => Cancel countdown
                if (distance > AttackRange)
                {
                    CancelExplosion();
                }

                return;
            }

            // -------------------------------------------------
            // COMMIT >= DELAY
            // -------------------------------------------------
            //
            // Chỉ nổ nếu Player vẫn còn trong AttackRange.
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
        // START VISUAL
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

        // =====================================================
        // HIDE VISUAL
        // =====================================================

        HideExplosionVisual();
    }

    // =========================================================
    // EXPLODE
    // =========================================================

    private void Explode()
    {
        if (hasExploded)
            return;

        if (baseEnemy != null &&
            baseEnemy.IsDead())
        {
            HideExplosionVisual();
            return;
        }

        hasExploded = true;

        isCountingExplosion = false;

        isExplosionCommitted = true;

        // =====================================================
        // HIDE VISUAL
        // =====================================================

        HideExplosionVisual();

        // =====================================================
        // SELF DESTRUCT
        // =====================================================

        if (baseEnemy != null)
        {
            baseEnemy.SelfDestruct();
        }
        else if (enemyAnim != null)
        {
            enemyAnim.PlayDead(null);
        }
    }

    // =========================================================
    // HIDE EXPLOSION VISUAL
    // =========================================================

    private void HideExplosionVisual()
    {
        if (explosionVisual != null)
        {
            explosionVisual.Hide();
        }
    }

    // =========================================================
    // EXPLOSION ANIMATION EVENT
    // =========================================================

    public void ExplosionHitEvent()
    {
        if (!hasExploded)
            return;

        PerformExplosionDamage();
    }

    // =========================================================
    // EXPLOSION DAMAGE
    // =========================================================

    private void PerformExplosionDamage()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance > explosionRange)
            return;

        IDamageable damageable =
            player.GetComponentInChildren<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(
                atkDamage
            );
        }
    }

    // =========================================================
    // STOP ATTACK
    // =========================================================

    public override void StopAttack()
    {
        // =====================================================
        // ĐÃ COMMIT
        // =====================================================
        //
        // Không được Cancel.
        //
        // =====================================================

        if (isExplosionCommitted)
            return;

        CancelExplosion();
    }

    // =========================================================
    // DISABLE
    // =========================================================

    protected override void OnDisable()
    {
        // =====================================================
        // ĐẢM BẢO VISUAL TẮT
        // =====================================================

        HideExplosionVisual();

        if (enemyAnim != null)
        {
            enemyAnim.OnExplosionHit -=
                ExplosionHitEvent;
        }

        base.OnDisable();
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        // =====================================================
        // EXPLOSION RANGE
        // =====================================================

        Gizmos.color =
            new Color(
                1f,
                0.15f,
                0f,
                0.35f
            );

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRange
        );

        // =====================================================
        // CENTER
        // =====================================================

        Gizmos.color =
            new Color(
                1f,
                0.5f,
                0f,
                0.8f
            );

        Gizmos.DrawWireSphere(
            transform.position,
            0.15f
        );
    }
}
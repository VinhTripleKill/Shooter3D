using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] protected float atkRange = 4f;
    [SerializeField] protected float atkDamage = 10f;
    [SerializeField] protected float atkCD = 2f;

    protected EnemyBehaviour behaviour;
    protected EnemyAnim enemyAnim;

    protected Transform player;

    protected float nextAttackTime;

    public float AttackRange => atkRange;
    public float AttackDamage => atkDamage;
    public float AttackCooldown => atkCD;

    public bool CanAttack =>
        Time.time >= nextAttackTime;

    // =========================================================
    // KEEP ATTACK STATE
    // =========================================================
    //
    // Mặc định:
    // Player ra ngoài AttackRange
    // => EnemyBehaviour chuyển sang Chase.
    //
    // Bomber có thể override:
    // Sau khi đã commit nổ
    // => dù Player chạy ra ngoài range
    // => vẫn giữ Attack State.
    //
    // =========================================================

    public virtual bool ShouldKeepAttackState => false;

    // =========================================================
    // INITIALIZE
    // =========================================================

    public virtual void Initialize(
        EnemyBehaviour behaviour,
        EnemyAnim enemyAnim)
    {
        this.behaviour = behaviour;
        this.enemyAnim = enemyAnim;
    }

    public virtual void SetPlayer(
        Transform player)
    {
        this.player = player;
    }

    // =========================================================
    // UPDATE ATTACK
    // =========================================================

    public virtual void UpdateAttack()
    {
        if (player == null)
            return;

        if (!CanAttack)
            return;

        StartAttack();
    }

    // =========================================================
    // START ATTACK
    // =========================================================

    protected virtual void StartAttack()
    {
        nextAttackTime =
            Time.time + atkCD;

        if (enemyAnim != null)
        {
            enemyAnim.PlayAttack();
        }
    }

    // =========================================================
    // STOP
    // =========================================================

    public virtual void StopAttack()
    {
    }

    protected virtual void OnDisable()
    {
        StopAttack();
    }
}
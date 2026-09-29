using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public enum TypeFace
    {
        FaceFollow,
        NoFaceFollow
    }

    [Header("Attack Settings")]
    [SerializeField] protected float atkRange = 4f;
    [SerializeField] protected float atkDamage = 10f;
    [SerializeField] protected float atkCD = 2f;

    [Header("Face Settings")]
    [SerializeField] protected TypeFace typeFace =
        TypeFace.NoFaceFollow;

    protected EnemyBehaviour behaviour;
    protected EnemyAnim enemyAnim;

    protected Transform player;

    protected float nextAttackTime;

    public float AttackDamage => atkDamage;
    public float AttackCooldown => atkCD;

    public TypeFace FaceType => typeFace;

    public bool CanAttack =>
        Time.time >= nextAttackTime;
public float AttackRange => atkRange;

public void SetAttackRange(float newRange)
{
    atkRange = Mathf.Max(0f, newRange);
}
public void SetAttackDamage(float value)
{
    atkDamage = Mathf.Max(0f, value);
}

public void SetAttackCooldown(float value)
{
    atkCD = Mathf.Max(0f, value);
}
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

    // =========================================================
    // SET PLAYER
    // =========================================================

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

        // =====================================================
        // FACE PLAYER
        // =====================================================

        UpdateFacePlayer();

        // =====================================================
        // ATTACK COOLDOWN
        // =====================================================

        if (!CanAttack)
            return;

        StartAttack();
    }

    // =========================================================
    // FACE PLAYER
    // =========================================================
    //
    // Nếu:
    //
    // FaceFollow
    //      => luôn quay mặt về Player
    //
    // NoFaceFollow
    //      => không tự xoay
    //
    // =========================================================

    protected virtual void UpdateFacePlayer()
    {
        if (typeFace != TypeFace.FaceFollow)
            return;

        if (player == null)
            return;

        FacePlayer();
    }

    // =========================================================
    // FACE PLAYER
    // =========================================================

    protected void FacePlayer()
    {
        Vector3 lookPosition =
            new Vector3(
                player.position.x,
                transform.position.y,
                player.position.z
            );

        Vector3 direction =
            lookPosition -
            transform.position;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        transform.rotation =
            Quaternion.LookRotation(direction);
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

    // =========================================================
    // DISABLE
    // =========================================================

    protected virtual void OnDisable()
    {
        StopAttack();
    }
}
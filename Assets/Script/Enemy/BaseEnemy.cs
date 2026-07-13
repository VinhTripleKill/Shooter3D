using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy : BaseCharacter, IAutoAimTarget
{
    public enum EnemyState { Idle, Chase, Attack, Dead }

    [Header("Detect & Attack")]
    [SerializeField] protected float detectRange = 15f;
    [SerializeField] protected float atkRange = 4f;           // Tầm xa tấn công
    [SerializeField] protected float atkDamage = 10f;
    [SerializeField] protected float atkCD = 2f;
    [SerializeField] protected float atkTimeAnim = 1.2f;
    public float attackHeightOffset;
    [Header("Cone Attack")]
    [SerializeField, Range(0f, 360f)] protected float attackAngle = 90f;  // Góc mở hình nón
    [Header("Gizmos")]
    [SerializeField] private Color gizmoColor = new Color(1f, 0.2f, 0f, 0.35f);

    protected EnemyState currentState;
    protected Transform player;
    protected bool isAttacking;

    [Header("Reward")]
    [SerializeField] protected int expReward = 200;

    protected EnemyAnim enemyAnim;
    protected NavMeshAgent agent;
    private Collider[] enemyColliders;

    public static List<BaseEnemy> AllEnemies { get; } = new List<BaseEnemy>();

    protected override void Awake()
    {
        base.Awake();
        enemyColliders = GetComponentsInChildren<Collider>();
        enemyAnim = GetComponentInChildren<EnemyAnim>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() => FindPlayer();

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
{
    if (isDead || player == null)
    {
        if (player == null) FindPlayer();
        return;
    }

    if (player.GetComponent<BaseCharacter>().IsDead())
    {
        currentState = EnemyState.Idle;
        enemyAnim.SetSpeed(0);
        return;
    }

    float distance = Vector3.Distance(transform.position, player.position);

    switch (currentState)
    {
        case EnemyState.Idle:
            enemyAnim.SetSpeed(0);
            if (distance <= detectRange) currentState = EnemyState.Chase;
            break;

        case EnemyState.Chase:
            UpdateChase(distance);
            break;

        case EnemyState.Attack:
            UpdateAttack(distance);
            break;
    }
}

    protected virtual void UpdateChase(float distance)
{
    if (distance > detectRange)
    {
        currentState = EnemyState.Idle;
        enemyAnim.SetSpeed(0);           // ← Thêm
        return;
    }

    if (distance <= atkRange)
    {
        currentState = EnemyState.Attack;
        enemyAnim.SetSpeed(0);           // ← Thêm
        return;
    }

    if (agent.isOnNavMesh)
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        float speed = agent.velocity.magnitude;
        enemyAnim.SetSpeed(1);
    }
    else
    {
        enemyAnim.SetSpeed(0);
    }
}

protected virtual void UpdateAttack(float distance)
{
    if (distance > atkRange)
    {
        currentState = EnemyState.Chase;
        return;
    }

    if (agent.isOnNavMesh)
    {
        agent.ResetPath();
        agent.isStopped = true;
    }

    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

    enemyAnim.SetSpeed(0);   // Đứng yên khi Attack

    if (!isAttacking)
    {
        StartCoroutine(AttackRoutine());
    }
}

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        enemyAnim.PlayAttack();

        yield return new WaitForSeconds(atkTimeAnim * 0.6f);

        PerformConeAttack();   // Tấn công theo hình nón

        yield return new WaitForSeconds(atkTimeAnim * 0.4f + atkCD);
        isAttacking = false;
    }

    // ====================== CONE ATTACK ======================
    protected virtual void PerformConeAttack()
{
    if (player == null) return;

    Vector3 origin = transform.position + Vector3.up * 1f;

    // Bỏ ảnh hưởng của chiều cao, chỉ xét mặt phẳng XZ
    Vector3 toPlayer = player.position - origin;
    Vector3 flatToPlayer = Vector3.ProjectOnPlane(toPlayer, Vector3.up);
    Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

    float angle = Vector3.Angle(flatForward, flatToPlayer);

    // Chỉ cần trong góc cone + trong tầm đánh
    if (flatToPlayer.magnitude <= atkRange && angle <= attackAngle * 0.5f)
    {
        IDamageable damageable = player.GetComponentInChildren<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(atkDamage);
            Debug.Log($"[ENEMY] Cone Attack gây {atkDamage} damage!");
        }
    }

    enemyAnim.AttackHitEvent();
}
    // ====================== VẼ GIZMOS HÌNH NÓN ======================
    protected virtual void OnDrawGizmosSelected()
    {
        if (!enabled) return;

        Vector3 origin = transform.position + Vector3.up * attackHeightOffset;

        // Vẽ Detect Range
        Gizmos.color = Color.green * 0.3f;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Vẽ Cone Attack
        Gizmos.color = gizmoColor;
        DrawConeGizmo(origin, transform.forward, atkRange, attackAngle);

        // Vẽ Attack Radius
        Gizmos.color = Color.red * 0.6f;
        
    }

    private void DrawConeGizmo(Vector3 origin, Vector3 direction, float range, float angle)
    {
        int segments = 12;
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Quaternion rot = Quaternion.Euler(0, currentAngle * Mathf.Rad2Deg, 0);
            Vector3 dir = rot * direction;

            Gizmos.DrawRay(origin, dir * range);
        }

        // Vẽ cung tròn
        for (int i = 0; i < segments; i++)
        {
            float a1 = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
            float a2 = Mathf.Lerp(-halfAngle, halfAngle, (float)(i + 1) / segments);

            Quaternion r1 = Quaternion.Euler(0, a1 * Mathf.Rad2Deg, 0);
            Quaternion r2 = Quaternion.Euler(0, a2 * Mathf.Rad2Deg, 0);

            Vector3 p1 = origin + r1 * direction * range;
            Vector3 p2 = origin + r2 * direction * range;

            Gizmos.DrawLine(p1, p2);
        }
    }

    protected virtual void DisableCollision()
    {
        foreach (Collider col in enemyColliders) col.enabled = false;
    }

    protected virtual void OnEnable()
    {
        AllEnemies.Add(this);
        AutoAimManager.Register(this);
    }

    protected virtual void OnDisable()
    {
        AllEnemies.Remove(this);
        AutoAimManager.Unregister(this);
    }

    public virtual Transform GetTargetTransform() => transform;

    protected override void Die()
{
    base.Die();
    StopAllCoroutines();
    isAttacking = false;
    currentState = EnemyState.Dead;
    
    DisableCollision();
    if (agent != null)
    {
        agent.isStopped = true;
        agent.enabled = false;
    }

    if (player != null)
    {
        PlayerProgress pp = player.GetComponent<PlayerProgress>();
        pp?.AddExp(expReward);
    }

    // === THÔNG BÁO CHO WAVE UI ===
    CoreGameUI coreUI = FindObjectOfType<CoreGameUI>();
    coreUI?.OnEnemyDied();

    AllEnemies.Remove(this);
    AutoAimManager.Unregister(this);

    enemyAnim.PlayDead(() => Destroy(gameObject));
}
}
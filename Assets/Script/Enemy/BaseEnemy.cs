using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy :
    BaseCharacter, IAutoAimTarget
{

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Dead
    }
    [Header("Detect")]
    [SerializeField]
    protected float detectRange = 15f;

    [SerializeField]
    protected float atkRange = 1f;

    [SerializeField]
    protected float atkDamage = 10f;

    [SerializeField]
    protected float atkCD = 2f;
    [SerializeField]
    protected float atkTimeAnim= 2f;


    protected float nextAttackTime;

    protected EnemyState currentState;

    protected Transform player;
    bool isAttacking;

    protected EnemyAnim enemyAnim;
    protected NavMeshAgent agent;

    public static List<BaseEnemy> AllEnemies { get; }
        = new List<BaseEnemy>();

    protected override void Awake()
    {
        base.Awake();

        enemyAnim = GetComponentInChildren<EnemyAnim>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        enemyAnim.OnAttackHit += DealDamage;
    }
    protected virtual void Update()
    {
        if (isDead)
            return;

        if (player == null)
            return;

        BaseCharacter playerCharacter =
            player.GetComponent<BaseCharacter>();

        if (playerCharacter == null || playerCharacter.IsDead())
        {
            currentState = EnemyState.Idle;

            if (agent.isOnNavMesh)
                agent.ResetPath();

            enemyAnim.SetSpeed(0);

            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                player.position);

        switch (currentState)
        {
            case EnemyState.Idle:

                enemyAnim.SetSpeed(0);

                if (distance <= detectRange)
                    currentState = EnemyState.Chase;

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

            if (agent.isOnNavMesh)
                agent.ResetPath();

            enemyAnim.SetSpeed(0);

            return;
        }

        if (distance <= atkRange)
        {
            currentState = EnemyState.Attack;
            return;
        }

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            enemyAnim.SetSpeed(agent.velocity.magnitude);
        }
    }

    protected virtual void UpdateAttack(float distance)
    {
        // player chạy ra khỏi vùng attack
        if (distance > atkRange)
        {
            currentState = EnemyState.Chase;

            return;
        }

        // đứng yên khi đánh
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        enemyAnim.SetSpeed(0);


        // xoay mặt nhìn player
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;

        transform.LookAt(lookPos);

        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        enemyAnim.PlayAttack();

        // đợi animation attack chạy xong
        yield return new WaitForSeconds(atkTimeAnim);

        // đợi thêm cooldown
        yield return new WaitForSeconds(atkCD);

        isAttacking = false;
    }
    protected virtual void DealDamage()
    {
        if (player == null)
            return;

        if (Vector3.Distance(
            transform.position,
            player.position) > atkRange)
            return;

        IDamageable damageable =
            player.GetComponent<IDamageable>();

        damageable?.TakeDamage(
            atkDamage);
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


    public virtual Transform GetTargetTransform()
    {
        return transform;
    }

    protected override void Die()
    {
        base.Die();

        StopAllCoroutines();

        isAttacking = false;

        currentState = EnemyState.Dead;

        enemyAnim.OnAttackHit -= DealDamage;

        AutoAimManager.Unregister(this);
        AllEnemies.Remove(this);

        if (agent != null)
        {
            if (agent.isOnNavMesh)
                agent.ResetPath();

            agent.isStopped = true;
            agent.enabled = false;
        }

        enemyAnim.SetSpeed(0);

        enemyAnim.PlayDead(() =>
        {
            Destroy(gameObject);
        });
    }
}
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public enum BehaviourState
    {
        Idle,
        Chase,
        Attack
    }

    private BehaviourState currentState = BehaviourState.Idle;

    private Transform player;

    private BaseEnemy baseEnemy;
    private EnemyAnim enemyAnim;
    private NavMeshAgent agent;

    private EnemyIdleAndChase idleAndChase;
    private EnemyAttack enemyAttack;
    public BehaviourState CurrentState => currentState;

    public float DetectRange => idleAndChase.DetectRange;
    public float AttackRange => enemyAttack.AttackRange;
    public float AttackDamage => enemyAttack.AttackDamage;
    

    private void Awake()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        agent = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponentInChildren<EnemyAnim>();

        idleAndChase = GetComponent<EnemyIdleAndChase>();
        enemyAttack = GetComponent<EnemyAttack>();

        if (idleAndChase == null)
        {
            idleAndChase = gameObject.AddComponent<EnemyIdleAndChase>();
        }

        if (enemyAttack == null)
        {
            enemyAttack = gameObject.AddComponent<EnemyAttack>();
        }

        idleAndChase.Initialize(
            this,
            agent,
            enemyAnim
        );

        enemyAttack.Initialize(
            this,
            enemyAnim
        );
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        // =====================================================
        // ENEMY DEAD
        // =====================================================

        if (baseEnemy == null || baseEnemy.IsDead())
        {
            StopBehaviour();
            return;
        }

        // =====================================================
        // FIND PLAYER
        // =====================================================

        if (player == null)
        {
            FindPlayer();
            return;
        }

        // =====================================================
        // PLAYER DEAD
        // =====================================================

        BaseCharacter playerCharacter =
            player.GetComponent<BaseCharacter>();

        if (playerCharacter != null &&
            playerCharacter.IsDead())
        {
            ChangeState(BehaviourState.Idle);

            idleAndChase.StopMovement();
            idleAndChase.SetAnimationSpeed(0f);

            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // =====================================================
        // STATE MACHINE / BEHAVIOUR DISPATCH
        // =====================================================

        switch (currentState)
        {
            case BehaviourState.Idle:

                UpdateIdle(distance);

                break;

            case BehaviourState.Chase:

                UpdateChase(distance);

                break;

            case BehaviourState.Attack:

                UpdateAttack(distance);

                break;
        }
    }

    // =========================================================
    // PLAYER
    // =========================================================

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            idleAndChase.SetPlayer(player);
            enemyAttack.SetPlayer(player);
        }
    }

    // =========================================================
    // IDLE
    // =========================================================

    private void UpdateIdle(float distance)
    {
        idleAndChase.UpdateIdle(distance);

        if (distance <= idleAndChase.DetectRange)
        {
            ChangeState(BehaviourState.Chase);
        }
    }

    // =========================================================
    // CHASE
    // =========================================================

    private void UpdateChase(float distance)
    {
        // Player ra khỏi vùng detect
        if (distance > idleAndChase.DetectRange)
        {
            ChangeState(BehaviourState.Idle);

            idleAndChase.StopMovement();
            idleAndChase.SetAnimationSpeed(0f);

            return;
        }

        // Player đủ gần để attack
        if (distance <= enemyAttack.AttackRange)
        {
            ChangeState(BehaviourState.Attack);

            idleAndChase.StopMovement();
            idleAndChase.SetAnimationSpeed(0f);

            return;
        }

        idleAndChase.UpdateChase(distance);
    }

    // =========================================================
    // ATTACK
    // =========================================================

    private void UpdateAttack(float distance)
    {
        // Player ra khỏi attack range
        if (distance > enemyAttack.AttackRange)
        {
            ChangeState(BehaviourState.Chase);

            enemyAttack.StopAttack();

            return;
        }

        idleAndChase.StopMovement();

        enemyAttack.UpdateAttack();
    }

    // =========================================================
    // CHANGE STATE
    // =========================================================

    private void ChangeState(BehaviourState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
    }

    // =========================================================
    // STOP
    // =========================================================

    private void StopBehaviour()
    {
        enemyAttack.StopAttack();

        idleAndChase.StopMovement();
        idleAndChase.SetAnimationSpeed(0f);
    }

}
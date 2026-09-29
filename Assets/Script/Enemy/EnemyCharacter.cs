using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacter : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField]
    private EnemyData enemyData;

    public EnemyData Data => enemyData;


    // =========================================================
    // RUNTIME REFERENCES
    // =========================================================

    private BaseEnemy baseEnemy;
    private EnemyAttack enemyAttack;
    private NavMeshAgent agent;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        baseEnemy = GetComponent<BaseEnemy>();
        enemyAttack = GetComponent<EnemyAttack>();
        agent = GetComponent<NavMeshAgent>();

        if (baseEnemy == null)
        {
            Debug.LogError(
                $"[{name}] EnemyCharacter cần BaseEnemy."
            );
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ApplyData();
    }


    // =========================================================
    // APPLY DATA
    // =========================================================

    public void ApplyData()
    {
        if (enemyData == null)
        {
            Debug.LogWarning(
                $"[{name}] EnemyCharacter chưa được gán EnemyData."
            );

            return;
        }


        // =====================================================
        // BASE CHARACTER
        // =====================================================

        if (baseEnemy != null)
        {
            baseEnemy.SetMaxHp(
                enemyData.maxHp
            );

            baseEnemy.SetDefense(
                enemyData.defense
            );
        }


        // =====================================================
        // NAVMESH
        // =====================================================

        if (agent != null)
        {
            agent.speed =
                Mathf.Max(
                    0f,
                    enemyData.moveSpeed
                );
        }


        // =====================================================
        // ATTACK
        // =====================================================

        if (enemyAttack != null)
        {
            enemyAttack.SetAttackDamage(
                enemyData.atk
            );

            enemyAttack.SetAttackCooldown(
                enemyData.atkCD
            );

            enemyAttack.SetAttackRange(
                enemyData.enemyRanged
            );
        }
    }


    // =========================================================
    // SET DATA
    // =========================================================

    public void SetEnemyData(EnemyData data)
    {
        enemyData = data;

        ApplyData();
    }
}
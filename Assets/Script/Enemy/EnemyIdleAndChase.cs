using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleAndChase : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectRange = 15f;

    [Header("NavMesh Optimize")]
    [SerializeField] private float nearUpdateRate = 0.1f;
    [SerializeField] private float mediumUpdateRate = 0.25f;
    [SerializeField] private float farUpdateRate = 0.5f;
    [SerializeField] private float veryFarUpdateRate = 1f;

    private EnemyBehaviour behaviour;
    private NavMeshAgent agent;
    private EnemyAnim enemyAnim;

    private Transform player;

    private float nextUpdatePathTime;

    public float DetectRange => detectRange;

    public void Initialize(
        EnemyBehaviour behaviour,
        NavMeshAgent agent,
        EnemyAnim enemyAnim)
    {
        this.behaviour = behaviour;
        this.agent = agent;
        this.enemyAnim = enemyAnim;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    // =========================================================
    // IDLE
    // =========================================================

    public void UpdateIdle(float distance)
    {
        StopMovement();

        SetAnimationSpeed(0f);
    }

    // =========================================================
    // CHASE
    // =========================================================

    public void UpdateChase(float distance)
    {
        if (player == null)
            return;

        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;

        // Không SetDestination mỗi frame
        if (Time.time >= nextUpdatePathTime)
        {
            nextUpdatePathTime =
                Time.time +
                GetPathUpdateRate(distance);

            agent.SetDestination(
                player.position
            );
        }

        float speed =
            agent.velocity.magnitude;

        SetAnimationSpeed(
            speed > 0.1f ? 1f : 0f
        );
    }

    // =========================================================
    // NAVMESH
    // =========================================================

    private float GetPathUpdateRate(float distance)
    {
        if (distance < 10f)
            return nearUpdateRate;

        if (distance < 20f)
            return mediumUpdateRate;

        if (distance < 40f)
            return farUpdateRate;

        return veryFarUpdateRate;
    }

    public void StopMovement()
    {
        if (agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        agent.ResetPath();
        agent.isStopped = true;
    }

    public void SetAnimationSpeed(float speed)
    {
        if (enemyAnim != null)
        {
            enemyAnim.SetSpeed(speed);
        }
    }
}
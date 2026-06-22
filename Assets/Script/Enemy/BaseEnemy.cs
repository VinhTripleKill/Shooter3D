using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BaseEnemy :
    BaseCharacter, IAutoAimTarget
{
    protected Transform player;
    public static List<BaseEnemy> AllEnemies { get; private set; } = new List<BaseEnemy>();
    [Header("Detection")]
    [SerializeField]
    protected float detectionRange = 30f;

    [SerializeField]
    protected LayerMask playerLayer;

    protected bool hasDetectedPlayer;
    protected EnemyAnim enemyAnim;

    protected override void Awake()
    {
        base.Awake();

        enemyAnim = GetComponent<EnemyAnim>();
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

    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")
                           ?.transform;
    }

    protected virtual void DetectPlayer()
    {
        if (isDead)
        {
            hasDetectedPlayer = false;
            return;
        }

        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                detectionRange,
                playerLayer);

        hasDetectedPlayer = hits.Length > 0;
    }
    protected virtual void Update()
    {
        if (isDead)
            return;

        DetectPlayer();
        Move();
        ApplyGravity();
    }

    protected virtual void Move()
    {
        FollowPlayer();
    }

    protected virtual void FollowPlayer()
    {
        if (player == null)
            return;

        if (!hasDetectedPlayer)
        {
            enemyAnim?.SetWalk(false);
            return;
        }

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
        {
            enemyAnim?.SetWalk(false);
            return;
        }

        enemyAnim?.SetWalk(true);

        transform.forward = dir.normalized;

        controller.Move(
            dir.normalized *
            moveSpeed *
            Time.deltaTime);
    }

    public virtual Transform GetTargetTransform()
    {
        return transform;
    }
    protected override void Die()
    {
        base.Die();

        enemyAnim?.SetWalk(false);

        // remove khỏi auto aim ngay lập tức
        AutoAimManager.Unregister(this);
        AllEnemies.Remove(this);

        // stop movement
        enabled = false;
        controller.enabled = false;

        if (enemyAnim != null)
        {
            enemyAnim.PlayDead(() =>
            {
                Destroy(gameObject);
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange);
    }
}
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BaseEnemy :
    BaseCharacter, IAutoAimTarget
{
    protected Transform player;
    public static List<BaseEnemy> AllEnemies { get; private set; } = new List<BaseEnemy>();
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

    protected virtual void Update()
    {
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

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            return;

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
}
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BaseEnemy :
    MonoBehaviour, IDamageable
{
    public static List<BaseEnemy> AllEnemies =
        new List<BaseEnemy>();

    [Header("HP")]
    [SerializeField]
    protected float maxHp = 100f;

    protected float currentHp;

    [Header("Movement")]
    [SerializeField]
    protected float moveSpeed = 3f;

    [SerializeField]
    protected float gravity = -20f;

    protected CharacterController controller;
    protected Transform player;

    protected float verticalVelocity;

    protected virtual void OnEnable()
    {
        AllEnemies.Add(this);
    }

    protected virtual void OnDisable()
    {
        AllEnemies.Remove(this);
    }

    protected virtual void Awake()
    {
        currentHp = maxHp;

        controller =
            GetComponent<CharacterController>();
    }

    protected virtual void Start()
    {
        player =
            GameObject.FindGameObjectWithTag("Player")
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

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return;

        transform.forward =
            direction.normalized;

        controller.Move(
            direction.normalized *
            moveSpeed *
            Time.deltaTime);
    }

    protected virtual void ApplyGravity()
    {
        if (controller.isGrounded &&
            verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity * Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }
}
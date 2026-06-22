using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BaseCharacter :
    MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] protected float maxHp = 100f;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float gravity = -20f;

    protected float currentHp;

    protected CharacterController controller;
    protected float verticalVelocity;
    protected bool isDead;

    public bool IsDead()
    {
        return isDead;
    }
    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentHp = maxHp;
    }
    protected virtual void Start()
    {

    }
    public virtual void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            currentHp = 0;
            isDead = true;

            Die();
        }
    }

    protected virtual void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime);
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public float GetCurrentHp() => currentHp;
    public float GetMaxHp() => maxHp;
}
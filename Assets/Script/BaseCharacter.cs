using UnityEngine;

public abstract class BaseCharacter :
    MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField]
    protected float maxHp = 100f;
    public System.Action<float, float> OnHpChanged;
    public System.Action<float> OnTakeDamage;
    protected float currentHp;
    protected bool isDead;
    
    public bool IsDead()
    {
        return isDead;
    }

    protected virtual void Awake()
    {
        currentHp = maxHp;

        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHp -= damage;

        currentHp = Mathf.Clamp(
            currentHp,
            0,
            maxHp);

        // Báo vừa nhận damage
        OnTakeDamage?.Invoke(damage);

        // Báo HP đã thay đổi
        OnHpChanged?.Invoke(
            currentHp,
            maxHp);

        if (currentHp <= 0)
        {
            isDead = true;
            Die();
        }
    }
    public virtual void Heal(float amount)
    {
        if (isDead)
            return;

        currentHp += amount;

        currentHp = Mathf.Clamp(
            currentHp,
            0,
            maxHp);

        OnHpChanged?.Invoke(
            currentHp,
            maxHp);
    }
    protected virtual void Die()
    {
        isDead = true;
    }

    public float GetCurrentHp() => currentHp;

    public float GetMaxHp() => maxHp;
}
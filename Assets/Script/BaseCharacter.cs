using UnityEngine;

public abstract class BaseCharacter :
    MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField]
    protected float maxHp = 100f;
    public System.Action<float, float> OnHpChanged;
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

        OnHpChanged?.Invoke(
            currentHp,
            maxHp);

        if (currentHp <= 0)
        {
            isDead = true;
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    public float GetCurrentHp() => currentHp;

    public float GetMaxHp() => maxHp;
}
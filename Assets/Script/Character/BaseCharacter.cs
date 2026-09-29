
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField]
    protected float maxHp = 100f;

    [Header("Defense")]
    [SerializeField]
    protected float defense = 0f;

    public System.Action<float, float> OnHpChanged;
    public System.Action<float> OnTakeDamage;

    public float currentHp;

    protected bool isDead;

    // =========================================================
    // PROPERTIES
    // =========================================================

    public float CurrentDefense => defense;

    // =========================================================
    // DEAD
    // =========================================================

    public bool IsDead()
    {
        return isDead;
    }

    // =========================================================
    // AWAKE
    // =========================================================

    protected virtual void Awake()
    {
        currentHp = maxHp;

        OnHpChanged?.Invoke(
            currentHp,
            maxHp
        );
    }

    // =========================================================
    // TAKE DAMAGE
    // =========================================================

    public virtual void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        // =====================================================
        // RAW DAMAGE
        // =====================================================

        float rawDamage = damage;

        // =====================================================
        // DEFENSE REDUCTION
        // =====================================================

        float finalDamage =
            CalculateDamageAfterDefense(
                rawDamage
            );

        // Không cho damage âm
        finalDamage =
            Mathf.Max(
                0f,
                finalDamage
            );

        // =====================================================
        // APPLY HP
        // =====================================================

        currentHp -= finalDamage;

        currentHp =
            Mathf.Clamp(
                currentHp,
                0f,
                maxHp
            );

        // =====================================================
        // DAMAGE EVENT
        // =====================================================
        //
        // Quan trọng:
        //
        // OnTakeDamage nhận FINAL DAMAGE
        // sau khi đã tính Defense.
        //
        // Damage Popup sẽ hiển thị số damage
        // Player thực sự nhận.
        //
        // =====================================================

        OnTakeDamage?.Invoke(
            finalDamage
        );

        // =====================================================
        // HP EVENT
        // =====================================================

        OnHpChanged?.Invoke(
            currentHp,
            maxHp
        );

        // =====================================================
        // DEATH
        // =====================================================

        if (currentHp <= 0f)
        {
            isDead = true;

            Die();
        }
    }

    // =========================================================
    // CALCULATE DAMAGE
    // =========================================================
    //
    // Công thức:
    //
    // FinalDamage =
    // RawDamage * 100 / (100 + Defense)
    //
    // Ví dụ:
    //
    // Defense 0:
    // 100 -> 100
    //
    // Defense 50:
    // 100 -> 66.67
    //
    // Defense 100:
    // 100 -> 50
    //
    // Defense 200:
    // 100 -> 33.33
    //
    // =========================================================

    protected virtual float CalculateDamageAfterDefense(
        float rawDamage
    )
    {
        float currentDefense =
            GetDefense();

        currentDefense =
            Mathf.Max(
                0f,
                currentDefense
            );

        return
            rawDamage *
            (100f /
            (100f + currentDefense));
    }

    // =========================================================
    // GET DEFENSE
    // =========================================================
    //
    // Tách thành method riêng để sau này
    // RuntimeCombatStats có thể override logic.
    //
    // =========================================================

    protected virtual float GetDefense()
    {
        return defense;
    }

    // =========================================================
    // SET DEFENSE
    // =========================================================

    public virtual void SetDefense(float value)
    {
        defense =
            Mathf.Max(
                0f,
                value
            );
    }
    public virtual void SetMaxHp(float value)
{
    maxHp = Mathf.Max(0f, value);

    currentHp = maxHp;

    isDead = false;

    OnHpChanged?.Invoke(
        currentHp,
        maxHp
    );
}

    public virtual void Heal(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0f)
            return;

        currentHp += amount;

        currentHp =
            Mathf.Clamp(
                currentHp,
                0f,
                maxHp
            );

        OnHpChanged?.Invoke(
            currentHp,
            maxHp
        );
    }

    // =========================================================
    // DIE
    // =========================================================

    protected virtual void Die()
    {
        isDead = true;
    }

    // =========================================================
    // GET HP
    // =========================================================

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }
}

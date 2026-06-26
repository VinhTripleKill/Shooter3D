using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BasePlayer : BaseCharacter
{
    [Header("Move")]
    [SerializeField]
    protected float moveSpeed = 5f;

    [SerializeField]
    protected float gravity = -20f;

    protected CharacterController controller;
    protected float verticalVelocity;

    [Header("Mana")]
    [SerializeField]
    protected float maxMana = 100;
    public System.Action<float, float> OnManaChanged;
    protected float currentMana;

    [Header("Sprint")]
    [SerializeField]
    protected float maxSprintEnergy = 100;

    protected float currentSprintEnergy;

    protected override void Awake()
    {
        base.Awake();

        controller = GetComponent<CharacterController>();

        currentMana = maxMana;
        currentSprintEnergy = maxSprintEnergy;
        OnManaChanged?.Invoke(currentMana, maxMana);
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
    public float GetCurrentMana()
    {
        return currentMana;
    }

    public float GetMaxMana()
    {
        return maxMana;
    }
    public bool ConsumeMana(float mana)
    {
        if (currentMana < mana)
            return false;

        currentMana -= mana;

        OnManaChanged?.Invoke(currentMana, maxMana);

        return true;
    }
}
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
}
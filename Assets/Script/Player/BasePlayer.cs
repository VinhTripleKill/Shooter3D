using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class BasePlayer : BaseCharacter
{
    [Header("Move")]
    [SerializeField] protected float gravity = -20f;
    protected PlayerCharacter playerCharacter;
    protected CharacterController controller;
    protected float verticalVelocity;
    public System.Action<float, float> OnManaChanged;
    protected float currentMana;
    protected float currentSprintEnergy;

    protected override void Awake()
    {
        base.Awake();
        
        controller = GetComponent<CharacterController>();
        playerCharacter = GetComponent<PlayerCharacter>();
        maxHp = playerCharacter.Data.CharacterStats.maxHp;
        currentHp = maxHp;
        currentMana = playerCharacter.Data.CharacterStats.maxMana;
        currentSprintEnergy = playerCharacter.Data.CharacterStats.maxSprint;
        OnManaChanged?.Invoke(currentMana, playerCharacter.Data.CharacterStats.maxMana);
    }

    protected virtual void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move( Vector3.up * verticalVelocity * Time.deltaTime);
    }
    public float GetCurrentMana()
    {
        return currentMana;
    }

    public float GetMaxMana()
    {
        return playerCharacter.Data.CharacterStats.maxMana;;
    }
    public bool ConsumeMana(float mana)
    {
        if (currentMana < mana)
            return false;

        currentMana -= mana;

        OnManaChanged?.Invoke(currentMana, playerCharacter.Data.CharacterStats.maxMana);

        return true;
    }
    public void RecoverMana(float mana)
{
    if (mana <= 0f) return;

    currentMana += mana;

    currentMana = Mathf.Clamp( currentMana, 0, playerCharacter.Data.CharacterStats.maxMana);

    OnManaChanged?.Invoke( currentMana, playerCharacter.Data.CharacterStats.maxMana);
}
}
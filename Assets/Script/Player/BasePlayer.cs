using UnityEngine;

public abstract class BasePlayer : BaseCharacter
{
    [Header("Mana")]
    [SerializeField] protected float maxMana = 100;
    protected float currentMana;

    [Header("Sprint")]
    [SerializeField] protected float maxSprintEnergy = 100;
    protected float currentSprintEnergy;

    protected override void Awake()
    {
        base.Awake();

        currentMana = maxMana;
        currentSprintEnergy = maxSprintEnergy;
    }
}
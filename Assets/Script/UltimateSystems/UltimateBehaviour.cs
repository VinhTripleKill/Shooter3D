using UnityEngine;

public abstract class UltimateBehaviour : MonoBehaviour
{
    [SerializeField]
    protected UltimateData data;

    protected PlayerUltimate owner;

    protected PlayerController playerController;
    protected PlayerCharacter playerCharacter;
    protected PlayerAnim playerAnim;
    protected PlayerSkill playerSkill;
    protected PlayerWeapon playerWeapon;
    protected PlayerSprint playerSprint;
    protected PlayerInteraction playerInteraction;

    protected UltimateSocket socket;

    public UltimateData Data => data;

    public virtual void Initialize(PlayerUltimate owner)
    {
        this.owner = owner;

        playerController = owner.GetComponent<PlayerController>();
        playerCharacter = owner.GetComponent<PlayerCharacter>();
        playerAnim = owner.GetComponent<PlayerAnim>();
        playerSkill = owner.GetComponent<PlayerSkill>();
        playerWeapon = owner.GetComponent<PlayerWeapon>();
        playerSprint = owner.GetComponent<PlayerSprint>();
        playerInteraction = owner.GetComponent<PlayerInteraction>();

        socket = owner.GetComponentInChildren<UltimateSocket>();
    }

    public virtual bool CanExecute()
    {
        if (playerController == null) return false;

        if (playerController.IsDead()) return false;

        return true;
    }

    public abstract bool Execute();
}
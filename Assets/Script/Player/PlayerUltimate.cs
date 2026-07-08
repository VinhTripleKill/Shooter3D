using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerUltimate : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerInput input;
    private InputAction ultimateAction;
    private float currentCooldown;
    [SerializeField] private GamePlayUI gameplayUI;
    private PlayerCharacter playerCharacter;
    [SerializeField] private UltimateBehaviour currentUltimate;
    [SerializeField] private UltimateSocket ultimateSocket;
    private bool canUltimate = true;
    private float cooldownTimer;
    private bool isCooldown;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();

        ultimateAction = input.actions["Ultimate"];

        gameplayUI.SetUltimateBar(1f);
        playerCharacter = GetComponent<PlayerCharacter>();

        ultimateSocket.SetUltimate( playerCharacter.Data.ultimatePrefab, this);



        currentUltimate = ultimateSocket.CurrentUltimate;
    }

    private void OnEnable()
    {
        ultimateAction.performed += UltimatePerformed;
        gameplayUI.GetUltimateButton().onClick.AddListener(UseUltimate);
    }

    private void OnDisable()
    {
        ultimateAction.performed -= UltimatePerformed;
        gameplayUI.GetUltimateButton().onClick.RemoveListener(UseUltimate);
    }

    private void UltimatePerformed(InputAction.CallbackContext ctx)
    {
        UseUltimate();
    }

    private void UseUltimate()
{
    if (!canUltimate)
        return;

    if (playerController.IsDead())
        return;

    if (currentUltimate == null)
        return;

    UltimateData data = currentUltimate.Data;

    if (!playerController.ConsumeMana(data.manaCost))
    {
        Debug.Log("Not enough mana");
        return;
    }

    bool success = currentUltimate.Execute();

    if (!success)
        return;

    StartCooldown(data.cooldown);
}

private void StartCooldown(float cooldown)
{
    canUltimate = false;
    isCooldown = true;

    currentCooldown = cooldown;
    cooldownTimer = cooldown;

    gameplayUI.SetUltimateBar(0f);
}

    private void Update()
    {
        if (!isCooldown)
            return;

        cooldownTimer -= Time.deltaTime;

        float progress = 1 - (cooldownTimer / currentCooldown);

        gameplayUI.SetUltimateBar(progress);

        if (cooldownTimer <= 0f)
        {
            isCooldown = false;
            canUltimate = true;

            gameplayUI.SetUltimateBar(1f);
        }
    }
    
}
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerUltimate : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerInput input;
    private InputAction ultimateAction;

    [SerializeField] private GamePlayUI gameplayUI;

    [Header("Ultimate Config")]
    [SerializeField] private float manaConsumption = 20f;
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float ultimateCd = 5f;

    private bool canUltimate = true;
    private float cooldownTimer;
    private bool isCooldown;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();

        ultimateAction = input.actions["Ultimate"];

        gameplayUI.SetUltimateBar(1f);
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

        if (!playerController.ConsumeMana(manaConsumption))
        {
            Debug.Log("Insufficient mana");
            return;
        }

        playerController.Heal(healAmount);

        StartCooldown();
    }

    private void StartCooldown()
    {
        canUltimate = false;
        isCooldown = true;
        cooldownTimer = ultimateCd;

        // reset bar về 0 khi dùng ultimate
        gameplayUI.SetUltimateBar(0f);
    }

    private void Update()
    {
        if (!isCooldown)
            return;

        cooldownTimer -= Time.deltaTime;

        float progress = 1f - (cooldownTimer / ultimateCd);

        gameplayUI.SetUltimateBar(progress);

        if (cooldownTimer <= 0f)
        {
            isCooldown = false;
            canUltimate = true;

            gameplayUI.SetUltimateBar(1f);
        }
    }
}
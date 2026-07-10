using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerUltimate : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerInput input;
    private InputAction ultimateAction;

    public GamePlayUI gameplayUI;

    private PlayerCharacter playerCharacter;
    private UltimateBehaviour currentUltimate;
    [SerializeField] private UltimateSocket ultimateSocket;

    private bool canUltimate = true;
    private float currentCooldown;
    private float cooldownTimer;
    private bool isCooldown;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();
        playerCharacter = GetComponent<PlayerCharacter>();

        if (input != null) ultimateAction = input.actions["Ultimate"];
    }


    public void InitializeUltimate()
    {
        if (playerCharacter?.Data?.ultimatePrefab == null)
        {
            Debug.LogWarning("Character không có UltimatePrefab");
            return;
        }

        ultimateSocket.SetUltimate(playerCharacter.Data.ultimatePrefab, this);
        currentUltimate = ultimateSocket.CurrentUltimate;

        Debug.Log("PlayerUltimate: Đã khởi tạo Ultimate");
    }

    public void SetGameplayUI(GamePlayUI ui)
    {
        gameplayUI = ui;

        if (gameplayUI != null && currentUltimate != null)
        {
            UltimateData data = currentUltimate.Data;
            if (data != null && data.icon != null)
            {
                gameplayUI.SetUltimateIcon(data.icon);     // ← Thêm hàm này
            }

            gameplayUI.SetUltimateBar(1f);
            gameplayUI.GetUltimateButton().onClick.AddListener(UseUltimate);
        }
    }

    private void OnEnable()
    {
        if (ultimateAction != null)
            ultimateAction.performed += UltimatePerformed;

        // Tránh lỗi nếu gameplayUI chưa được gán
        if (gameplayUI != null)
            gameplayUI.GetUltimateButton().onClick.AddListener(UseUltimate);
    }

    private void OnDisable()
    {
        if (ultimateAction != null)
            ultimateAction.performed -= UltimatePerformed;

        if (gameplayUI != null)
            gameplayUI.GetUltimateButton().onClick.RemoveListener(UseUltimate);
    }

    private void UltimatePerformed(InputAction.CallbackContext ctx)
    {
        UseUltimate();
    }

    private void UseUltimate()
    {
        if (!canUltimate || playerController.IsDead() || currentUltimate == null)
            return;

        UltimateData data = currentUltimate.Data;
        if (!playerController.ConsumeMana(data.manaCost))
        {
            Debug.Log("Not enough mana");
            return;
        }

        bool success = currentUltimate.Execute();
        if (!success) return;

        StartCooldown(data.cooldown);
    }

    private void StartCooldown(float cooldown)
    {
        canUltimate = false;
        isCooldown = true;
        currentCooldown = cooldown;
        cooldownTimer = cooldown;

        if (gameplayUI != null)
            gameplayUI.SetUltimateBar(0f);
    }

    private void Update()
    {
        if (!isCooldown || gameplayUI == null) return;

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
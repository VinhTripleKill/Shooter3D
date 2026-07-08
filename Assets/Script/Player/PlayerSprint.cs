using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerSprint : MonoBehaviour
{
    [Header("Sprint Settings")]
    [SerializeField] private ParticleSystem sprintEffect;
    [SerializeField] private float sprintLockDuration = 5f;
    public float sprintConsumption = 5f;
    [Header("Sprint Energy")]
    [SerializeField] private float sprintSpeed = 3;
    [SerializeField] private float sprintRecoveryWalk = 5;
    [SerializeField] private float sprintRecoveryIdle = 10;
    
    private bool isSprintOn = true;
    private bool sprintLocked;
    private float sprintLockTimer;
    private bool wasSprinting;

    // Reference
    private PlayerController playerController;
    public GamePlayUI gameplayUI;
    private InputAction sprintAction;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        
    }

    public void Initialize(InputAction sprintInputAction)
    {
        sprintAction = sprintInputAction;
        
        if (gameplayUI != null)
        {
            gameplayUI.GetSprintButton().onClick.AddListener(ToggleSprint);
        }
    }

    public void OnEnableSprint()
    {
        if (sprintAction != null)
            sprintAction.performed += SprintPerformed;
    }

    public void OnDisableSprint()
    {
        if (sprintAction != null)
            sprintAction.performed -= SprintPerformed;
    }

    private void Update()
    {
        UpdateSprintLock();
        HandleSprintEnergy();
        UpdateSprintEffect();
    }

    private void UpdateSprintEffect()
    {
        bool isSprintingNow = IsSprinting();

        if (isSprintingNow && !wasSprinting)
        {
            sprintEffect?.Play();
        }
        else if (!isSprintingNow && wasSprinting)
        {
            sprintEffect?.Stop();
        }

        wasSprinting = isSprintingNow;
    }

    public void ToggleSprint()
    {
        if (sprintLocked) return;
        isSprintOn = !isSprintOn;
        Debug.Log($"Sprint: {(isSprintOn ? "On" : "Off")}");
    }

    private void SprintPerformed(InputAction.CallbackContext ctx)
    {
        ToggleSprint();
    }

    private void HandleSprintEnergy()
    {
        bool isMoving = playerController.IsMoving();
        bool isActuallySprinting = isSprintOn && isMoving && playerController.CurrentSprintEnergy > 0f;

        if (isActuallySprinting)
            playerController.CurrentSprintEnergy -= sprintConsumption * Time.deltaTime;
        else if (isMoving)
            playerController.CurrentSprintEnergy += sprintRecoveryWalk * Time.deltaTime;
        else
            playerController.CurrentSprintEnergy += sprintRecoveryIdle * Time.deltaTime;

        playerController.CurrentSprintEnergy = Mathf.Clamp(playerController.CurrentSprintEnergy, 0f, playerController.MaxSprintEnergy);

        if (!sprintLocked && playerController.CurrentSprintEnergy <= 0f)
            SprintOutOfEnergy();

        if (gameplayUI != null)
            gameplayUI.UpdateSprintBar(playerController.CurrentSprintEnergy, playerController.MaxSprintEnergy);
    }

    private void SprintOutOfEnergy()
    {
        sprintLocked = true;
        sprintLockTimer = sprintLockDuration;
        isSprintOn = false;
        gameplayUI?.ShowSprintLock();
    }

    private void UpdateSprintLock()
    {
        if (!sprintLocked) return;

        sprintLockTimer -= Time.deltaTime;
        if (sprintLockTimer <= 0f)
        {
            sprintLocked = false;
            gameplayUI?.HideSprintLock();
        }
    }

    public bool IsSprinting()
    {
        return playerController.Controller.isGrounded &&
               playerController.IsMoving() &&
               isSprintOn &&
               playerController.CurrentSprintEnergy > 0f;
    }

    // Getter / Helper cho PlayerController
    public float GetSprintSpeedBonus() => sprintSpeed;
    public bool CanSprint() => isSprintOn && playerController.CurrentSprintEnergy > 0f;
    public bool IsSprintOn() => isSprintOn;
}
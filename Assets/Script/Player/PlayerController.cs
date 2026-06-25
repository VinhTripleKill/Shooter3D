using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : BasePlayer
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    [Header("Mobile")]
    [SerializeField] private JoystickMove joystickMove;
    [Header("Movement")]
    public float sprintSpeed = 3f;
    [Header("Sprint Effect")]
    [SerializeField]
    private ParticleSystem sprintEffect;
    [SerializeField]
    private float rotationSpeed = 10f;
    [Header("Sprint Energy")]
    public float sprintConsumption = 5f;
    public float sprintRecoveryWalk = 5f;
    public float sprintRecoveryIdle = 10f;
    private bool wasSprinting;
    private bool sprintLocked;
    private float sprintLockTimer;
    [SerializeField] private float sprintLockDuration = 5f;
    private float testTimer;
    private bool isSprintOn = true;
    private Vector2 moveInput;
    private PlayerAnim playerAnim;
    private bool canMove = true;
    [Header("UI")]
    [SerializeField] private GamePlayUI gameplayUI;

    // Reference đến script weapon
    public PlayerWeapon playerWeapon;

    protected override void Awake()
    {
        base.Awake();

        playerAnim = GetComponent<PlayerAnim>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];

        gameplayUI.UpdateSprintBar(currentSprintEnergy, maxSprintEnergy);
        gameplayUI.GetSprintButton().onClick.AddListener(ToggleSprint);
        gameplayUI.UpdateHpBar(currentHp, maxHp);
        gameplayUI.UpdateManaBar(currentMana, maxMana);
        if (playerWeapon == null)
            playerWeapon = GetComponent<PlayerWeapon>();
        OnHpChanged += gameplayUI.UpdateHpBar;
        if (playerWeapon == null)
            playerWeapon = gameObject.AddComponent<PlayerWeapon>();

    }

    private void OnEnable()
    {
        sprintAction.performed += SprintPerformed;
    }

    private void OnDisable()
    {
        sprintAction.performed -= SprintPerformed;
    }

    private void Update()
    {
        if (isDead)
            return;
        ReadMovementInput();

        UpdateSprintLock();
        HandleSprintEnergy();

        Move();

        ApplyGravity(); // từ BaseCharacter

        testTimer += Time.deltaTime;

        if (testTimer >= 1f)
        {
            testTimer = 0f;
            TakeDamage(0);
           
        }

        UpdateSprintEffect();
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        gameplayUI.UpdateHpBar(currentHp, maxHp);
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
    private void ReadMovementInput()
    {
        Vector2 keyboardInput =
            moveAction.ReadValue<Vector2>();

        if (joystickMove != null &&
            joystickMove.IsDragging())
        {
            moveInput = joystickMove.MoveDirection;
        }
        else
        {
            moveInput = keyboardInput;
        }
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
        bool isMoving = moveInput != Vector2.zero;
        bool isActuallySprinting = isSprintOn && isMoving && currentSprintEnergy > 0f;

        if (isActuallySprinting)
            currentSprintEnergy -= sprintConsumption * Time.deltaTime;
        else if (isMoving)
            currentSprintEnergy += sprintRecoveryWalk * Time.deltaTime;
        else
            currentSprintEnergy += sprintRecoveryIdle * Time.deltaTime;

        currentSprintEnergy = Mathf.Clamp(currentSprintEnergy, 0f, maxSprintEnergy);

        if (!sprintLocked && currentSprintEnergy <= 0f)
            SprintOutOfEnergy();

        gameplayUI.UpdateSprintBar(currentSprintEnergy, maxSprintEnergy);
    }

    private void SprintOutOfEnergy()
    {
        sprintLocked = true;
        sprintLockTimer = sprintLockDuration;
        isSprintOn = false;
        gameplayUI.ShowSprintLock();
    }

    private void UpdateSprintLock()
    {
        if (!sprintLocked) return;

        sprintLockTimer -= Time.deltaTime;
        if (sprintLockTimer <= 0f)
        {
            sprintLocked = false;
            gameplayUI.HideSprintLock();
        }
    }

    private void Move()
    {
        if (!canMove)
            return;
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        float currentSpeed = moveSpeed;

        bool canSprint = isSprintOn && move != Vector3.zero && currentSprintEnergy > 0f;
        if (canSprint)
            currentSpeed += sprintSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Xoay hướng
        if (move != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(move);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime);
        }

        // Animation
        float animSpeed = 0f;
        if (move != Vector3.zero)
            animSpeed = canSprint ? 1f : 0.5f;

        playerAnim.SetSpeed(animSpeed);
    }

    private bool IsSprinting()
    {
        return controller.isGrounded &&
               IsMoving() &&
               isSprintOn &&
               currentSprintEnergy > 0f;
    }
    private bool IsMoving()
    {
        return moveInput.sqrMagnitude > 0.01f;
    }
    private void OnDestroy()
    {
        OnHpChanged -= gameplayUI.UpdateHpBar;
    }
    protected override void Die()
    {
        Debug.Log("Player has die");
        gameplayUI.UpdateHpBar(currentHp, maxHp);
        canMove = false;

        playerAnim.PlayDead();
    }
    // Public để PlayerWeapon truy cập
    public bool IsSprintingPublic() => IsSprinting();
}
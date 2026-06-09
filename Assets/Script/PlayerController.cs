using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    [Header("Mobile")]
    [SerializeField] private JoystickMove joystickMove;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 3f;
    public float gravity = -20f;

    [Header("Sprint Energy")]
    public float maxSprintEnergy = 100f;
    public float sprintConsumption = 5f;
    public float sprintRecoveryWalk = 5f;
    public float sprintRecoveryIdle = 10f;

    private float currentSprintEnergy;
    private bool sprintLocked;
    private float sprintLockTimer;
    [SerializeField] private float sprintLockDuration = 5f;

    private bool isSprintOn = true;
    private Vector2 moveInput;
    private Vector3 velocity;

    private CharacterController controller;
    private PlayerAnim playerAnim;

    [Header("UI")]
    [SerializeField] private GamePlayUI gameplayUI;

    // Reference đến script weapon
    public PlayerWeapon playerWeapon;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerAnim = GetComponent<PlayerAnim>();
        playerInput = GetComponent<PlayerInput>();

        // Input cơ bản
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];

        currentSprintEnergy = maxSprintEnergy;
        gameplayUI.UpdateSprintBar(currentSprintEnergy, maxSprintEnergy);
        gameplayUI.GetSprintButton().onClick.AddListener(ToggleSprint);

        // Lấy hoặc thêm PlayerWeapon
        if (playerWeapon == null)
            playerWeapon = GetComponent<PlayerWeapon>();

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
        ReadMovementInput();

        UpdateSprintLock();
        HandleSprintEnergy();
        Move();
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
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        float currentSpeed = moveSpeed;

        bool canSprint = isSprintOn && move != Vector3.zero && currentSprintEnergy > 0f;
        if (canSprint)
            currentSpeed += sprintSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Xoay hướng
        if (move != Vector3.zero)
            transform.forward = move;

        // Animation
        float animSpeed = 0f;
        if (move != Vector3.zero)
            animSpeed = canSprint ? 1f : 0.5f;

        playerAnim.SetSpeed(animSpeed);
    }

    private bool IsSprinting()
    {
        return isSprintOn && moveInput != Vector2.zero && currentSprintEnergy > 0f;
    }

    // Public để PlayerWeapon truy cập
    public bool IsSprintingPublic() => IsSprinting();
}
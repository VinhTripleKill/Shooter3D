using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : BasePlayer
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private PlayerSprint playerSprint;

    [Header("Mobile")]
    [SerializeField] private JoystickMove joystickMove;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("UI")]
    [SerializeField] private GamePlayUI gameplayUI;
    private float damageTestTimer = 0f;
    private PlayerWeapon playerWeapon;
    private PlayerProgress playerProgress;
    private Vector2 moveInput;
    private PlayerAnim playerAnim;
    private bool canMove = true;
    private bool canDash = true;
    private bool isDashing;
    private float dashLockTimer;

protected override void Awake()
    {
        base.Awake();
        playerWeapon = GetComponent<PlayerWeapon>();
        playerAnim = GetComponent<PlayerAnim>();
        playerInput = GetComponent<PlayerInput>();
        playerSprint = GetComponent<PlayerSprint>();
        playerProgress = GetComponent<PlayerProgress>();
        moveAction = playerInput.actions["Move"];

        // Khởi tạo Sprint
        if (playerSprint != null)
            playerSprint.Initialize(playerInput.actions["Sprint"]);

        if (playerWeapon == null)
            playerWeapon = GetComponent<PlayerWeapon>();
        if (playerWeapon == null)
            playerWeapon = gameObject.AddComponent<PlayerWeapon>();
    }
    public void InitializeSceneReferences(JoystickMove joystick, GamePlayUI ui)
    {
        joystickMove = joystick;
        gameplayUI = ui;

        InitializePlayerStats();
        if (gameplayUI != null)
        {
            if (playerProgress != null)
            {
                playerProgress.OnLevelChanged += gameplayUI.UpdateLevelText;
                playerProgress.OnExpChanged += gameplayUI.UpdateLevelBar;
                gameplayUI.UpdateLevelText(playerProgress);
                gameplayUI.UpdateLevelBar(playerProgress);
            }

            OnHpChanged += gameplayUI.UpdateHpBar;
            OnManaChanged += gameplayUI.UpdateManaBar;

            // Cập nhật UI ban đầu
            gameplayUI.UpdateSprintBar(currentSprintEnergy, playerCharacter.Data.CharacterStats.maxSprint);
            gameplayUI.UpdateHpBar(currentHp, maxHp);
            
            gameplayUI.UpdateManaBar(currentMana, playerCharacter.Data.CharacterStats.maxMana);
        }

        Debug.Log("PlayerController: Đã gán JoystickMove và GamePlayUI từ Scene");
    }
    
    private void OnDestroy()
    {
        OnHpChanged -= gameplayUI.UpdateHpBar;
        OnManaChanged -= gameplayUI.UpdateManaBar;
        if (playerProgress != null)
        {
            playerProgress.OnLevelChanged -= gameplayUI.UpdateLevelText;
            playerProgress.OnExpChanged -= gameplayUI.UpdateLevelBar;
        }
    }

    private void OnEnable()
    {
        playerSprint?.OnEnableSprint();
    }

    private void OnDisable()
    {
        playerSprint?.OnDisableSprint();
    }

    private void Update()
    {
        if (isDead) return;

        ReadMovementInput();
        Move();
        ApplyGravity();

        if (!canDash)
        {
            dashLockTimer -= Time.deltaTime;
            if (dashLockTimer <= 0)
                canDash = true;
        }

        // ==================== TEST DAMAGE ====================
        //TestAutoDamage();
    }

    // Hàm test đơn giản: mỗi 2 giây tự mất 10 HP
    private void TestAutoDamage()
    {
        damageTestTimer += Time.deltaTime;

        if (damageTestTimer >= 2f)   // Mỗi 2 giây trừ 10 HP
        {
            damageTestTimer = 0f;

            if (!isDead)
            {
                TakeDamage(10f);        // Gọi trực tiếp TakeDamage từ BaseCharacter
                Debug.Log("TEST: Player tự mất 10 HP");
            }
        }
    }

    private void ReadMovementInput()
    {
        Vector2 keyboardInput = moveAction.ReadValue<Vector2>();

        if (joystickMove != null && joystickMove.IsDragging())
        {
            moveInput = joystickMove.MoveDirection;
        }
        else
        {
            moveInput = keyboardInput;
        }
    }

    private void Move()
    {
        if (!canMove) return;

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        float currentSpeed = playerCharacter.Data.CharacterStats.moveSpeed;

        // === SPRINT LOGIC ===
        bool canSprint = playerSprint != null && 
                        playerSprint.CanSprint() && 
                        move != Vector3.zero;

        if (canSprint)
            currentSpeed += playerSprint.GetSprintSpeedBonus();

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Xoay hướng
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Animation
        float animSpeed = 0f;
        if (move != Vector3.zero)
            animSpeed = canSprint ? 1f : 0.5f;

        playerAnim.SetSpeed(animSpeed);
    }

    public bool IsMoving()
    {
        return moveInput.sqrMagnitude > 0.01f;
    }

    private bool IsSprinting()
    {
        return playerSprint?.IsSprinting() ?? false;
    }

    protected override void Die()
{
    Debug.Log("Player has die");
    canMove = false;
    playerAnim.PlayDead();

    // Dừng wave
    FindObjectOfType<EnemyWaveSpawn>()?.GameOver();

    // Dừng timer
    FindObjectOfType<CoreGameUI>()?.StopTimer();
}

    // Public APIs
    public bool IsSprintingPublic() => IsSprinting();

    public Vector3 GetMoveDirection()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        return move.normalized;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public bool StartDash(float dashTime, float dashSpeed, float nextDashTime)
    {
        if (isDashing || !canDash) return false;

        StartCoroutine(DashCoroutine(dashTime, dashSpeed, nextDashTime));
        return true;
    }

    private IEnumerator DashCoroutine(float dashTime, float dashSpeed, float nextDashTime)
    {
        Vector3 direction = GetMoveDirection();
        if (direction == Vector3.zero) yield break;

        isDashing = true;
        canDash = false;
        canMove = false;

        float timer = 0f;
        while (timer < dashTime)
        {
            controller.Move(direction * dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        canMove = true;
        isDashing = false;

        yield return new WaitForSeconds(nextDashTime);
        canDash = true;
        dashLockTimer = dashTime + nextDashTime;
    }

    // Expose cho PlayerSprint truy cập
    public CharacterController Controller => controller;
    public float CurrentSprintEnergy { get => currentSprintEnergy; set => currentSprintEnergy = value; }
    public float MaxSprintEnergy => playerCharacter.Data.CharacterStats.maxSprint;
}
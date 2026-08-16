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
    [Header("Scene References")]
    [SerializeField] private GamePlayUI gameplayUI;
    [SerializeField] private CoreGameUI coreGameUI;
    [SerializeField] private EnemyWaveSpawn enemyWaveSpawn;
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
    
    public void InitializeSceneReferences(JoystickMove joystick,GamePlayUI ui,CoreGameUI coreUI,EnemyWaveSpawn waveSpawn)
    {
        joystickMove = joystick;
        gameplayUI = ui;
        coreGameUI = coreUI;
        enemyWaveSpawn = waveSpawn;
    
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
    
            gameplayUI.UpdateSprintBar(currentSprintEnergy, playerCharacter.Data.CharacterStats.maxSprint);
            gameplayUI.UpdateHpBar(currentHp, maxHp);
            gameplayUI.UpdateManaBar(currentMana, playerCharacter.Data.CharacterStats.maxMana);
        } 
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

        bool canSprint = playerSprint != null && playerSprint.CanSprint() && move != Vector3.zero;

        if (canSprint)
            currentSpeed += playerSprint.GetSprintSpeedBonus();

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

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
    
        enemyWaveSpawn?.GameOver();
        coreGameUI?.StopTimer();
    }

    
    public Vector3 GetMoveDirection()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        return move.normalized;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }


    public bool StartDash(Vector3 direction,float dashDistance,float dashTime,float nextDashTime,LayerMask obstacleMask)
    {
        if (isDashing || !canDash)
            return false;
    
        direction.y = 0f;
    
        if (direction.sqrMagnitude < 0.001f)
            direction = transform.forward;
    
        direction.Normalize();
    
        transform.forward = direction;
    
        StartCoroutine(
            DashCoroutine(
                direction,
                dashDistance,
                dashTime,
                nextDashTime,
                obstacleMask
            )
        );
    
        return true;
    }
    
    private IEnumerator DashCoroutine(
        Vector3 direction,
        float dashDistance,
        float dashTime,
        float nextDashTime,
        LayerMask obstacleMask)
    {
        isDashing = true;
        canDash = false;
        canMove = false;
    
        direction.y = 0f;
        direction.Normalize();
    
        transform.forward = direction;
    
        // ==========================================
        // TÍNH KHOẢNG CÁCH DASH THỰC TẾ
        // ==========================================
    
        float actualDistance =
            CalculateDashDistance(
                direction,
                dashDistance,
                obstacleMask
            );
    
        Debug.Log(
            $"DASH | " +
            $"Requested: {dashDistance:F2}m | " +
            $"Actual: {actualDistance:F2}m"
        );
    
        float timer = 0f;
        float distanceMoved = 0f;
    
        while ( timer < dashTime && distanceMoved < actualDistance)
        {
            float deltaDistance = (actualDistance / dashTime) * Time.deltaTime;
    
            deltaDistance = Mathf.Min( deltaDistance, actualDistance - distanceMoved );
    
            if (deltaDistance <= 0f) break;
    
            CollisionFlags flags = controller.Move( direction * deltaDistance );
    
            distanceMoved += deltaDistance;
            timer += Time.deltaTime;
    
            if ((flags & CollisionFlags.Sides) != 0)
            {
                Debug.Log("DASH STOPPED BY COLLISION");
                break;
            }
    
            yield return null;
        }
    
        canMove = true;
        isDashing = false;
    
        Debug.Log($"DASH END | " +$"Moved: {distanceMoved:F2}/{dashDistance:F2}");
    
        yield return new WaitForSeconds(nextDashTime);
    
        canDash = true;
        dashLockTimer = dashTime + nextDashTime;
    
        Debug.Log("DASH READY");
    }
    
    private float CalculateDashDistance( Vector3 direction, float maxDistance, LayerMask obstacleMask)
    {
        if (controller == null) return maxDistance;
    
        direction.y = 0f;
        direction.Normalize();
    
        float radius = controller.radius;
    
        float height = controller.height;
    
        Vector3 center = transform.position + controller.center;
    
        float cylinderHeight = Mathf.Max( height - radius * 2f, 0f);
    
        Vector3 point1 = center + Vector3.up * (cylinderHeight * 0.5f);
    
        Vector3 point2 = center - Vector3.up * (cylinderHeight * 0.5f);

    
        RaycastHit hit;
    
        bool blocked =
            Physics.CapsuleCast(
                point1,
                point2,
                radius,
                direction,
                out hit,
                maxDistance,
                obstacleMask,
                QueryTriggerInteraction.Ignore);
    
        if (!blocked) return maxDistance;
        
        float safeDistance = Mathf.Max(hit.distance - 0.02f,0f);
    
        Debug.Log(
            $"DASH BLOCKED | " +
            $"Obstacle: {hit.collider.name} | " +
            $"Hit Distance: {hit.distance} | " +
            $"Dash Distance: {safeDistance}"
        );
    
        return safeDistance;
    }
    public bool IsSprintingPublic() => IsSprinting();
    public CharacterController Controller => controller;
    public float CurrentSprintEnergy { get => currentSprintEnergy; set => currentSprintEnergy = value; }
    public float MaxSprintEnergy => playerCharacter.Data.CharacterStats.maxSprint;
}
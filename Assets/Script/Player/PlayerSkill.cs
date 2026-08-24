using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{
    private InputAction skillAction;
    private PlayerController playerController;

    [SerializeField] private Transform firePoint;
    [Header("Aim")]
    public float timeAuto = 0.2f;
    private BasePlayer basePlayer;
    private bool useAutoAim = true;
    private SkillBehaviour currentSkillInstance; // chỉ dùng khi cần
    private SkillData currentSkillData;

    public float targetRange = 15f;
    public GamePlayUI gameplayUI;

    private float rechargeTimer;
    private int currentStack;
    private Vector3 aimDirection = Vector3.forward;

    private Vector3 skillTargetPosition;
    private bool hasSkillTargetPosition;
    public Vector3 GetAimDirection()=> aimDirection;

private void Awake()
{
    playerController = GetComponent<PlayerController>();
    basePlayer = GetComponent<BasePlayer>();

    var playerInput = GetComponent<PlayerInput>();
    if (playerInput != null)
        skillAction = playerInput.actions["Skill"];
}

    public void SetSkillTargetPosition(Vector3 targetPosition)
    {
        skillTargetPosition = targetPosition;
    
        hasSkillTargetPosition = true;
    }
    
    
    public SkillData GetCurrentSkillData() =>currentSkillData;
    
    public bool TryGetSkillTargetPosition( out Vector3 targetPosition)
    {
        targetPosition = skillTargetPosition;
    
        return hasSkillTargetPosition;
    }
    
    public bool IsCurrentSkillGrenade()
    {
        return currentSkillData is GrenadeSkillData;
    }
    
    public void ClearSkillTargetPosition()
    {
        skillTargetPosition = Vector3.zero;
        hasSkillTargetPosition = false;
    }
    public void SetSelectedSkill(SkillData skillData)
    {
        if (skillData == null) return;
        

        currentSkillData = skillData;
        currentStack = currentSkillData.maxStack;

        if (gameplayUI != null)
            gameplayUI.InitializeSkillUI(currentSkillData.icon, currentStack, currentSkillData.maxStack);

        Debug.Log($"Đã set skill: {currentSkillData.skillName}");
    }

    public void SetGameplayUI(GamePlayUI ui)
    {
        gameplayUI = ui;

        if (gameplayUI != null && currentSkillData != null)
        {
            gameplayUI.InitializeSkillUI(currentSkillData.icon, currentStack, currentSkillData.maxStack);
            // gameplayUI.GetSkillButton().onClick.AddListener(UseSkill);
        }
    }

    public void SetAimDirection(Vector3 dir, bool autoAim)
    {
        useAutoAim = autoAim;
    
        if (dir.sqrMagnitude > 0.001f)
            aimDirection = dir.normalized;
    }
    
    public bool IsAutoAim()
    {
        return useAutoAim;
    }

    private void OnEnable()
    {
        if (skillAction != null)
            skillAction.performed += SkillPerformed;

        // if (gameplayUI != null)
        //     gameplayUI.GetSkillButton().onClick.AddListener(UseSkill);
    }

    private void OnDisable()
    {
        if (skillAction != null)
            skillAction.performed -= SkillPerformed;

        // if (gameplayUI != null)
        //     gameplayUI.GetSkillButton().onClick.RemoveListener(UseSkill);
    }

    private void Update()
    {
        RechargeStack();
    }

    private void RechargeStack()
    {
        if (currentSkillData == null) return;

        if (currentStack >= currentSkillData.maxStack)
        {
            rechargeTimer = 0;
            gameplayUI?.ShowSkillCooldown(false);
            return;
        }

        rechargeTimer += Time.deltaTime;
        gameplayUI?.UpdateSkillCooldown(rechargeTimer, currentSkillData.cooldown);

        if (rechargeTimer >= currentSkillData.cooldown)
        {
            rechargeTimer = 0;
            currentStack++;
            gameplayUI?.UpdateSkillStack(currentStack);
            gameplayUI?.ShowSkillCooldown(currentStack < currentSkillData.maxStack);
        }
    }

    private void UseSkill() => SkillPerformed(default);

private void SkillPerformed(InputAction.CallbackContext ctx)
{
    if (playerController == null || playerController.IsDead())
        return;

    if (currentSkillData == null)
        return;

    // Không còn stack
    if (currentStack <= 0)
    {
        Debug.Log("Không thể sử dụng skill: hết stack!");
        return;
    }

    if (basePlayer == null)
    {
        Debug.LogError("PlayerSkill: Không tìm thấy BasePlayer!");
        return;
    }

    // Kiểm tra mana trước khi thực hiện skill
    float manaCost = currentSkillData.skillCostMana;

    if (basePlayer.GetCurrentMana() < manaCost)
    {
        Debug.Log(
            $"Không đủ mana để dùng skill {currentSkillData.skillName}! " +
            $"Cần: {manaCost}, hiện tại: {basePlayer.GetCurrentMana()}"
        );

        return;
    }

    if (currentSkillData.skillBehaviourPrefab == null)
    {
        Debug.LogWarning("SkillBehaviourPrefab chưa được gán trong SkillData!");
        return;
    }

    // Tạo skill
    SkillBehaviour tempSkill =
        Instantiate(currentSkillData.skillBehaviourPrefab);

    // Thực thi skill
    bool success = tempSkill.Execute(this);

    Destroy(tempSkill.gameObject);

    // Chỉ trừ stack + mana khi skill thực sự thành công
    if (success)
    {
        currentStack--;

        // Trừ mana
        basePlayer.ConsumeMana(manaCost);

        gameplayUI?.UpdateSkillStack(currentStack);
        gameplayUI?.ShowSkillCooldown(
            currentStack < currentSkillData.maxStack
        );

        Debug.Log(
            $"Đã sử dụng skill: {currentSkillData.skillName} | " +
            $"Stack: {currentStack}/{currentSkillData.maxStack} | " +
            $"Mana: {basePlayer.GetCurrentMana()}"
        );
    }
}

    public void TryUseSkill()
    {
        UseSkill();
    }

    public Transform GetFirePoint() => firePoint;
}
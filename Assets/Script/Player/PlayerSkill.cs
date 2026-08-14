using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{
    private InputAction skillAction;
    private PlayerController playerController;

    [SerializeField] private Transform firePoint;
    [Header("Aim")]
    public float timeAuto = 0.2f;

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
        if (playerController.IsDead() || currentSkillData == null || currentStack <= 0) return;
        
        if (currentSkillData.skillBehaviourPrefab != null)
        {
            SkillBehaviour tempSkill = Instantiate(currentSkillData.skillBehaviourPrefab);
            bool success = tempSkill.Execute(this);
            Destroy(tempSkill.gameObject); // Xóa ngay sau khi dùng

            if (success)
            {
                currentStack--;
                gameplayUI?.UpdateSkillStack(currentStack);
                gameplayUI?.ShowSkillCooldown(currentStack < currentSkillData.maxStack);
            }
        }
        else
        {
            Debug.LogWarning("SkillBehaviourPrefab chưa được gán trong SkillData!");
        }
    }

    public void TryUseSkill()
    {
        UseSkill();
    }

    public Transform GetFirePoint() => firePoint;
}
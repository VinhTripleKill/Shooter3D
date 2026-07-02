using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{
    private InputAction skillAction;

    private PlayerController playerController;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private SkillBehaviour currentSkill;
    public float targetRange = 15f;
    private SkillData currentSkillData;
    public GamePlayUI gameplayUI;
    private float rechargeTimer;
    private int currentStack;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        var playerInput =
            GetComponent<PlayerInput>();

        skillAction =
            playerInput.actions["Skill"];
    }
 private void Start()
{
    currentSkillData = currentSkill.GetSkillData();

    currentStack = currentSkillData.maxStack;

    gameplayUI.InitializeSkillUI(
        currentSkillData.icon,
        currentStack,
        currentSkillData.maxStack);

    
}
    private void Update()
    {
        RechargeStack();
    }
    private void RechargeStack()
    {
        if (currentStack >= currentSkillData.maxStack)
        {
        rechargeTimer = 0;

        gameplayUI.ShowSkillCooldown(false);

        return;
        }

        rechargeTimer += Time.deltaTime;
        gameplayUI.UpdateSkillCooldown(rechargeTimer,currentSkillData.cooldown);
        if (rechargeTimer >= currentSkillData.cooldown)
        {
        rechargeTimer = 0;
        currentStack++;
        gameplayUI.UpdateSkillStack(currentStack);
        gameplayUI.ShowSkillCooldown(currentStack < currentSkillData.maxStack);
        Debug.Log($"Skill {currentSkillData.skillName} have {currentStack}");
        }
    }

    private void OnEnable()
    {
        skillAction.performed += SkillPerformed;
        gameplayUI.GetSkillButton()
        .onClick.AddListener(UseSkill);
    }

    private void OnDisable()
    {
        skillAction.performed -= SkillPerformed;
        gameplayUI.GetSkillButton()
            .onClick.RemoveListener(UseSkill);
    }

    private void UseSkill(){ SkillPerformed(default); }
    private void SkillPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.IsDead()) return;

        if (currentSkill == null) return;

        if (currentStack <= 0) return;

        bool success = currentSkill.Execute(this);
        if (!success) return;
        currentStack--;
        gameplayUI.UpdateSkillStack(currentStack);
        
        gameplayUI.ShowSkillCooldown(currentStack < currentSkillData.maxStack);
        Debug.Log($"Skill {currentSkillData.skillName} have {currentStack}");
    }

    public Transform GetFirePoint()
    {
        return firePoint;
    }

}
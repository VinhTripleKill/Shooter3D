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
        currentSkillData =
            ((GrenadeSkillBehaviour)currentSkill).GetSkillData();

        currentStack =
            currentSkillData.maxStack;

        Debug.Log(
            $"Skill {currentSkillData.skillName} have {currentStack}");
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
            return;
        }

        rechargeTimer += Time.deltaTime;

        if (rechargeTimer >= currentSkillData.cooldown)
        {
            rechargeTimer = 0;

            currentStack++;

            Debug.Log(
                $"Skill {currentSkillData.skillName} have {currentStack}");
        }
    }

    private void OnEnable()
    {
        skillAction.performed += SkillPerformed;
    }

    private void OnDisable()
    {
        skillAction.performed -= SkillPerformed;
    }

    private void SkillPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.IsDead())
            return;

        if (currentSkill == null)
            return;

        if (currentStack <= 0)
            return;

        currentSkill.Execute(this);

        currentStack--;

        Debug.Log(
            $"Skill {currentSkillData.skillName} have {currentStack}");
    }

    public Transform GetFirePoint()
    {
        return firePoint;
    }

}
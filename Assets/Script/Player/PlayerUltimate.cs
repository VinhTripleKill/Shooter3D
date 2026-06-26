using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerUltimate : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private GamePlayUI gameplayUI;
    private InputAction ultimateAction;
    [SerializeField]
    private float manaConsumption = 20f;

    [SerializeField]
    private float healAmount = 10f;

    [SerializeField]
    private float ultimateCd = 5f;

    private bool canUltimate = true;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        PlayerInput input = GetComponent<PlayerInput>();

        ultimateAction = input.actions["Ultimate"];
    }
    private void OnEnable()
    {
        ultimateAction.performed += UltimatePerformed;

        gameplayUI.GetUltimateButton().onClick.AddListener(UseUltimate);
    }
    private void OnDisable()
    {
        ultimateAction.performed -= UltimatePerformed;

        gameplayUI.GetUltimateButton().onClick.RemoveListener(UseUltimate);
    }
    private void UltimatePerformed(InputAction.CallbackContext ctx)
    {
        UseUltimate();
    }
    private void UseUltimate()
    {
        if (!canUltimate)
            return;

        if (!playerController.ConsumeMana(manaConsumption))
        {
            Debug.Log("Insufficient mana");

            return;
        }

        playerController.Heal(10);

        StartCoroutine(UltimateCooldown());
    }
    private IEnumerator UltimateCooldown()
    {
        canUltimate = false;

        yield return new WaitForSeconds(ultimateCd);

        canUltimate = true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

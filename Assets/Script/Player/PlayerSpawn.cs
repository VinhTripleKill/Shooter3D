using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PlayerSpawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ListCharacterManager listCharacterManager;
    [SerializeField] private ListSkillManager listSkillManager;
    [SerializeField] private VisualCharacterInfo visualCharacterInfo;
    [SerializeField] private Button battleButton;
    [SerializeField] private GameObject panelChoooseCharacter;
    private CharacterData selectedCharacter;
private SkillData selectedSkill;
    [Header("Scene References")]
    [SerializeField] private JoystickMove sceneJoystickMove;
    [SerializeField] private JoystickAttack sceneJoystickAttack;
    [SerializeField] private GamePlayUI sceneGamePlayUI;
    [SerializeField] private CameraFollow cameraFollow;        // ← THÊM

    [Header("Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnemyWaveSpawn waveManager ;
    [SerializeField] private CoreGameUI coreUI;
    private GameObject currentPlayer;

    private void Awake()
    {
        if (battleButton != null)
            battleButton.onClick.AddListener(OnBattleButtonClicked);
    }

    private void OnDestroy()
    {
        if (battleButton != null)
            battleButton.onClick.RemoveListener(OnBattleButtonClicked);
    }

    private void OnBattleButtonClicked()
{
    selectedCharacter =
        listCharacterManager.CurrentCharacter;

    selectedSkill =
        listSkillManager?.CurrentSkill;

    if (selectedCharacter == null)
        return;

    panelChoooseCharacter.SetActive(false);

    SpawnPlayer(
        selectedCharacter,
        selectedSkill
    );
}
public void ReplayGame()
{
    if (selectedCharacter == null) return;
    

    Debug.Log("REPLAY GAME");

    // Reset game time
    Time.timeScale = 1f;

    // ============================
    // XÓA ENEMY CŨ
    // ============================

    if (waveManager != null)
    {
        waveManager.ResetWaves();
    }


    if (currentPlayer != null)
    {
        Destroy(currentPlayer);
        currentPlayer = null;
    }

    if (cameraFollow != null)
    {
        cameraFollow.ClearTarget();
    }


    SpawnPlayer( selectedCharacter, selectedSkill );
}
    public void SpawnPlayer(
    CharacterData charData,
    SkillData skillData)
{
    if (
        playerPrefab == null ||
        spawnPoint == null
    )
    {
        return;
    }

    // Xóa player cũ
    if (currentPlayer != null)
    {
        Destroy(currentPlayer);
    }

    currentPlayer =
        Instantiate(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

    // ============================
    // PLAYER INPUT
    // ============================

    PlayerInput playerInput =
        currentPlayer.GetComponent<PlayerInput>();

    if (
        coreUI != null &&
        playerInput != null
    )
    {
        coreUI.RegisterPlayerInput(
            playerInput
        );
    }

    // ============================
    // CHARACTER MODEL
    // ============================

    PlayerVisualChar visualChar =
        currentPlayer.GetComponent<PlayerVisualChar>();

    if (visualChar != null)
    {
        visualChar.SpawnCharacterModel(
            charData
        );
    }

    // ============================
    // CHARACTER DATA
    // ============================

    PlayerCharacter playerChar =
        currentPlayer.GetComponent<PlayerCharacter>();

    if (playerChar != null)
    {
        playerChar.SetCharacter(
            charData
        );
    }

    // ============================
    // INITIALIZE COMPONENTS
    // ============================

    InitializePlayerComponents(
        currentPlayer,
        skillData
    );

if (cameraFollow != null)
{
    cameraFollow.SetTarget(
        currentPlayer.transform
    );
}

    if (coreUI != null)
    {
        coreUI.Initialize(
            waveManager,
            currentPlayer.GetComponent<PlayerController>()
        );

        coreUI.StartTimer();
    }

    Debug.Log(
        $"Player spawn thành công: {charData.characterName}"
    );

    // ============================
    // START WAVE
    // ============================

    waveManager?.StartNextWave();
}
    private void InitializePlayerComponents(GameObject player, SkillData selectedSkill)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.InitializeSceneReferences( sceneJoystickMove, sceneGamePlayUI, coreUI, waveManager );

        // PlayerWeapon
        PlayerWeapon weapon = player.GetComponent<PlayerWeapon>();
        if (weapon != null)
            weapon.SetGameplayUI(sceneGamePlayUI);

        // === LIÊN KẾT JOYSTICK ATTACK ===
        if (sceneJoystickAttack != null && weapon != null)
        {
            sceneJoystickAttack.SetPlayerWeapon(weapon);
            Debug.Log("Đã liên kết JoystickAttack với PlayerWeapon");
        }

        // Các component khác...
        PlayerUltimate ultimate = player.GetComponent<PlayerUltimate>();
        if (ultimate != null)
        {
            ultimate.InitializeUltimate();
            ultimate.SetGameplayUI(sceneGamePlayUI);
        }

        PlayerSkill skillComp = player.GetComponent<PlayerSkill>();
        if (skillComp != null)
        {
            skillComp.SetSelectedSkill(selectedSkill);
            skillComp.SetGameplayUI(sceneGamePlayUI);
        }

        PlayerSprint sprint = player.GetComponent<PlayerSprint>();
        if (sprint != null) sprint.SetGameplayUI(sceneGamePlayUI);

        PlayerInteraction interaction = player.GetComponent<PlayerInteraction>();
        if (interaction != null) interaction.SetGameplayUI(sceneGamePlayUI);

        Debug.Log("Đã gán GamePlayUI và khởi tạo các hệ thống");
    }
}
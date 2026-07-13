using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ListCharacterManager listCharacterManager;
    [SerializeField] private ListSkillManager listSkillManager;
    [SerializeField] private VisualCharacterInfo visualCharacterInfo;
    [SerializeField] private Button battleButton;
    [SerializeField] private GameObject panelChoooseCharacter;

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
        CharacterData selectedChar = listCharacterManager.CurrentCharacter;
        SkillData selectedSkill = listSkillManager?.CurrentSkill;

        if (selectedChar == null) return;

        panelChoooseCharacter.SetActive(false);
        SpawnPlayer(selectedChar, selectedSkill);
    }

    public void SpawnPlayer(CharacterData charData, SkillData skillData)
{
    if (playerPrefab == null || spawnPoint == null) return;

    if (currentPlayer != null)
        Destroy(currentPlayer);

    currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

    // Spawn Model + Set Data...
    PlayerVisualChar visualChar = currentPlayer.GetComponent<PlayerVisualChar>();
    if (visualChar != null)
        visualChar.SpawnCharacterModel(charData);

    PlayerCharacter playerChar = currentPlayer.GetComponent<PlayerCharacter>();
    if (playerChar != null)
        playerChar.SetCharacter(charData);

    InitializePlayerComponents(currentPlayer, skillData);

    if (cameraFollow != null)
        cameraFollow.SetTarget(currentPlayer.transform);

    // === KHỞI TẠO CORE GAME UI ===
    if (coreUI != null)
    {
        coreUI.Initialize(waveManager, currentPlayer.GetComponent<PlayerController>());
        coreUI.StartTimer();
    }

    Debug.Log($"Player spawn thành công: {charData.characterName}");

    // CHỈ GỌI 1 LẦN
    waveManager?.StartNextWave();
}
    private void InitializePlayerComponents(GameObject player, SkillData selectedSkill)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.InitializeSceneReferences(sceneJoystickMove, sceneGamePlayUI);

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
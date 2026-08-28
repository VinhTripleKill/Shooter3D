using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private JoystickMove sceneJoystickMove;
    [SerializeField] private JoystickAttack sceneJoystickAttack;
    [SerializeField] private GamePlayUI sceneGamePlayUI;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnemyWaveSpawn waveManager;
    [SerializeField] private CoreGameUI coreUI;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private SkillNavigationArea skillNav;
    [SerializeField] private SkillJoystickHandle joystickHandle;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;

    private GameObject currentPlayer;

    private void Start()
    {
        SpawnFromTransferData();
    }

    private void SpawnFromTransferData()
    {
        if (TransferDataEquip.Instance == null ||
            !TransferDataEquip.Instance.HasData)
        {
            Debug.LogError("PlayerSpawn | Không có dữ liệu Player.");
            return;
        }

        SpawnPlayer(
            TransferDataEquip.Instance.SelectedCharacter,
            TransferDataEquip.Instance.SelectedSkill,
            TransferDataEquip.Instance.SelectedGun
        );
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
    
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnPlayer(CharacterData character, SkillData skill, GunData gun)
    {
        if (playerPrefab == null || spawnPoint == null)
        {
            Debug.LogError("PlayerSpawn | Chưa gán PlayerPrefab hoặc SpawnPoint.");
            return;
        }

        if (currentPlayer != null)
            Destroy(currentPlayer);

        currentPlayer = Instantiate(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        PlayerInput input = currentPlayer.GetComponent<PlayerInput>();
        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        PlayerWeapon weapon = currentPlayer.GetComponent<PlayerWeapon>();
        PlayerSkill skillComp = currentPlayer.GetComponent<PlayerSkill>();
        PlayerProgress progress = currentPlayer.GetComponent<PlayerProgress>();

        PlayerVisualChar visual = currentPlayer.GetComponent<PlayerVisualChar>();
        PlayerCharacter playerCharacter = currentPlayer.GetComponent<PlayerCharacter>();
        PlayerSprint sprint = currentPlayer.GetComponent<PlayerSprint>();
        PlayerInteraction interaction = currentPlayer.GetComponent<PlayerInteraction>();

        if (visual != null)
            visual.SpawnCharacterModel(character);

        if (playerCharacter != null)
            playerCharacter.SetCharacter(character);

        if (input != null && coreUI != null)
            coreUI.RegisterPlayerInput(input);

        if (controller != null)
        {
            controller.InitializeSceneReferences(
                sceneJoystickMove,
                sceneGamePlayUI,
                coreUI,
                waveManager
            );
        }

        if (weapon != null)
        {
            weapon.SetGameplayUI(sceneGamePlayUI);

            if (gun != null)
                weapon.EquipStartingGun(gun);

            sceneJoystickAttack?.SetPlayerWeapon(weapon);
        }

        if (skillComp != null)
        {
            skillComp.SetSelectedSkill(skill);
            skillComp.SetGameplayUI(sceneGamePlayUI);
        }

        if (progress != null && sceneGamePlayUI != null)
            sceneGamePlayUI.SetPlayerProgress(progress);

        SetupSkillUI(skillComp, skill);

        if (sprint != null)
            sprint.SetGameplayUI(sceneGamePlayUI);

        if (interaction != null)
            interaction.SetGameplayUI(sceneGamePlayUI);

        cameraFollow?.SetTarget(currentPlayer.transform);

        if (coreUI != null)
        {
            coreUI.Initialize(waveManager, controller);
            coreUI.StartTimer();
        }

        waveManager?.StartNextWave();

        Debug.Log(
            $"PlayerSpawn | Spawn: {character?.characterName} | " +
            $"Skill: {skill?.skillName}"
        );
    }

    private void SetupSkillUI(PlayerSkill skillComp, SkillData skill)
    {
        if (skillComp == null || skillNav == null)
            return;

        skillNav.SetPlayerSkill(skillComp);
        skillNav.SetCurrentSkill(skill);

        SkillIndicatorUI indicator =
            currentPlayer.GetComponentInChildren<SkillIndicatorUI>(true);

        if (indicator == null)
            return;

        skillNav.SetSkillIndicator(indicator);
        skillNav.SetJoystickHandle(joystickHandle);

        indicator.SetPlayerTransform(currentPlayer.transform);
        indicator.SetSkillData(skill);

        joystickHandle?.SetIndicator(indicator);
    }
}
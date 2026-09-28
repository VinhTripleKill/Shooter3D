
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    // =====================================================
    // SCENE REFERENCES
    // =====================================================

    [Header("Scene References")]

    [SerializeField]
    private JoystickMove sceneJoystickMove;

    [SerializeField]
    private JoystickAttack sceneJoystickAttack;

    [SerializeField]
    private GamePlayUI sceneGamePlayUI;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private EnemyWaveSpawn waveManager;

    [SerializeField]
    private CoreGameUI coreUI;

    [SerializeField]
    private CameraFollow cameraFollow;

    [SerializeField]
    private SkillNavigationArea skillNav;

    [SerializeField]
    private SkillJoystickHandle joystickHandle;

    // =====================================================
    // SPAWN SETTINGS
    // =====================================================

    [Header("Spawn Settings")]

    [SerializeField]
    private GameObject playerPrefab;

    // =====================================================
    // CURRENT PLAYER
    // =====================================================

    private GameObject currentPlayer;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        SpawnFromTransferData();
    }

    // =====================================================
    // SPAWN FROM TRANSFER DATA
    // =====================================================

    private void SpawnFromTransferData()
    {
        if (
            TransferDataEquip.Instance == null ||
            !TransferDataEquip.Instance.HasData
        )
        {
            Debug.LogError(
                "PlayerSpawn | Không có dữ liệu Player."
            );

            return;
        }

        SpawnPlayer(
            TransferDataEquip.Instance.SelectedCharacter,
            TransferDataEquip.Instance.SelectedSkill,
            TransferDataEquip.Instance.SelectedGun
        );
    }

    // =====================================================
    // REPLAY
    // =====================================================

    public void ReplayGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    // =====================================================
    // SPAWN PLAYER
    // =====================================================

    public void SpawnPlayer(
        CharacterData character,
        SkillData skill,
        GunData gun)
    {
        // ---------------------------------------------
        // CHECK
        // ---------------------------------------------

        if (
            playerPrefab == null ||
            spawnPoint == null
        )
        {
            Debug.LogError(
                "PlayerSpawn | Chưa gán PlayerPrefab hoặc SpawnPoint."
            );

            return;
        }

        if (character == null)
        {
            Debug.LogError(
                "PlayerSpawn | CharacterData null."
            );

            return;
        }

        if (character.CharacterStats == null)
        {
            Debug.LogError(
                "PlayerSpawn | CharacterStats null."
            );

            return;
        }

        // ---------------------------------------------
        // DESTROY OLD PLAYER
        // ---------------------------------------------

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        // ---------------------------------------------
        // INSTANTIATE
        // ---------------------------------------------

        currentPlayer =
            Instantiate(
                playerPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        // =================================================
        // COMPONENTS
        // =================================================

        PlayerInput input =
            currentPlayer.GetComponent<PlayerInput>();

        PlayerController controller =
            currentPlayer.GetComponent<PlayerController>();

        PlayerWeapon weapon =
            currentPlayer.GetComponent<PlayerWeapon>();

        PlayerShoot shoot =
            currentPlayer.GetComponent<PlayerShoot>();

        PlayerReload reload =
            currentPlayer.GetComponent<PlayerReload>();

        PlayerSkill skillComp =
            currentPlayer.GetComponent<PlayerSkill>();

        PlayerProgress progress =
            currentPlayer.GetComponent<PlayerProgress>();

        PlayerVisualChar visual =
            currentPlayer.GetComponent<PlayerVisualChar>();

        PlayerCharacter playerCharacter =
            currentPlayer.GetComponent<PlayerCharacter>();

        PlayerSprint sprint =
            currentPlayer.GetComponent<PlayerSprint>();

        PlayerInteraction interaction =
            currentPlayer.GetComponent<PlayerInteraction>();
        PlayerAmmo ammo =
    currentPlayer.GetComponent<PlayerAmmo>();
        // =================================================
        // CHARACTER
        // =================================================

        if (visual != null)
        {
            visual.SpawnCharacterModel(
                character
            );
        }

        if (playerCharacter != null)
        {
            playerCharacter.SetCharacter(
                character
            );
        }

        // =================================================
        // CHARACTER COMBAT STATS
        // =================================================

        if (weapon != null)
        {
            weapon.SetCharacterStats(
                character.CharacterStats
            );

            Debug.Log(
                $"PlayerSpawn | Character Stats Loaded | " +
                $"CritRate: {character.CharacterStats.critRate} | " +
                $"CritDamage: {character.CharacterStats.critDamage}"
            );
        }
        if (ammo != null)
{
    ammo.SetGameplayUI(
        sceneGamePlayUI
    );
}

        // =================================================
        // CHARACTER ATTACK RANGE
        // =================================================

        if (shoot != null)
        {
            float attackRange =
                character.CharacterStats.attackRange;

            shoot.SetAutoAimRange(
                attackRange
            );

            Debug.Log(
                $"PlayerSpawn | Character: " +
                $"{character.characterName} | " +
                $"AttackRange: {attackRange}"
            );
        }

        // =================================================
        // PLAYER INPUT
        // =================================================

        if (
            input != null &&
            coreUI != null
        )
        {
            coreUI.RegisterPlayerInput(
                input
            );
        }

        // =================================================
        // PLAYER CONTROLLER
        // =================================================

        if (controller != null)
        {
            controller.InitializeSceneReferences(
                sceneJoystickMove,
                sceneGamePlayUI,
                coreUI,
                waveManager
            );
        }

        // =================================================
        // PLAYER WEAPON
        // =================================================

        if (weapon != null)
        {
            weapon.SetGameplayUI(
                sceneGamePlayUI
            );

            if (gun != null)
            {
                weapon.EquipStartingGun(
                    gun
                );
            }

            sceneJoystickAttack?.SetPlayerWeapon(
                weapon
            );
        }

        // =================================================
        // PLAYER SHOOT
        // =================================================

        if (shoot != null)
        {
            sceneJoystickAttack?.SetPlayerShoot(
                shoot
            );
        }

        // =================================================
        // PLAYER RELOAD
        // =================================================

        if (reload != null)
        {
            sceneJoystickAttack?.SetPlayerReload(
                reload
            );

            if (sceneGamePlayUI != null)
            {
                reload.SetReloadButton(
                    sceneGamePlayUI.GetReloadButton()
                );
            }
        }

        // =================================================
        // PLAYER SKILL
        // =================================================

        if (skillComp != null)
        {
            skillComp.SetSelectedSkill(
                skill
            );

            skillComp.SetGameplayUI(
                sceneGamePlayUI
            );
        }

        // =================================================
        // PLAYER PROGRESS
        // =================================================

        if (
            progress != null &&
            sceneGamePlayUI != null
        )
        {
            sceneGamePlayUI.SetPlayerProgress(
                progress
            );
        }

        // =================================================
        // SKILL UI
        // =================================================

        SetupSkillUI(
            skillComp,
            skill
        );

        // =================================================
        // SPRINT
        // =================================================

        if (sprint != null)
        {
            sprint.SetGameplayUI(
                sceneGamePlayUI
            );
        }

        // =================================================
        // INTERACTION
        // =================================================

        if (interaction != null)
        {
            interaction.SetGameplayUI(
                sceneGamePlayUI
            );
        }

        // =================================================
        // CAMERA
        // =================================================

        cameraFollow?.SetTarget(
            currentPlayer.transform
        );

        // =================================================
        // CORE UI
        // =================================================

        if (coreUI != null)
        {
            coreUI.Initialize(
                waveManager,
                controller
            );

            coreUI.StartTimer();
        }

        // =================================================
        // WAVE
        // =================================================

        waveManager?.StartNextWave();

        // =================================================
        // DEBUG
        // =================================================

        Debug.Log(
            $"PlayerSpawn | Spawn: " +
            $"{character.characterName} | " +
            $"Skill: {skill?.skillName} | " +
            $"AttackRange: {character.CharacterStats.attackRange} | " +
            $"CritRate: {character.CharacterStats.critRate} | " +
            $"CritDamage: {character.CharacterStats.critDamage}"
        );
    }

    // =====================================================
    // SKILL UI
    // =====================================================

    private void SetupSkillUI(
        PlayerSkill skillComp,
        SkillData skill)
    {
        if (
            skillComp == null ||
            skillNav == null
        )
        {
            return;
        }

        skillNav.SetPlayerSkill(
            skillComp
        );

        skillNav.SetCurrentSkill(
            skill
        );

        SkillIndicatorUI indicator =
            currentPlayer.GetComponentInChildren<
                SkillIndicatorUI
            >(true);

        if (indicator == null)
            return;

        skillNav.SetSkillIndicator(
            indicator
        );

        skillNav.SetJoystickHandle(
            joystickHandle
        );

        indicator.SetPlayerTransform(
            currentPlayer.transform
        );

        indicator.SetSkillData(
            skill
        );

        joystickHandle?.SetIndicator(
            indicator
        );
    }
}
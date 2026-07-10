using UnityEngine;
using UnityEngine.UI;

public class PlayerSpawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ListCharacterManager listCharacterManager;
    [SerializeField] private ListSkillManager listSkillManager;        // ← THÊM
    [SerializeField] private VisualCharacterInfo visualCharacterInfo;
    [SerializeField] private Button battleButton;

    [Header("Scene References")]
    [SerializeField] private JoystickMove sceneJoystickMove;
    [SerializeField] private GamePlayUI sceneGamePlayUI;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

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

        if (selectedChar == null)
        {
            Debug.LogWarning("Chưa chọn nhân vật!");
            return;
        }

        SpawnPlayer(selectedChar, selectedSkill);
    }

    // Sửa thành nhận thêm SkillData
    public void SpawnPlayer(CharacterData charData, SkillData skillData)
    {
        if (playerPrefab == null || spawnPoint == null)
        {
            Debug.LogError("PlayerPrefab hoặc SpawnPoint chưa được gán!");
            return;
        }

        if (sceneJoystickMove == null || sceneGamePlayUI == null)
        {
            Debug.LogError("Chưa gán JoystickMove hoặc GamePlayUI!");
            return;
        }

        if (currentPlayer != null)
            Destroy(currentPlayer);

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // Spawn Model
        PlayerVisualChar visualChar = currentPlayer.GetComponent<PlayerVisualChar>();
        if (visualChar != null)
            visualChar.SpawnCharacterModel(charData);

        // Set CharacterData
        PlayerCharacter playerChar = currentPlayer.GetComponent<PlayerCharacter>();
        if (playerChar != null)
            playerChar.SetCharacter(charData);

        // === GÁN TẤT CẢ REFERENCES ===
        InitializePlayerComponents(currentPlayer, skillData);

        Debug.Log($"Player spawn thành công: {charData.characterName} | Skill: {(skillData?.skillName ?? "None")}");
    }

    private void InitializePlayerComponents(GameObject player, SkillData selectedSkill)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
            controller.InitializeSceneReferences(sceneJoystickMove, sceneGamePlayUI);

        if (sceneGamePlayUI == null) return;

        // PlayerUltimate
        PlayerUltimate ultimate = player.GetComponent<PlayerUltimate>();
        if (ultimate != null)
        {
            ultimate.InitializeUltimate();
            ultimate.SetGameplayUI(sceneGamePlayUI);
        }

        // PlayerSkill - Truyền skill được chọn
        PlayerSkill skillComp = player.GetComponent<PlayerSkill>();
        if (skillComp != null)
        {
            skillComp.SetSelectedSkill(selectedSkill);
            skillComp.SetGameplayUI(sceneGamePlayUI);
        }

        // Các component khác
        PlayerWeapon weapon = player.GetComponent<PlayerWeapon>();
        if (weapon != null) weapon.SetGameplayUI(sceneGamePlayUI);

        PlayerSprint sprint = player.GetComponent<PlayerSprint>();
        if (sprint != null) sprint.SetGameplayUI(sceneGamePlayUI);

        PlayerInteraction interaction = player.GetComponent<PlayerInteraction>();
        if (interaction != null) interaction.SetGameplayUI(sceneGamePlayUI);

        Debug.Log("Đã gán GamePlayUI và khởi tạo các hệ thống");
    }
}
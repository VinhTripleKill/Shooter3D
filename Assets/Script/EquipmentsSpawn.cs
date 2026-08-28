using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EquipmentsSpawn : MonoBehaviour
{
    [Header("Visual ModelChar")]
    [SerializeField] private Transform modelShow;

    [Header("References")]
    [SerializeField] private ListCharacterManager listCharacterManager;
    [SerializeField] private ListSkillManager listSkillManager;
    [SerializeField] private ListGunManager listGunManagr;
    [SerializeField] private VisualCharacterInfo visualCharacterInfo;
    [SerializeField] private Button battleButton;

    private GameObject currentModel;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (battleButton != null)
        {
            battleButton.onClick.AddListener(
                OnBattleButtonClicked
            );
        }

        if (listCharacterManager != null)
        {
            listCharacterManager.OnCharacterChanged +=
                ShowCharacterModel;
        }
    }

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        /*
         * Trong trường hợp ListCharacterManager đã chọn
         * character đầu tiên trước khi EquipmentsSpawn
         * đăng ký event, ta kiểm tra lại CurrentCharacter.
         */

        if (listCharacterManager != null)
        {
            CharacterData currentCharacter =
                listCharacterManager.CurrentCharacter;

            if (currentCharacter != null)
            {
                ShowCharacterModel(
                    currentCharacter
                );
            }
        }
    }

    // =====================================================
    // DESTROY
    // =====================================================

    private void OnDestroy()
    {
        if (battleButton != null)
        {
            battleButton.onClick.RemoveListener(
                OnBattleButtonClicked
            );
        }

        if (listCharacterManager != null)
        {
            listCharacterManager.OnCharacterChanged-=
                ShowCharacterModel;
        }
    }

    // =====================================================
    // SHOW CHARACTER MODEL
    // =====================================================

    private void ShowCharacterModel(
        CharacterData characterData)
    {
        if (modelShow == null)
        {
            Debug.LogError(
                "EquipmentsSpawn | modelShow chưa được gán!"
            );

            return;
        }

        // Không có CharacterData
        if (characterData == null)
        {
            ClearCharacterModel();
            return;
        }

        // Không có model
        if (characterData.modelPrefab == null)
        {
            Debug.LogWarning(
                $"EquipmentsSpawn | " +
                $"Character '{characterData.characterName}' " +
                $"chưa có modelPrefab!"
            );

            ClearCharacterModel();
            return;
        }

        // =================================================
        // DESTROY MODEL CŨ
        // =================================================

        ClearCharacterModel();

        // =================================================
        // SPAWN MODEL MỚI
        // =================================================

        currentModel = Instantiate(
            characterData.modelPrefab,
            modelShow
        );

        // =================================================
        // RESET LOCAL TRANSFORM
        // =================================================

        currentModel.transform.localPosition =
            Vector3.zero;

        currentModel.transform.localRotation =
            Quaternion.identity;

        currentModel.transform.localScale =
            Vector3.one;

        Debug.Log(
            $"EquipmentsSpawn | " +
            $"Đã hiển thị model: " +
            $"{characterData.characterName}"
        );
    }

    // =====================================================
    // CLEAR MODEL
    // =====================================================

    private void ClearCharacterModel()
    {
        if (currentModel == null)
            return;

        Destroy(currentModel);

        currentModel = null;
    }

    // =====================================================
    // BATTLE
    // =====================================================

    private void OnBattleButtonClicked()
    {
        CharacterData selectedCharacter =
            listCharacterManager != null
                ? listCharacterManager.CurrentCharacter
                : null;

        SkillData selectedSkill =
            listSkillManager != null
                ? listSkillManager.CurrentSkill
                : null;

        GunData selectedGun =
            listGunManagr != null
                ? listGunManagr.CurrentGun
                : null;

        // =================================================
        // CHECK CHARACTER
        // =================================================

        if (selectedCharacter == null)
        {
            Debug.LogWarning(
                "EquipmentsSpawn | Chưa chọn Character!"
            );

            return;
        }

        // =================================================
        // CHECK TRANSFER DATA
        // =================================================

        if (TransferDataEquip.Instance == null)
        {
            Debug.LogError(
                "EquipmentsSpawn | " +
                "Không tìm thấy TransferDataEquip!"
            );

            return;
        }

        // =================================================
        // SAVE DATA
        // =================================================

        TransferDataEquip.Instance.SetData(
            selectedCharacter,
            selectedSkill,
            selectedGun
        );

        Debug.Log(
            $"EquipmentsSpawn | Battle -> " +
            $"Character: {selectedCharacter.characterName}"
        );

        // =================================================
        // LOAD GAMEPLAY
        // =================================================

        SceneManager.LoadScene(
            "GamePlay"
        );
    }
}
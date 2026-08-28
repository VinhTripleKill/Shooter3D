using UnityEngine;

public class TransferDataEquip : MonoBehaviour
{
    public static TransferDataEquip Instance { get; private set; }

    private CharacterData selectedCharacter;
    private SkillData selectedSkill;
    private GunData selectedGun;

    public CharacterData SelectedCharacter => selectedCharacter;
    public SkillData SelectedSkill => selectedSkill;
    public GunData SelectedGun => selectedGun;

    public bool HasData => selectedCharacter != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Lưu lựa chọn Character + Skill + Gun
    /// </summary>
    public void SetData(
        CharacterData character,
        SkillData skill,
        GunData gun)
    {
        selectedCharacter = character;
        selectedSkill = skill;
        selectedGun = gun;

        Debug.Log(
            $"TransferDataEquip | " +
            $"Character = {selectedCharacter?.characterName}, " +
            $"Skill = {selectedSkill?.skillName}, " +
            $"Gun = {selectedGun?.gunName}"
        );
    }

    /// <summary>
    /// Xóa dữ liệu khi muốn bắt đầu một lựa chọn hoàn toàn mới.
    /// Không cần gọi khi Replay.
    /// </summary>
    public void ClearData()
    {
        selectedCharacter = null;
        selectedSkill = null;
        selectedGun = null;

        Debug.Log("TransferDataEquip | Data đã được clear.");
    }
}

using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class VisualCharacterInfo : MonoBehaviour
{
    [Header("Character Information")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI hp_text;
    [SerializeField] private TextMeshProUGUI moveSpeed_text;
    [SerializeField] private TextMeshProUGUI manaMax_text;
    [SerializeField] private TextMeshProUGUI sprintEnergy_text;
    [SerializeField] private TextMeshProUGUI damageChar_text;
    [SerializeField] private TextMeshProUGUI critRate_text;
    [SerializeField] private TextMeshProUGUI critDamage_text;
    [SerializeField] private TextMeshProUGUI defense_text;
    [SerializeField] private TextMeshProUGUI descriptionCharacter;

    [Header ("Skill")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName_text;
    [SerializeField] private TextMeshProUGUI skillStack_text;
    [SerializeField] private TextMeshProUGUI skillCD_text;
    [SerializeField] private Button SkillChooseB;
    [SerializeField] private GameObject listSkill;
    [SerializeField] private ListSkillManager skillManager;
    [Header("Gun")]
    [SerializeField] private Image GunIcon;
    [SerializeField] private TextMeshProUGUI gunName_text;
    [SerializeField] private TextMeshProUGUI damageGun_text;
    [SerializeField] private TextMeshProUGUI countShot_text;
    [SerializeField] private TextMeshProUGUI reloadTime_text;
    [SerializeField] private TextMeshProUGUI manaRecor_text;
    [SerializeField] private Button gunChooseB;
    [SerializeField] private GameObject listGun;
    [SerializeField] private ListGunManager gunManager;
    private void Awake()
{
    if (listSkill != null)
        listSkill.SetActive(false);

    if (listGun != null)
        listGun.SetActive(false);

    SkillChooseB.onClick.AddListener(ToggleSkillList);
    gunChooseB.onClick.AddListener(ToggleGunList);
}

    private void OnDestroy()
{
    SkillChooseB.onClick.RemoveListener(ToggleSkillList);
    gunChooseB.onClick.RemoveListener(ToggleGunList);
}
    private void ToggleGunList()
{
    if (listGun == null)
        return;

    listGun.SetActive(!listGun.activeSelf);
}
private void ToggleSkillList()
    {
        if (listSkill == null) return;
    
        listSkill.SetActive(!listSkill.activeSelf);
    }
    public void ShowCharacter(CharacterData data){
    

    characterName.text = data.characterName;

    hp_text.text = data.CharacterStats.maxHp.ToString();

    moveSpeed_text.text = data.CharacterStats.moveSpeed.ToString();

    manaMax_text.text = data.CharacterStats.maxMana.ToString();

    sprintEnergy_text.text = data.CharacterStats.maxSprint.ToString();

    damageChar_text.text = data.CharacterStats.damage.ToString();

    critRate_text.text = data.CharacterStats.critRate + "%";

    critDamage_text.text = data.CharacterStats.critDamage + "%";

    defense_text.text = data.CharacterStats.defense.ToString();

    descriptionCharacter.text =data.descriptionCharacter;

    }
    public void ShowSkill(SkillData data)
    {
        if (data == null)
        {
            skillIcon.sprite = null;
            skillName_text.text = "";
            skillStack_text.text = "";
            skillCD_text.text = "";
            return;
        }
    
        skillIcon.sprite = data.icon;
    
        skillName_text.text = data.skillName;
    
        //skillStack_text.text = $"{data.maxStack}";
    
        //skillCD_text.text = $"{data.cooldown:0.0}s";
    }
    public void ShowGun(GunData data)
{
    if (data == null)
    {
        GunIcon.sprite = null;

        gunName_text.text = "";
        damageGun_text.text = "";
        countShot_text.text = "";
        reloadTime_text.text = "";
        manaRecor_text.text = "";

        return;
    }

    GunIcon.sprite = data.icon;

    gunName_text.text = data.gunName;

    //damageGun_text.text = data.damage.ToString();

    //countShot_text.text = data.maxCountShot.ToString();

    //reloadTime_text.text = $"{data.timeReload:0.0}s";

    //manaRecor_text.text = data.manaRecoveryByHit.ToString();
}
}

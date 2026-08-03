
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

    [Header ("Ultimate character")]
    [SerializeField] private Image ultimateIcon;
    [SerializeField] private TextMeshProUGUI ultimateName;
    [SerializeField] private TextMeshProUGUI manaCost_text;
    [SerializeField] private TextMeshProUGUI ultimateCD_text;
    [SerializeField] private TextMeshProUGUI descriptionUltimate;
    
    [Header ("Skill")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName_text;
    [SerializeField] private TextMeshProUGUI skillStack_text;
    [SerializeField] private TextMeshProUGUI skillCD_text;
    [Header("Gun")]
    [SerializeField] private Image GunIcon;
    [SerializeField] private TextMeshProUGUI gunName_text;
    [SerializeField] private TextMeshProUGUI damageGun_text;
    [SerializeField] private TextMeshProUGUI countShot_text;
    [SerializeField] private TextMeshProUGUI reloadTime_text;
    [SerializeField] private TextMeshProUGUI manaRecor_text;
    [Header ("Button")]
    [SerializeField] private Button SkillChooseB;
    [SerializeField] private GameObject listSkill;
    [SerializeField]
private ListSkillManager skillManager;
    private void Awake()
    {
        if (listSkill != null)
            listSkill.SetActive(false);
    
        SkillChooseB.onClick.AddListener(ToggleSkillList);
    }

    private void OnDestroy()
    {
        SkillChooseB.onClick.RemoveListener(ToggleSkillList);
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
    
        skillStack_text.text = $"{data.maxStack}";
    
        skillCD_text.text = $"{data.cooldown:0.0}s";
    }
}

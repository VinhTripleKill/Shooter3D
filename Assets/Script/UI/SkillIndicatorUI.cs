
using UnityEngine;
using UnityEngine.UI;

public class SkillIndicatorUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject skillIndicatorCanvas;
    [SerializeField] private Image rangeCircleSkill;
    [SerializeField] private Image dashIndicator;
    [SerializeField] private Image gradeIndicator;

    [Header("Range Visual")]
    [SerializeField] private float pixelsPerMeter = 100f;

    private void Awake()
    {
        HideIndicator();
    }

    // =========================================================
    // SET SKILL DATA
    // =========================================================

    public void SetSkillData(SkillData skillData)
    {
        if (skillData == null)
        {
            Debug.LogWarning(
                "SkillIndicatorUI: SkillData is null!"
            );

            return;
        }

        if (rangeCircleSkill == null)
        {
            Debug.LogWarning(
                "SkillIndicatorUI: rangeCircleSkill chưa được gán!"
            );

            return;
        }

        // =====================================================
        // TÍNH KÍCH THƯỚC VÒNG TRÒN
        //
        // 1 unit range = 100 pixel
        // Vì rangeRadius là bán kính
        // nên Width / Height = radius * 100 * 2
        // =====================================================

        float diameter =
            skillData.rangeRadius *
            pixelsPerMeter *
            2f;

        rangeCircleSkill.rectTransform.sizeDelta =
            new Vector2(diameter, diameter);

        Debug.Log(
            $"Skill Indicator | " +
            $"Skill: {skillData.skillName} | " +
            $"Range Radius: {skillData.rangeRadius} | " +
            $"Indicator Size: {diameter} x {diameter}"
        );
    }

    // =========================================================
    // SHOW / HIDE
    // =========================================================

    public void ShowIndicator()
    {
        if (skillIndicatorCanvas != null)
            skillIndicatorCanvas.SetActive(true);
    }

    public void HideIndicator()
    {
        if (skillIndicatorCanvas != null)
            skillIndicatorCanvas.SetActive(false);
    }

    public GameObject GetIndicatorCanvas()
    {
        return skillIndicatorCanvas;
    }
}

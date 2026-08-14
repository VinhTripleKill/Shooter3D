using UnityEngine;
using UnityEngine.UI;

public class SkillIndicatorUI : MonoBehaviour
{
    [SerializeField] private GameObject skillIndicatorCanvas;
    [SerializeField] private Image rangeCircleSkill;
    [SerializeField] private DashIndicatorUI dashIndicator;
    [SerializeField] private GrenadeIndicatorUI grenadeIndicator;

    [Header("Range Visual")]
    [SerializeField] private float pixelsPerMeter = 100f;

    private Transform playerTransform;
    private SkillData currentSkillData;

    public SkillData CurrentSkillData => currentSkillData;

    private void Awake() => HideIndicator();

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
        dashIndicator?.SetPlayerTransform(player);
        grenadeIndicator?.SetPlayerTransform(player);
    }

    public void SetSkillData(SkillData skillData)
    {
        currentSkillData = skillData;

        if (skillData == null)
        {
            HideIndicator();
            return;
        }

        UpdateRangeCircle(skillData);

        dashIndicator?.SetPixelsPerMeter(pixelsPerMeter);
        grenadeIndicator?.SetPixelsPerMeter(pixelsPerMeter);

        dashIndicator?.SetSkillData(skillData);
        grenadeIndicator?.SetSkillData(skillData);
    }

    private void UpdateRangeCircle(SkillData skillData)
    {
        if (rangeCircleSkill == null) return;

        float diameter = skillData.rangeRadius * pixelsPerMeter * 2f;
        rangeCircleSkill.rectTransform.sizeDelta = new Vector2(diameter, diameter);
    }

    public void ShowIndicator()
    {
        skillIndicatorCanvas?.SetActive(true);

        if (currentSkillData is DashSkillData)
            dashIndicator?.Show();
    }

    public void HideIndicator()
    {
        skillIndicatorCanvas?.SetActive(false);
        dashIndicator?.Hide();
        grenadeIndicator?.Hide();
    }

    public void ShowAutoPreview()
    {
        if (currentSkillData is DashSkillData)
            dashIndicator?.SetAutoDirection();
        else if (currentSkillData is GrenadeSkillData)
            grenadeIndicator?.ShowAutoPreview();
    }

    public void UpdateDirection(Vector2 direction, float distance)
    {
        if (currentSkillData is DashSkillData)
            dashIndicator?.SetDirection(direction);
        else if (currentSkillData is GrenadeSkillData)
            grenadeIndicator?.SetDirection(direction, distance);
    }

    public bool TryGetGrenadeTarget(out Vector3 target)
    {
        target = Vector3.zero;

        if (currentSkillData is not GrenadeSkillData || grenadeIndicator == null)
            return false;

        return grenadeIndicator.TryGetTargetPosition(out target);
    }

    public GameObject GetIndicatorCanvas() => skillIndicatorCanvas;
    public void SetAutoDashDirection() => dashIndicator?.SetAutoDirection();
    public void SetDashDirection(Vector2 direction) => dashIndicator?.SetDirection(direction);
}
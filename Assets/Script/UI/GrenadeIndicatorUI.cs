using UnityEngine;
using UnityEngine.UI;

public class GrenadeIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image grenadeIndicator;
    [SerializeField] private float grenadeIndicatorYOffset;

    private Transform playerTransform;
    private GrenadeSkillData grenadeSkillData;
    private Vector3 grenadeTargetPosition;
    private bool hasGrenadeTargetPosition;
    private float pixelsPerMeter = 100f;

    public void SetPlayerTransform(Transform player) => playerTransform = player;

    public void SetPixelsPerMeter(float value) => pixelsPerMeter = value;

    public void SetSkillData(SkillData skillData)
    {
        grenadeSkillData = skillData as GrenadeSkillData;

        if (grenadeSkillData == null)
        {
            Hide();
            return;
        }

        float diameter = grenadeSkillData.explodeRadius * pixelsPerMeter * 2f;
        grenadeIndicator.rectTransform.sizeDelta = new Vector2(diameter, diameter);
        Hide();
    }

    public void Show() => grenadeIndicator?.gameObject.SetActive(true);

    public void Hide()
    {
        grenadeIndicator?.gameObject.SetActive(false);
        ClearTarget();
    }

    public void ShowAutoPreview()
    {
        if (grenadeSkillData == null || playerTransform == null)
            return;

        Transform firePoint = GetFirePoint();
        if (firePoint == null) return;

        Transform target = CombatTargetFinder.GetNearestTarget(
            firePoint.position,
            grenadeSkillData.rangeRadius);

        Vector3 targetPosition;

        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            Vector3 direction = playerTransform.forward;
            direction.y = 0f;
            direction = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;

            targetPosition = firePoint.position + direction * grenadeSkillData.rangeRadius;
        }

        if (TryFindGroundPosition(targetPosition, out Vector3 groundPosition))
            SetTargetPosition(groundPosition);
    }

    public void SetDirection(Vector2 direction, float distanceNormalized)
    {
        if (grenadeSkillData == null || playerTransform == null || direction.sqrMagnitude < 0.001f)
            return;

        Transform firePoint = GetFirePoint();
        if (firePoint == null) return;

        direction.Normalize();
        distanceNormalized = Mathf.Clamp01(distanceNormalized);

        Vector3 worldDirection = new Vector3(direction.x, 0f, direction.y);
        float range = grenadeSkillData.rangeRadius * distanceNormalized;
        Vector3 targetPosition = firePoint.position + worldDirection * range;

        if (TryFindGroundPosition(targetPosition, out Vector3 groundPosition))
            SetTargetPosition(groundPosition);
    }

    private void SetTargetPosition(Vector3 position)
    {
        if (grenadeIndicator == null) return;

        position.y = grenadeIndicatorYOffset;
        grenadeTargetPosition = position;
        hasGrenadeTargetPosition = true;

        grenadeIndicator.rectTransform.position = position;
        grenadeIndicator.gameObject.SetActive(true);
    }

    public bool TryGetTargetPosition(out Vector3 targetPosition)
    {
        targetPosition = grenadeTargetPosition;
        return hasGrenadeTargetPosition;
    }

    public void ClearTarget()
    {
        grenadeTargetPosition = Vector3.zero;
        hasGrenadeTargetPosition = false;
    }

    private Transform GetFirePoint()
    {
        return playerTransform != null
            ? playerTransform.GetComponent<PlayerSkill>()?.GetFirePoint()
            : null;
    }

    private bool TryFindGroundPosition(Vector3 targetPosition, out Vector3 groundPosition)
    {
        Vector3 rayStart = targetPosition + Vector3.up * 50f;

        if (Physics.Raycast(
            rayStart,
            Vector3.down,
            out RaycastHit hit,
            100f,
            grenadeSkillData.hitMask,
            QueryTriggerInteraction.Ignore))
        {
            groundPosition = hit.point;
            return true;
        }

        groundPosition = targetPosition;
        return false;
    }
}
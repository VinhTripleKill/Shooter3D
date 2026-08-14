using UnityEngine;
using UnityEngine.UI;

public class DashIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image dashIndicator;
    [SerializeField] private Transform posRotationDash;

    private Transform playerTransform;
    private float pixelsPerMeter = 100f;

    public void SetPlayerTransform(Transform player) => playerTransform = player;

    public void SetPixelsPerMeter(float value) => pixelsPerMeter = value;

    public void SetSkillData(SkillData skillData)
    {
        DashSkillData data = skillData as DashSkillData;

        if (data == null)
        {
            Hide();
            return;
        }

        float height = data.dashDistance * pixelsPerMeter;
        RectTransform rect = dashIndicator.rectTransform;

        Vector3 pos = rect.localPosition;
        pos.z = height * 0.5f;
        rect.localPosition = pos;

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        Show();
    }

    public void Show() => dashIndicator?.gameObject.SetActive(true);

    public void Hide() => dashIndicator?.gameObject.SetActive(false);

    public void SetAutoDirection()
    {
        if (posRotationDash != null)
            posRotationDash.localRotation = Quaternion.identity;
    }

    public void SetDirection(Vector2 direction)
    {
        if (posRotationDash == null || direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        float joystickAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        float playerRotationY = playerTransform != null ? playerTransform.eulerAngles.y : 0f;
        float localAngle = Mathf.DeltaAngle(0f, joystickAngle - playerRotationY);

        Vector3 rotation = posRotationDash.localEulerAngles;
        rotation.y = localAngle;
        posRotationDash.localEulerAngles = rotation;
    }
}
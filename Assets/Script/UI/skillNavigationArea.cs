using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNavigationArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image skillArea;
    [SerializeField] private Image skillJoystick;
    [SerializeField] private Image skillCancelZone;
    [SerializeField] private PlayerSkill playerSkill;
    [SerializeField] private SkillIndicatorUI skillIndicator;
    [SerializeField] private SkillJoystickHandle joystickHandle;

    [Header("Alpha")]
    [SerializeField] private float alphaColorHide;
    [SerializeField] private float alphaColorVisible = 1f;

    [Header("Color Cancel")]
    [SerializeField] private Color notCancelColor;
    [SerializeField] private Color isCancelColor;

    [Header("Area")]
    [SerializeField] private float initialLength = 150f;
    [SerializeField] private float initialWidth = 150f;
    [SerializeField] private float dragLength = 300f;
    [SerializeField] private float dragWidth = 300f;

    private RectTransform skillAreaRect;
    private RectTransform skillJoystickRect;
    private RectTransform skillCancelZoneRect;

    private Vector2 startPosition;

    private float pointerDownTime;

    private bool isDragging;
    private bool isSkillCancel;

    public Vector2 SkillDirection { get; private set; }

    private void Awake()
    {
        skillAreaRect = skillArea.rectTransform;
        skillJoystickRect = skillJoystick.rectTransform;

        skillCancelZoneRect = skillCancelZone != null
            ? skillCancelZone.rectTransform
            : null;

        startPosition = skillJoystickRect.anchoredPosition;

        if (skillCancelZone != null)
        {
            notCancelColor = skillCancelZone.color;
            skillCancelZone.gameObject.SetActive(false);
        }

        ResetVisual();
    }

    public void SetPlayerSkill(PlayerSkill skill)
    {
        playerSkill = skill;
    }

    public void SetSkillIndicator(SkillIndicatorUI indicator)
    {
        skillIndicator = indicator;
    }

    public void SetJoystickHandle(SkillJoystickHandle handle)
    {
        joystickHandle = handle;
    }

    public void SetCurrentSkill(SkillData skillData)
    {
        joystickHandle?.SetSkill(skillData);
        skillIndicator?.SetSkillData(skillData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerSkill == null) return;

        pointerDownTime = Time.time;

        isDragging = false;
        isSkillCancel = false;

        SkillDirection = Vector2.zero;

        SetImageAlpha(alphaColorVisible);
        SetSkillAreaSize(dragWidth, dragLength);

        if (skillCancelZone != null)
        {
            skillCancelZone.gameObject.SetActive(true);
            SetColorCancelSkill(false);
        }

        skillIndicator?.ShowIndicator();
        joystickHandle?.Begin();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (playerSkill == null) return;

        isDragging = true;
        UpdateCancelState(eventData);
        UpdateJoystick(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playerSkill == null)
        {
            ResetNavigation();
            return;
        }
        UpdateCancelState(eventData);

        if (isSkillCancel)
        {
            ResetNavigation();
            return;
        }

        float holdTime = Time.time - pointerDownTime;

        bool autoAim = holdTime <= playerSkill.timeAuto && !isDragging;

        bool manualAim = isDragging;

        if (joystickHandle != null && joystickHandle.RequiresDrag() && !autoAim && !manualAim)
        {
            ResetNavigation();
            return;
        }

        if (autoAim)
        {
            Vector3 direction = playerSkill.transform.forward;

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                direction = Vector3.forward;

            direction.Normalize();
            SkillDirection = new Vector2( direction.x, direction.z);
            playerSkill.SetAimDirection(direction, true);
        }


        else if (manualAim)
        {
            if (SkillDirection.sqrMagnitude < 0.001f)
            {
                ResetNavigation();
                return;
            }

            Vector3 direction = new Vector3( SkillDirection.x, 0f, SkillDirection.y );

            playerSkill.SetAimDirection( direction.normalized, false );

            if (joystickHandle != null &&
                joystickHandle.TryGetGrenadeTarget(out Vector3 target))
            {
                playerSkill.SetSkillTargetPosition(target);
            }
        }
        playerSkill.TryUseSkill();
        ResetNavigation();
    }

    private void UpdateCancelState(PointerEventData eventData)
    {
        if (skillCancelZoneRect == null)
        {
            isSkillCancel = false;
            SetColorCancelSkill(false);
            return;
        }

        bool inside = RectTransformUtility.RectangleContainsScreenPoint(
            skillCancelZoneRect,
            eventData.position,
            eventData.pressEventCamera
        );

        isSkillCancel = inside;
        SetColorCancelSkill(isSkillCancel);
    }

    private void UpdateJoystick(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            skillAreaRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            return;
        }

        float radius = skillAreaRect.rect.width * 0.5f;

        Vector2 position =
            Vector2.ClampMagnitude(localPoint, radius);

        skillJoystickRect.anchoredPosition = position;

        float distance = radius > 0.001f
            ? Mathf.Clamp01(position.magnitude / radius)
            : 0f;

        SkillDirection =
            position.sqrMagnitude > 0.001f
                ? position.normalized
                : Vector2.zero;

        joystickHandle?.UpdateDirection(
            SkillDirection,
            distance
        );
    }

    private void SetColorCancelSkill(bool isCancel)
    {
        if (skillCancelZone == null)
            return;

        skillCancelZone.color =
            isCancel
                ? isCancelColor
                : notCancelColor;
    }


    private void ResetNavigation()
    {
        skillIndicator?.HideIndicator();

        SetImageAlpha(alphaColorHide);

        SetSkillAreaSize(
            initialWidth,
            initialLength
        );

        skillJoystickRect.anchoredPosition = startPosition;

        SkillDirection = Vector2.zero;

        playerSkill?.ClearSkillTargetPosition();

        isDragging = false;
        isSkillCancel = false;

        SetColorCancelSkill(false);

        if (skillCancelZone != null)
            skillCancelZone.gameObject.SetActive(false);
    }

    private void SetSkillAreaSize(float width, float height)
    {
        skillAreaRect.sizeDelta = new Vector2(width, height);
    }

    private void SetImageAlpha(float alpha)
    {
        SetAlpha(skillArea, alpha);
        SetAlpha(skillJoystick, alpha);
    }

    private void SetAlpha(Image image, float alpha)
    {
        if (image == null) return;

        Color color = image.color;

        color.a = alpha;

        image.color = color;
    }

    private void ResetVisual()
    {
        SetSkillAreaSize( initialWidth, initialLength );

        SetImageAlpha(alphaColorHide);

        SetColorCancelSkill(false);
    }
}
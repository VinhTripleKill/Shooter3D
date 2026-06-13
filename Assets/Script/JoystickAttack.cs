using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickAttack : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image circleAreaATK;
    [SerializeField] private Image circleInnerAttackArea;
    [SerializeField] private Image attackB;
    [Header("Player")]
    [SerializeField] private PlayerWeapon playerWeapon;

    private RectTransform circleRect;
    private RectTransform innerRect;
    private RectTransform attackRect;

    private Vector2 startPosition;

    private bool isDragging;

    public Vector2 InputDirection { get; private set; }

    private enum AttackZone
    {
        Inner,
        Outer
    }

    private AttackZone currentZone;

    private void Awake()
    {
        circleRect = circleAreaATK.rectTransform;
        innerRect = circleInnerAttackArea.rectTransform;
        attackRect = attackB.rectTransform;
        startPosition = attackRect.anchoredPosition;

        currentZone = AttackZone.Inner;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        if (playerWeapon != null &&
            playerWeapon.HasGun())
        {
            playerWeapon.SetAttackState(true);
        }

        UpdateJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateJoystick(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        attackRect.anchoredPosition = startPosition;

        InputDirection = Vector2.zero;

        if (playerWeapon != null)
        {
            playerWeapon.SetAttackState(false);
        }
    }

    private void UpdateJoystick(PointerEventData eventData)
    {
        Vector2 localPoint;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                circleRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            return;

        float radius =
            circleRect.rect.width * 0.5f;

        Vector2 clampedPosition =
            Vector2.ClampMagnitude(
                localPoint,
                radius);

        attackRect.anchoredPosition =
            clampedPosition;

        InputDirection =
            clampedPosition.normalized;
        if (playerWeapon != null)
        {
            playerWeapon.SetManualAimDirection(
                InputDirection);
        }
        CheckZone(clampedPosition);
    }

    private void CheckZone(Vector2 currentPos)
    {
        float distance =
            currentPos.magnitude;

        float innerRadius =
            innerRect.rect.width * 0.5f;

        AttackZone newZone =
            distance <= innerRadius
            ? AttackZone.Inner
            : AttackZone.Outer;

        if (newZone == currentZone)
        {
            if (newZone == currentZone)
            {
                if (currentZone == AttackZone.Outer)
                {
                    playerWeapon.SetManualAimDirection(
                        InputDirection);
                }

                return;
            }
        }

        currentZone = newZone;

        if (playerWeapon != null)
        {
            bool autoAim =
                currentZone == AttackZone.Inner;

            playerWeapon.SetAutoAim(autoAim);
        }

        Debug.Log(
            $"[{Time.time:F2}] Attack Zone Changed -> {currentZone}");
    }

    public bool IsDragging()
    {
        return isDragging;
    }
}
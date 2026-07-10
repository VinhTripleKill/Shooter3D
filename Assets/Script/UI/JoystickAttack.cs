using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickAttack : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image OuterArea;
    [SerializeField] private Image InnerArea;
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
        circleRect = OuterArea.rectTransform;
        innerRect = InnerArea.rectTransform;
        attackRect = attackB.rectTransform;
        startPosition = attackRect.anchoredPosition;

        currentZone = AttackZone.Inner;
    }


    public void OnDrag(PointerEventData eventData)
    {
        UpdateJoystick(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        // KHÔNG chỉ set cho gun nữa
        if (playerWeapon != null && playerWeapon.HasGun())
        {
            playerWeapon.SetAttackState(true);
        }
        // Melee sẽ được xử lý ở PlayerCombat

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
            playerWeapon.SetJoystickManualAim(
                InputDirection);
        }
        CheckZone(clampedPosition);
    }

    private void CheckZone(Vector2 currentPos)
    {
        float distance = currentPos.magnitude;

        float innerRadius =
            innerRect.rect.width * 0.5f;

        AttackZone newZone =
            distance <= innerRadius
            ? AttackZone.Inner
            : AttackZone.Outer;

        currentZone = newZone;

        if (playerWeapon == null)
            return;

        if (currentZone == AttackZone.Inner)
        {
            playerWeapon.SetAutoAim(true);
        }
        else
        {
            playerWeapon.SetAutoAim(false);

            playerWeapon.SetJoystickManualAim(
                InputDirection);
        }
    }
    public bool IsDragging()
    {
        return isDragging;
    }
}
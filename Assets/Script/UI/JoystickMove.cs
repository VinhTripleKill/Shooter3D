using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickMove : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image circleAreaMove;
    [SerializeField] private Image moveB;

    private RectTransform circleRect;
    private RectTransform moveRect;

    private Vector2 startPosition;

    private bool isDragging;

    public Vector2 MoveDirection { get; private set; }

    private void Start()
    {
        circleRect = circleAreaMove.rectTransform;
        moveRect = moveB.rectTransform;

        startPosition = moveRect.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");

        isDragging = true;

        UpdateJoystick(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        UpdateJoystick(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        moveRect.anchoredPosition = startPosition;

        MoveDirection = Vector2.zero;
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

        float radius = circleRect.rect.width * 0.5f;

        Vector2 clampedPosition =
            Vector2.ClampMagnitude(localPoint, radius);

        moveRect.anchoredPosition = clampedPosition;

        MoveDirection =
            clampedPosition.normalized;
    }
    public void ResetJoystick()
{
    isDragging = false;
    moveRect.anchoredPosition = startPosition;
    MoveDirection = Vector2.zero;
}
    public bool IsDragging()
    {
        return isDragging;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickAttack : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    // =====================================================
    // UI REFERENCES
    // =====================================================

    [Header("References")]

    [SerializeField]
    private Image OuterArea;

    [SerializeField]
    private Image InnerArea;

    [SerializeField]
    private Image attackB;

    // =====================================================
    // PLAYER REFERENCES
    // =====================================================

    [Header("Player")]

    [SerializeField]
    private PlayerWeapon playerWeapon;

    [SerializeField]
    private PlayerShoot playerShoot;

    [SerializeField]
    private PlayerReload playerReload;

    // =====================================================
    // RECT TRANSFORMS
    // =====================================================

    private RectTransform circleRect;
    private RectTransform innerRect;
    private RectTransform attackRect;

    private Vector2 startPosition;

    // =====================================================
    // STATE
    // =====================================================

    private bool isDragging;

    public Vector2 InputDirection
    {
        get;
        private set;
    }

    // =====================================================
    // ATTACK ZONE
    // =====================================================

    private enum AttackZone
    {
        Inner,
        Outer
    }

    private AttackZone currentZone;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (OuterArea != null)
        {
            circleRect =
                OuterArea.rectTransform;
        }

        if (InnerArea != null)
        {
            innerRect =
                InnerArea.rectTransform;
        }

        if (attackB != null)
        {
            attackRect =
                attackB.rectTransform;

            startPosition =
                attackRect.anchoredPosition;
        }

        currentZone =
            AttackZone.Inner;
    }

    // =====================================================
    // SET PLAYER WEAPON
    // =====================================================

    public void SetPlayerWeapon(
        PlayerWeapon weapon)
    {
        playerWeapon = weapon;

        Debug.Log(
            "JoystickAttack | Đã liên kết PlayerWeapon."
        );
    }

    // =====================================================
    // SET PLAYER SHOOT
    // =====================================================

    public void SetPlayerShoot(
        PlayerShoot shoot)
    {
        playerShoot = shoot;

        Debug.Log(
            "JoystickAttack | Đã liên kết PlayerShoot."
        );
    }

    // =====================================================
    // SET PLAYER RELOAD
    // =====================================================

    public void SetPlayerReload(
        PlayerReload reload)
    {
        playerReload = reload;

        Debug.Log(
            "JoystickAttack | Đã liên kết PlayerReload."
        );
    }

    // =====================================================
    // POINTER DOWN
    // =====================================================

    public void OnPointerDown(
        PointerEventData eventData)
    {
        isDragging = true;

        // ---------------------------------------------
        // BẮT ĐẦU ATTACK
        // ---------------------------------------------

        if (playerShoot != null)
        {
            playerShoot.SetAttackState(
                true
            );
        }

        UpdateJoystick(
            eventData
        );
    }

    // =====================================================
    // DRAG
    // =====================================================

    public void OnDrag(
        PointerEventData eventData)
    {
        UpdateJoystick(
            eventData
        );
    }

    // =====================================================
    // POINTER UP
    // =====================================================

    public void OnPointerUp(
        PointerEventData eventData)
    {
        isDragging = false;

        // ---------------------------------------------
        // RESET JOYSTICK
        // ---------------------------------------------

        if (attackRect != null)
        {
            attackRect.anchoredPosition =
                startPosition;
        }

        InputDirection =
            Vector2.zero;

        // ---------------------------------------------
        // STOP ATTACK
        // ---------------------------------------------

        if (playerShoot != null)
        {
            playerShoot.SetAttackState(
                false
            );
        }
    }

    // =====================================================
    // UPDATE JOYSTICK
    // =====================================================

    private void UpdateJoystick(
        PointerEventData eventData)
    {
        if (circleRect == null)
            return;

        Vector2 localPoint;

        if (!RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                circleRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            return;
        }

        float radius =
            circleRect.rect.width *
            0.5f;

        Vector2 clampedPosition =
            Vector2.ClampMagnitude(
                localPoint,
                radius
            );

        // ---------------------------------------------
        // MOVE BUTTON
        // ---------------------------------------------

        if (attackRect != null)
        {
            attackRect.anchoredPosition =
                clampedPosition;
        }

        // ---------------------------------------------
        // INPUT DIRECTION
        // ---------------------------------------------

        InputDirection =
            clampedPosition.sqrMagnitude >
            0.001f
                ? clampedPosition.normalized
                : Vector2.zero;

        // ---------------------------------------------
        // SEND AIM TO PLAYER SHOOT
        // ---------------------------------------------

        if (playerShoot != null)
        {
            playerShoot.SetJoystickManualAim(
                InputDirection
            );
        }

        // ---------------------------------------------
        // CHECK ZONE
        // ---------------------------------------------

        CheckZone(
            clampedPosition
        );
    }

    // =====================================================
    // CHECK ZONE
    // =====================================================

    private void CheckZone(
        Vector2 currentPos)
    {
        if (innerRect == null)
            return;

        float distance =
            currentPos.magnitude;

        float innerRadius =
            innerRect.rect.width *
            0.5f;

        AttackZone newZone =
            distance <= innerRadius
                ? AttackZone.Inner
                : AttackZone.Outer;

        currentZone =
            newZone;

        if (playerShoot == null)
            return;

        // ---------------------------------------------
        // INNER
        // AUTO AIM
        // ---------------------------------------------

        if (currentZone ==
            AttackZone.Inner)
        {
            playerShoot.SetAutoAim(
                true
            );
        }

        // ---------------------------------------------
        // OUTER
        // MANUAL AIM
        // ---------------------------------------------

        else
        {
            playerShoot.SetAutoAim(
                false
            );

            playerShoot.SetJoystickManualAim(
                InputDirection
            );
        }
    }

    // =====================================================
    // RELOAD BUTTON
    // =====================================================

    public void Reload()
    {
        if (playerReload == null)
            return;

        playerReload.ManualReload();
    }

    // =====================================================
    // IS DRAGGING
    // =====================================================

    public bool IsDragging()
    {
        return isDragging;
    }
}

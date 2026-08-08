using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillNavigationArea : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image skillArea;
    [SerializeField] private Image skillJoystick;
    [SerializeField] private PlayerSkill playerSkill;
    private RectTransform skillAreaRect;
    private RectTransform skillJoystickRect;
    [Header("Alpha")]
    [SerializeField] private float alphaColorHide = 0f;
    [SerializeField] private float alphaColorVisible = 1f;
    private float pointerDownTime;
    private Vector2 startPosition;

    private bool isDragging;


    [Header("Initial Skill Area")]
    [SerializeField] private float initialLength = 150f;
    [SerializeField] private float initialWidth = 150f;
    
    [Header("Drag Skill Area")]
    [SerializeField] private float dragLength = 300f;
    [SerializeField] private float dragWidth = 300f;


    public Vector2 SkillDirection { get; private set; }



    private void Awake()
    {
        skillAreaRect = skillArea.rectTransform;
        skillJoystickRect = skillJoystick.rectTransform;

        startPosition = skillJoystickRect.anchoredPosition;
        SetSkillAreaSize(initialWidth, initialLength);
        SetImageAlpha(alphaColorHide);
        Debug.Log( $"Skill Awake | Joystick Start Position: {startPosition}" );
    }
    private void SetSkillAreaSize(float width, float height)
    {
        skillAreaRect.sizeDelta = new Vector2(width, height);
    }
    private void SetImageAlpha(float alpha)
    {
        Color areaColor = skillArea.color;
        areaColor.a = alpha;
        skillArea.color = areaColor;
    
        Color joystickColor = skillJoystick.color;
        joystickColor.a = alpha;
        skillJoystick.color = joystickColor;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("========== SKILL POINTER DOWN ==========");
        pointerDownTime = Time.time;
        isDragging = true;
        SetImageAlpha(alphaColorVisible);
        SetSkillAreaSize(dragWidth, dragLength);
        Debug.Log($"IsDragging: {isDragging}");

        UpdateSkillJoystick(eventData);
    }



    public void OnDrag(PointerEventData eventData)
    {
        SetImageAlpha(alphaColorVisible);
        UpdateSkillJoystick(eventData);
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("========== SKILL POINTER UP ==========");
        float holdTime = Time.time - pointerDownTime;
    
        bool autoAim = holdTime <= playerSkill.timeAuto;
    
        Debug.Log($"Before Reset | SkillJoystick Position: {skillJoystickRect.anchoredPosition}");
    
    
        Debug.Log($"Before Reset | Skill Direction: {SkillDirection}");
    
    
        isDragging = false;
        SetImageAlpha(alphaColorHide);
        SetSkillAreaSize(initialWidth, initialLength);
        Vector2 dir = SkillDirection;
    
        Vector3 worldDir = new Vector3( SkillDirection.x, 0, SkillDirection.y);
    
        playerSkill.SetAimDirection(worldDir, autoAim);
    
        playerSkill.TryUseSkill();
    
    
        skillJoystickRect.anchoredPosition = startPosition;
    
    
        SkillDirection = Vector2.zero;
    
    
        Debug.Log($"After Reset | SkillJoystick Position: {skillJoystickRect.anchoredPosition}");
    
    
        Debug.Log($"IsDragging: {isDragging}");
    
        Debug.Log("=======================================");
    }


    
    public void SetPlayerSkill(PlayerSkill skill)
    {
        playerSkill = skill;
    
        Debug.Log("SkillNavigationArea đã nhận PlayerSkill");
    }



    private void UpdateSkillJoystick(PointerEventData eventData)
    {
        Vector2 localPoint;


        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            skillAreaRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
            return;



        float radius = skillAreaRect.rect.width * 0.5f;



        Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, radius);



        skillJoystickRect.anchoredPosition = clampedPosition;



        SkillDirection = clampedPosition.normalized;



        // DEBUG KHI ĐANG GIỮ
        Debug.Log(
            $"Dragging: {isDragging} | " +
            $"Joystick Pos: {skillJoystickRect.anchoredPosition} | " +
            $"SkillArea Center: {Vector2.zero} | " +
            $"Distance: {clampedPosition.magnitude}/{radius} | " +
            $"Direction: {SkillDirection}"
        );
    }




    public void ResetSkillJoystick()
    {
        Debug.Log("Reset Skill Joystick");


        isDragging = false;


        skillJoystickRect.anchoredPosition = startPosition;


        SkillDirection = Vector2.zero;


        Debug.Log($"Reset Position: {skillJoystickRect.anchoredPosition}");
    }



    public bool IsDragging()
    {
        return isDragging;
    }
}
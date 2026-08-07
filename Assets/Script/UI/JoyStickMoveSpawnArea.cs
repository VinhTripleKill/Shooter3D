using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickMoveSpawnArea : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image joystickMoveSpawnArea;

    [SerializeField] private GameObject joystickMove;

    [SerializeField] private Transform resetJoystickMovePos;

    private RectTransform spawnRect;
    private RectTransform joystickRect;

    private JoystickMove joystick;

    private void Start()
    {
        spawnRect = joystickMoveSpawnArea.rectTransform;
        joystickRect = joystickMove.GetComponent<RectTransform>();
        joystick = joystickMove.GetComponent<JoystickMove>();

        ResetJoystick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SpawnJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (joystick != null)
            joystick.OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (joystick != null)
            joystick.OnPointerUp(eventData);

        ResetJoystick();
    }

    private void SpawnJoystick(PointerEventData eventData)
    {
        joystickMove.SetActive(true);

        joystickRect.position = eventData.position;

        if (joystick != null)
            joystick.OnPointerDown(eventData);
    }

    private void ResetJoystick()
{
    joystickRect.position = resetJoystickMovePos.position;

    joystick.ResetJoystick();
}
}
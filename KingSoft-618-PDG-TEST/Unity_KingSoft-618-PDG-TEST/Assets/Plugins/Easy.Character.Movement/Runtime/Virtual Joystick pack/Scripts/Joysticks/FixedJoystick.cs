using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedJoystick : Joystick
{
    public static FixedJoystick instance = null;
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();
    public int finderID = -1;
    public Text txt;

    void Start()
    {
        instance = this;
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
        //finderID = Input.GetTouch(0).fingerId;
        if(Input.touchCount > 0) {
            EventSystem eventSystem = EventSystem.current;

            for(int i = 0; i < Input.touchCount; i++)
            if ((eventSystem.IsPointerOverGameObject(Input.GetTouch(i).fingerId) && eventSystem.currentSelectedGameObject != null)) {
                finderID = Input.GetTouch(i).fingerId;
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
        finderID = eventData.pointerId;
        //txt.text = finderID + "";
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        finderID = -1;
    }
}
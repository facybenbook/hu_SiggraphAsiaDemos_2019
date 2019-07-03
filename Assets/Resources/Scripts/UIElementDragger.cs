using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementDragger : EventTrigger
{
    Camera ui_camera;
    float dis_canvas2camera=100f;

    private bool dragging;

    void Awake()
    {
        ui_camera = this.gameObject.transform.parent.GetComponent<Canvas>().worldCamera;
    }

    public void Update()
    {
        if (dragging)
        {
            Vector3 screenPoint = new Vector3(Input.mousePosition.x,Input.mousePosition.y,dis_canvas2camera);
            transform.position = ui_camera.ScreenToWorldPoint(screenPoint);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
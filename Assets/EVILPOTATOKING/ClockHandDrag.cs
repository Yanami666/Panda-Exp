using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ClockHandDrag : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    private float angleOffset;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        if (cam == null) cam = Camera.main;

        isDragging = true;

        Vector3 center = transform.position;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = center.z;

        Vector2 dir = (mouseWorld - center);
        float mouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        angleOffset = transform.eulerAngles.z - mouseAngle;
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 center = transform.position;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = center.z;

        Vector2 dir = (mouseWorld - center);
        float mouseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float finalAngle = mouseAngle + angleOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, finalAngle);
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
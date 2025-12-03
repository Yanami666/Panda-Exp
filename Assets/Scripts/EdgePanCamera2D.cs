using UnityEngine;

public class EdgePanCamera2D : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 5f;

    [Range(0f, 0.5f)]
    public float edgePercent = 0.05f;

    [Header("X 轴移动范围（世界坐标）")]
    public float minX = -4.54f;
    public float maxX = 4.54f;

    [Header("Lock Camera")]
    public bool canPan = true;   // ✨ 是否允许平移

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // ✅ 如果当前不允许移动，直接 return
        if (!canPan)
            return;

        float mouseX = Input.mousePosition.x;
        float screenW = Screen.width;
        float edgeZone = screenW * edgePercent;

        float dir = 0f;

        if (mouseX <= edgeZone)
        {
            dir = -1f;
        }
        else if (mouseX >= screenW - edgeZone)
        {
            dir = 1f;
        }

        if (dir != 0f)
        {
            Vector3 pos = transform.position;
            pos.x += dir * panSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;
        }
    }
}
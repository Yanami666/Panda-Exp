using UnityEngine;

public class ClickableSquare : MonoBehaviour
{
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color clickedColor = Color.green;

    [Header("This Square Will Activate This Object")]
    public GameObject objectToActivate;  // 比如 PuzzleOverlay_数独

    [Header("Optional: Close Icon (only for Sudoku overlay)")]
    public Transform closeIcon; // 拖 CloseIcon 进来（可空）
    public Vector2 closeIconViewportPos = new Vector2(0.92f, 0.88f); // 屏幕右上角附近（0~1）
    public Vector3 closeIconWorldOffset = Vector3.zero; // 细调（可空）

    private SpriteRenderer sr;
    private EdgePanCamera2D cameraPan;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = normalColor;

        Camera mainCam = Camera.main;
        if (mainCam != null)
            cameraPan = mainCam.GetComponent<EdgePanCamera2D>();
    }

    void OnMouseDown()
    {
        if (PuzzleManager.puzzleOpen)
            return;

        if (sr != null)
            sr.color = clickedColor;

        if (objectToActivate != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                // ✅ overlay 移到相机中心（只改 XY，不动 Z）
                Vector3 camPos = cam.transform.position;
                Vector3 newPos = objectToActivate.transform.position;
                newPos.x = camPos.x;
                newPos.y = camPos.y;
                objectToActivate.transform.position = newPos;
            }

            objectToActivate.SetActive(true);

            // ✅ 如果有 CloseIcon：强制放到相机右上角
            if (closeIcon != null)
            {
                PlaceCloseIconAtViewport(closeIcon, closeIconViewportPos, closeIconWorldOffset);
            }
        }

        if (cameraPan != null)
            cameraPan.canPan = false;

        PuzzleManager.puzzleOpen = true;
    }

    private void PlaceCloseIconAtViewport(Transform icon, Vector2 viewport01, Vector3 worldOffset)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // 用 icon 当前的 Z（避免跑到相机后面/前面）
        float z = icon.position.z;

        // 把 viewport(0~1) 转成世界坐标
        Vector3 world = cam.ViewportToWorldPoint(new Vector3(viewport01.x, viewport01.y, -cam.transform.position.z));

        // 保持 icon 的 Z
        world.z = z;

        icon.position = world + worldOffset;
    }
}
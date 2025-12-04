using UnityEngine;

public class ClickableSquare : MonoBehaviour
{
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color clickedColor = Color.green;

    [Header("This Square Will Activate This Object")]
    public GameObject objectToActivate;  // 比如 PuzzleOverlay_Chess

    private SpriteRenderer sr;
    private EdgePanCamera2D cameraPan;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = normalColor;

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            cameraPan = mainCam.GetComponent<EdgePanCamera2D>();
        }
    }

    void OnMouseDown()
    {
        // 🚫 如果当前有 puzzle 打开，直接忽略所有 square 的点击
        if (PuzzleManager.puzzleOpen)
            return;

        Debug.Log("Clicked: " + gameObject.name);

        if (sr != null)
            sr.color = clickedColor;

        if (objectToActivate != null)
        {
            // 移动到当前相机中心这一段如果你有，就保留：
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 camPos = cam.transform.position;
                Vector3 newPos = objectToActivate.transform.position;
                newPos.x = camPos.x;
                newPos.y = camPos.y;
                objectToActivate.transform.position = newPos;
            }

            objectToActivate.SetActive(true);
        }

        // 🔒 锁相机（如果你之前有）
        if (cameraPan != null)
        {
            cameraPan.canPan = false;
        }

        // ✅ 标记：现在有 puzzle 打开了
        PuzzleManager.puzzleOpen = true;
    }
}
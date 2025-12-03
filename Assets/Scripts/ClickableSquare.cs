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
        Debug.Log("Clicked: " + gameObject.name);

        if (sr != null)
            sr.color = clickedColor;

        if (objectToActivate != null)
        {
            // ⭐ 在激活之前，把 puzzle 挪到当前相机中心
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 camPos = cam.transform.position;

                // 保留原来的 Z（避免跑到相机里去看不到）
                Vector3 newPos = objectToActivate.transform.position;
                newPos.x = camPos.x;
                newPos.y = camPos.y;
                objectToActivate.transform.position = newPos;
            }

            objectToActivate.SetActive(true);
        }

        // 锁住相机
        if (cameraPan != null)
        {
            cameraPan.canPan = false;
        }
    }
}
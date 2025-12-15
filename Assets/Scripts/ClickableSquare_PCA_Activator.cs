using System.Collections;
using UnityEngine;

public class ClickableSquare_PCA_Activator : MonoBehaviour
{
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color clickedColor = Color.green;

    [Header("Overlay Root (activate this)")]
    public GameObject overlayRoot; // PuzzleOverlay_pcA

    [Header("Also activate this child/root (recommended)")]
    public GameObject contentRootToActivate; // Page_Desktop 或 puzzel

    [Header("Optional: Close Icon")]
    public Transform closeIcon;
    public Vector2 closeIconViewportPos = new Vector2(0.92f, 0.88f);
    public Vector3 closeIconWorldOffset = Vector3.zero;

    private SpriteRenderer sr;
    private EdgePanCamera2D cameraPan;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = normalColor;

        Camera mainCam = Camera.main;
        if (mainCam != null)
            cameraPan = mainCam.GetComponent<EdgePanCamera2D>();
    }

    private void OnMouseDown()
    {
        if (PuzzleManager.puzzleOpen)
            return;

        if (sr != null)
            sr.color = clickedColor;

        if (overlayRoot == null)
        {
            Debug.LogWarning("[PCA Activator] overlayRoot is null.");
            return;
        }

        // 把 overlay 移到相机中心（只改 XY）
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 camPos = cam.transform.position;
            Vector3 newPos = overlayRoot.transform.position;
            newPos.x = camPos.x;
            newPos.y = camPos.y;
            overlayRoot.transform.position = newPos;
        }

        overlayRoot.SetActive(true);

        if (closeIcon != null)
            PlaceCloseIconAtViewport(closeIcon, closeIconViewportPos, closeIconWorldOffset);

        if (cameraPan != null)
            cameraPan.canPan = false;

        PuzzleManager.puzzleOpen = true;

        // ✅ 关键：延迟一帧再强制打开内容，避免被别的脚本马上关回去
        StartCoroutine(ForceShowNextFrame());
    }

    private IEnumerator ForceShowNextFrame()
    {
        yield return null; // 等一帧

        if (contentRootToActivate != null)
        {
            contentRootToActivate.SetActive(true);

            // ✅ 把它下面所有子物体也一起打开（避免某些关键子节点默认是灰的）
            SetChildrenActiveRecursive(contentRootToActivate.transform, true);

            // ✅ 如果有 CanvasGroup，强制可见&可交互
            var groups = contentRootToActivate.GetComponentsInChildren<CanvasGroup>(true);
            foreach (var g in groups)
            {
                g.alpha = 1f;
                g.interactable = true;
                g.blocksRaycasts = true;
            }

            Debug.Log($"[PCA Activator] Forced show: {contentRootToActivate.name} (activeSelf={contentRootToActivate.activeSelf})");
        }
        else
        {
            Debug.LogWarning("[PCA Activator] contentRootToActivate is null (you didn't assign it).");
        }

        Debug.Log($"[PCA Activator] overlayRoot activeSelf={overlayRoot.activeSelf}");
    }

    private void SetChildrenActiveRecursive(Transform root, bool active)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform c = root.GetChild(i);
            c.gameObject.SetActive(active);
            SetChildrenActiveRecursive(c, active);
        }
    }

    private void PlaceCloseIconAtViewport(Transform icon, Vector2 viewport01, Vector3 worldOffset)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float z = icon.position.z;
        Vector3 world = cam.ViewportToWorldPoint(new Vector3(viewport01.x, viewport01.y, -cam.transform.position.z));
        world.z = z;
        icon.position = world + worldOffset;
    }
}
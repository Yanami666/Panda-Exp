using UnityEngine;

public class ClickToClosePuzzle : MonoBehaviour
{
    public GameObject puzzleRoot;   // 拖你的 Overlay 根物体（比如 日历 / item1_Overlay / PuzzleOverlay_xxx）

    private EdgePanCamera2D cameraPan;
    private float ignoreUntil;

    void Awake()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
            cameraPan = mainCam.GetComponent<EdgePanCamera2D>();
    }

    void OnEnable()
    {
        // 防止“打开的同一次点击”立刻触发关闭
        ignoreUntil = Time.time + 0.1f;
    }

    void OnMouseDown()
    {
        if (Time.time < ignoreUntil)
            return;

        // 主动点击关闭
        if (puzzleRoot != null)
            puzzleRoot.SetActive(false);

        // 这里仍然保留复位（正常路径）
        ReleasePuzzleLock();
    }

    // ✅ 关键：无论是点击关、还是被别的脚本 SetActive(false)，都会触发 OnDisable
    void OnDisable()
    {
        ReleasePuzzleLock();
    }

    private void ReleasePuzzleLock()
    {
        if (cameraPan != null)
            cameraPan.canPan = true;

        PuzzleManager.puzzleOpen = false;
    }
}
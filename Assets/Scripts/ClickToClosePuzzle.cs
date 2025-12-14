using UnityEngine;

public class ClickToClosePuzzle : MonoBehaviour
{
    public GameObject puzzleRoot;   // 拖 PuzzleOverlay_数独

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

        if (puzzleRoot != null)
            puzzleRoot.SetActive(false);

        if (cameraPan != null)
            cameraPan.canPan = true;

        PuzzleManager.puzzleOpen = false;
    }
}
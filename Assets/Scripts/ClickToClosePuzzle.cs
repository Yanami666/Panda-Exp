using UnityEngine;

public class ClickToClosePuzzle : MonoBehaviour
{
    public GameObject puzzleRoot;   // 一般拖 PuzzleOverlay 自己

    private EdgePanCamera2D cameraPan;

    void Awake()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            cameraPan = mainCam.GetComponent<EdgePanCamera2D>();
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Overlay clicked, closing puzzle.");

        if (puzzleRoot != null)
        {
            puzzleRoot.SetActive(false);
        }

        // 🔓 恢复相机移动（如果你之前有）
        if (cameraPan != null)
        {
            cameraPan.canPan = true;
        }

        // ✅ 标记：现在没有 puzzle 了，可以再次点击其他 square
        PuzzleManager.puzzleOpen = false;
    }
}
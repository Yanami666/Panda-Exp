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

        // ✨ 关闭 puzzle 时恢复相机移动
        if (cameraPan != null)
        {
            cameraPan.canPan = true;
        }
    }
}
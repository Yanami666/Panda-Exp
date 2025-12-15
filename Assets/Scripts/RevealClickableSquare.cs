using UnityEngine;

public class RevealClickableSquare : MonoBehaviour
{
    [Header("要显示的 Clickable Square（场景里的实例）")]
    public GameObject clickableSquare;

    [Header("Close Icon（点击关闭）")]
    public GameObject closeIcon;

    [Header("是否把方块移动到当前物体附近")]
    public bool moveSquareToThisObject = true;

    [Tooltip("以当前物体为中心的偏移量")]
    public Vector3 offset;

    [Header("只触发一次？")]
    public bool onlyOnce = true;

    private bool hasRevealed = false;
    private Collider2D myCollider;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();

        if (clickableSquare != null)
            clickableSquare.SetActive(false);

        if (closeIcon != null)
            closeIcon.SetActive(false);
    }

    void OnMouseDown()
    {
        if (onlyOnce && hasRevealed) return;

        hasRevealed = true;

        if (clickableSquare != null)
        {
            clickableSquare.SetActive(true);

            if (moveSquareToThisObject)
                clickableSquare.transform.position = transform.position + offset;
        }

        if (closeIcon != null)
            closeIcon.SetActive(true);

        // 防止和 clickableSquare 抢 Raycast
        if (onlyOnce && myCollider != null)
            myCollider.enabled = false;
    }

    /// <summary>
    /// 由 CloseIcon 调用
    /// </summary>
    public void CloseSquare()
    {
        if (clickableSquare != null)
            clickableSquare.SetActive(false);

        if (closeIcon != null)
            closeIcon.SetActive(false);

        // ✅ 兜底：如果你曾经打开过某个 overlay，但关闭路径没走到 ClickToClosePuzzle，
        // 这里也可以释放锁，避免所有 ClickableSquare 被 puzzleOpen 卡死。
        PuzzleManager.puzzleOpen = false;

        // 额外保险：如果你有 EdgePanCamera2D，也允许相机恢复
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            var pan = mainCam.GetComponent<EdgePanCamera2D>();
            if (pan != null) pan.canPan = true;
        }
    }
}
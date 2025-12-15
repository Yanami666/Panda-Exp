using UnityEngine;
using UnityEngine.Events;

public class ChessPuzzleManager : MonoBehaviour
{
    public static ChessPuzzleManager Instance { get; private set; }

    [Header("总格子数量（留空会自动数） / Total slots (auto if 0)")]
    public int totalSlots = 0;

    private int filledCorrectSlots = 0;
    private bool completed = false;

    [Header("成功时要显示的物体（UI 图片或 Sprite） / Object to show on success")]
    public GameObject successObject;   // 日记

    [Header("成功时要隐藏的 Puzzle 根物体 / Puzzle root to hide on success")]
    public GameObject puzzleRootToHide; // 你现在可能拖的是 PuzzleOverlay_沙盘A 或 a_chess

    [Header("完成后：强制把 successObject 移到相机中心")]
    public bool moveSuccessToCameraCenter = true;

    [Header("完成后：强制 successObject 渲染到最上层（SpriteRenderer）")]
    public bool forceSuccessOnTop = true;
    public int successSortingOrder = 999;

    [Header("拼图完成时触发的事件 / Event when puzzle is complete")]
    public UnityEvent onPuzzleComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (totalSlots <= 0)
            totalSlots = FindObjectsOfType<ChessPuzzleSlot>().Length;

        if (successObject != null)
            successObject.SetActive(false);
    }

    public void RegisterCorrectPlaced()
    {
        if (completed) return;

        filledCorrectSlots++;

        if (filledCorrectSlots < totalSlots)
            return;

        completed = true;
        UnityEngine.Debug.Log("Chess puzzle complete!");

        // 0) 防止 UnlockableItem 反手把日记隐藏（如果你还残留这个组件）
        if (successObject != null)
        {
            var unlockable = successObject.GetComponent<UnlockableItem>();
            if (unlockable != null) unlockable.enabled = false;
        }

        // 1) 如果 successObject 在 puzzleRootToHide 下面：不要 SetActive(false) root
        //    改为：隐藏 root 下的所有子物体（除了 successObject）
        if (puzzleRootToHide != null && successObject != null &&
            successObject.transform.IsChildOf(puzzleRootToHide.transform))
        {
            HideAllChildrenExcept(puzzleRootToHide.transform, successObject.transform);
        }
        else
        {
            // successObject 不在 root 下面：可以安全关闭 root
            if (puzzleRootToHide != null)
                puzzleRootToHide.SetActive(false);
        }

        // 2) 显示日记
        if (successObject != null)
            successObject.SetActive(true);

        // 3) 强制到相机中心
        if (moveSuccessToCameraCenter && successObject != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                float z = successObject.transform.position.z;
                Vector3 camPos = cam.transform.position;
                successObject.transform.position = new Vector3(camPos.x, camPos.y, z);
            }
        }

        // 4) 强制排序到最上层（只对 SpriteRenderer 有效）
        if (forceSuccessOnTop && successObject != null)
        {
            var srs = successObject.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in srs)
                sr.sortingOrder = successSortingOrder;
        }

        onPuzzleComplete?.Invoke();
    }

    private void HideAllChildrenExcept(Transform root, Transform keep)
    {
        // root 自己保持 active；只处理其子层级
        var all = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in all)
        {
            if (t == root) continue;

            // keep 本体及其子物体全部保留
            if (t == keep || t.IsChildOf(keep))
                continue;

            t.gameObject.SetActive(false);
        }
    }
}
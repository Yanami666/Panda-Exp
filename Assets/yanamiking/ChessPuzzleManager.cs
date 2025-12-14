using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class ChessPuzzleManager : MonoBehaviour
{
    public static ChessPuzzleManager Instance { get; private set; }

    [Header("总格子数量（留空会自动数） / Total slots (auto if 0)")]
    public int totalSlots = 0;

    private int filledCorrectSlots = 0;

    [Header("成功时要显示的物体（UI 图片或 Sprite） / Object to show on success")]
    public GameObject successObject;

    // ✅ 新增：拼图成功后要隐藏的 Puzzle 根物体（Overlay / Root）
    [Header("成功时要隐藏的 Puzzle 根物体 / Puzzle root to hide on success")]
    public GameObject puzzleRootToHide;

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
        // 如果没手动填，就自动统计场景中的 ChessPuzzleSlot 数量
        // If not set, auto-count ChessPuzzleSlot in scene
        if (totalSlots <= 0)
        {
            totalSlots = FindObjectsOfType<ChessPuzzleSlot>().Length;
        }

        if (successObject != null)
        {
            successObject.SetActive(false);
        }
    }

    public void RegisterCorrectPlaced()
    {
        filledCorrectSlots++;

        if (filledCorrectSlots >= totalSlots)
        {
            UnityEngine.Debug.Log("Chess puzzle complete!");

            // ✅ 1) 先隐藏 puzzle（让拼图消失）
            if (puzzleRootToHide != null)
                puzzleRootToHide.SetActive(false);

            // ✅ 2) 再显示线索 sprite/UI
            if (successObject != null)
                successObject.SetActive(true);

            onPuzzleComplete?.Invoke();
        }
    }
}
using UnityEngine;

[DisallowMultipleComponent]
public class SudokuNoteUnlocker : MonoBehaviour
{
    [Header("Unlockable note to show when solved")]
    [Tooltip("把 PuzzleOverlay_数独/Note 拖进来（或 Note 上的 UnlockableItem 组件）")]
    public UnlockableItem noteUnlockable;

    [Header("Optional: also force SetActive(true) after unlock")]
    public bool forceActiveTrueAfterUnlock = true;

    [Header("Debug")]
    public bool logDebug = true;

    /// <summary>
    /// ✅ 在“数独解对/完成”的那一刻调用这个方法
    /// 你可以从任何脚本里调用，也可以在 UnityEvent 里绑这个函数。
    /// </summary>
    public void OnSudokuSolved()
    {
        if (noteUnlockable == null)
        {
            if (logDebug)
                UnityEngine.Debug.LogError("[SudokuNoteUnlocker] noteUnlockable 没有绑定。请把 Note(带UnlockableItem)拖进来。", this);
            return;
        }

        // 1) 持久化解锁 + 按解锁状态应用可见性
        noteUnlockable.MarkUnlocked();

        // 2) 保险：有些情况下你希望立刻显示（哪怕外部脚本又动了 active）
        if (forceActiveTrueAfterUnlock)
        {
            GameObject noteGO = noteUnlockable.gameObject;
            if (!noteGO.activeSelf) noteGO.SetActive(true);
        }

        if (logDebug)
            UnityEngine.Debug.Log($"[SudokuNoteUnlocker] Unlocked note: {noteUnlockable.unlockId}", this);
    }
}
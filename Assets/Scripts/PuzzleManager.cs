using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // 是否有 puzzle 正在打开
    public static bool puzzleOpen = false;

    [Header("Unlock-All Settings")]
    [Tooltip("所有“解锁目标物体”都要设置成这个 Tag")]
    public string unlockableTag = "Unlockable";

    [Tooltip("当所有解锁目标都已解锁时，显示这个最终图片/Panel（先保持关闭）")]
    public GameObject finalRewardObject;

    [Header("Auto Check")]
    [Tooltip("开始时检查一次（适合从存档恢复）")]
    public bool checkOnStart = true;

    [Tooltip("每隔一段时间自动检查（如果解锁发生在别的脚本里但你没手动通知它）")]
    public bool periodicCheck = true;

    [Min(0.05f)]
    public float checkInterval = 0.5f;

    private float _timer;

    private void Start()
    {
        if (finalRewardObject != null)
            finalRewardObject.SetActive(false);

        if (checkOnStart)
            EvaluateAllUnlocks();

        _timer = checkInterval;
    }

    private void Update()
    {
        if (!periodicCheck) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = checkInterval;
            EvaluateAllUnlocks();
        }
    }

    /// <summary>
    /// 在任意谜题“成功”的那一刻调用这个函数。
    /// 传入：那个被解锁的目标物体（必须挂 UnlockableItem）
    /// </summary>
    public void MarkUnlockedAndEvaluate(UnlockableItem unlockTarget)
    {
        if (unlockTarget != null)
            unlockTarget.MarkUnlocked();

        EvaluateAllUnlocks();
    }

    /// <summary>
    /// 只做全局检查：是否所有 Unlockable 都解锁了
    /// </summary>
    public void EvaluateAllUnlocks()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(unlockableTag);

        // 场景里一个都没找到 -> 认为未完成（更安全）
        if (objs == null || objs.Length == 0)
        {
            SetFinal(false);
            return;
        }

        bool allUnlocked = true;

        for (int i = 0; i < objs.Length; i++)
        {
            UnlockableItem u = objs[i].GetComponent<UnlockableItem>();
            if (u == null)
            {
                Debug.LogWarning($"[PuzzleManager] Object '{objs[i].name}' has tag '{unlockableTag}' but no UnlockableItem.", objs[i]);
                allUnlocked = false;
                continue;
            }

            if (!u.IsUnlocked())
            {
                allUnlocked = false;
                break;
            }
        }

        SetFinal(allUnlocked);
    }

    private void SetFinal(bool show)
    {
        if (finalRewardObject == null) return;

        if (finalRewardObject.activeSelf != show)
            finalRewardObject.SetActive(show);
    }

    // 可选：测试用，清空当前场景所有解锁（别在正式版乱用）
    public void DebugResetAllFoundUnlocks()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(unlockableTag);
        foreach (var go in objs)
        {
            var u = go.GetComponent<UnlockableItem>();
            if (u != null) u.ResetUnlocked();
        }
        EvaluateAllUnlocks();
    }
}
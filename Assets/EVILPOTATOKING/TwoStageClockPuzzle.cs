using UnityEngine;

/// <summary>
/// 一个时钟，依次解两个时间：
/// 先解 Time1 -> 左侧方块1 变绿；
/// Time1 正确之后，才会开始检查 Time2 -> 方块2 变绿。
/// 通过把指针角度“换算成当前时间”再对比，不用角度误差，更稳定。
/// </summary>
public class TwoStageClockPuzzle : MonoBehaviour
{
    [Header("指针")]
    public Transform hourHand;      // 时针
    public Transform minuteHand;    // 分针

    [Header("第一个目标时间 (Time 1)")]
    [Range(0, 23)] public int targetHour1 = 3;
    [Range(0, 59)] public int targetMinute1 = 30;

    [Header("第二个目标时间 (Time 2)")]
    [Range(0, 23)] public int targetHour2 = 7;
    [Range(0, 59)] public int targetMinute2 = 45;

    [Header("左侧两个方块（SpriteRenderer）")]
    public SpriteRenderer stage1Box;    // 第一关状态方块
    public SpriteRenderer stage2Box;    // 第二关状态方块

    [Header("颜色设置")]
    public Color correctColor = Color.green; // 正确
    public Color wrongColor = Color.red;     // 错误
    public Color lockedColor = Color.gray;   // 第二关未解锁

    private bool stage1Solved = false;
    private bool stage2Solved = false;

    private void Start()
    {
        if (stage1Box != null) stage1Box.color = wrongColor;
        if (stage2Box != null) stage2Box.color = lockedColor;
    }

    private void Update()
    {
        // 先把当前指针角度转换成“当前时间”
        GetCurrentTime(out int currentHour, out int currentMinute);

        if (!stage1Solved)
        {
            // 检查第一组目标时间
            int th1 = NormalizeHour12(targetHour1);
            bool solved1 = (currentHour == th1) && (currentMinute == targetMinute1);

            stage1Solved = solved1;

            if (stage1Box != null)
                stage1Box.color = solved1 ? correctColor : wrongColor;

            if (stage2Box != null)
                stage2Box.color = lockedColor;   // 第二关未解锁
        }
        else
        {
            // 第一关已经通过，开始检查第二组时间
            int th2 = NormalizeHour12(targetHour2);
            bool solved2 = (currentHour == th2) && (currentMinute == targetMinute2);

            stage2Solved = solved2;

            if (stage1Box != null)
                stage1Box.color = correctColor;  // 保持绿

            if (stage2Box != null)
                stage2Box.color = solved2 ? correctColor : wrongColor;
        }
    }

    // ==================== 时间换算部分 ====================

    // 把 0/13/14 之类的转换到 1~12 制
    private int NormalizeHour12(int hour24)
    {
        int h12 = hour24 % 12;
        if (h12 == 0) h12 = 12;
        return h12;
    }

    // 把指针当前角度转换为“当前小时+分钟”（12 小时制）
    private void GetCurrentTime(out int currentHour, out int currentMinute)
    {
        if (hourHand == null || minuteHand == null)
        {
            currentHour = 12;
            currentMinute = 0;
            return;
        }

        // 1）先算分针 —— 用分针角度推分钟
        // Unity：z=0 在右，逆时针为正，所以我们改成：
        //   上方（12点方向）为 0 度，顺时针为正
        float minuteZ = minuteHand.eulerAngles.z;
        float minuteCW = Mathf.Repeat(-minuteZ + 90f, 360f);
        float minuteRaw = minuteCW / 6f;              // 每 6 度 = 1 分钟
        currentMinute = Mathf.RoundToInt(minuteRaw) % 60;

        // 2）再算时针 —— 把他换算成 1~12 的小时数
        float hourZ = hourHand.eulerAngles.z;
        float hourCW = Mathf.Repeat(-hourZ + 90f, 360f);
        float hourRaw = hourCW / 30f;                 // 每 30 度 = 1 小时
        int h12 = Mathf.RoundToInt(hourRaw) % 12;
        if (h12 == 0) h12 = 12;
        currentHour = h12;
    }

    // 可选：如果你后面别的地方需要知道是否全解了
    public bool IsAllSolved()
    {
        return stage1Solved && stage2Solved;
    }
}

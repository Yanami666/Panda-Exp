using UnityEngine;

/// <summary>
/// 挂在“时间方块”上的脚本。
/// 点击方块时，把同一场景里的时钟调整到指定时间。
/// 只用这一个脚本就能完成：显示 + 切换时间。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ClockTimeDisplay : MonoBehaviour
{
    [Header("时钟指针引用（同一套时钟的指针）")]
    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;

    [Header("这个方块对应的时间")]
    [Range(0, 23)] public int hour;
    [Range(0, 59)] public int minute;
    [Range(0, 59)] public int second;

    private void OnMouseDown()
    {
        SetClockToThisTime();
    }

    /// <summary>
    /// 把时钟指针旋转到这个方块对应的时间
    /// </summary>
    public void SetClockToThisTime()
    {
        int h12 = hour % 12;

        // 时针：每小时 30 度 + 每分钟 0.5 度，顺时针为负
        if (hourHand != null)
        {
            float hourAngle = -(h12 * 30f + minute * 0.5f);
            hourHand.localEulerAngles = new Vector3(0f, 0f, hourAngle);
        }

        // 分针：每分钟 6 度
        if (minuteHand != null)
        {
            float minuteAngle = -(minute * 6f);
            minuteHand.localEulerAngles = new Vector3(0f, 0f, minuteAngle);
        }

        // 秒针：每秒 6 度（如果你不用秒针，可以 secondHand 留空）
        if (secondHand != null)
        {
            float secondAngle = -(second * 6f);
            secondHand.localEulerAngles = new Vector3(0f, 0f, secondAngle);
        }
    }
}

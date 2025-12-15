using UnityEngine;

public class TwoStageClockPuzzle : MonoBehaviour
{
    [Header("Hands")]
    public Transform hourHand;
    public Transform minuteHand;

    [Header("Time 1")]
    [Range(0, 23)] public int targetHour1 = 3;
    [Range(0, 59)] public int targetMinute1 = 30;

    [Header("Time 2")]
    [Range(0, 23)] public int targetHour2 = 7;
    [Range(0, 59)] public int targetMinute2 = 45;

    [Header("Stage Boxes")]
    public SpriteRenderer stage1Box;
    public SpriteRenderer stage2Box;

    [Header("Colors")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color lockedColor = Color.gray;

    [Header("Reveal On Stage 2 Solved")]
    public GameObject revealSpriteRoot;   // 这里放你要弹出的那张 sprite 的物体（建议一开始隐藏）
    public bool hideRevealOnStart = true;

    private bool stage1Solved = false;
    private bool stage2Solved = false;

    private bool hasRevealed = false;

    private void Start()
    {
        if (stage1Box != null) stage1Box.color = wrongColor;
        if (stage2Box != null) stage2Box.color = lockedColor;

        if (revealSpriteRoot != null && hideRevealOnStart)
            revealSpriteRoot.SetActive(false);
    }

    private void Update()
    {
        GetCurrentTime(out int currentHour, out int currentMinute);

        if (!stage1Solved)
        {
            int th1 = NormalizeHour12(targetHour1);
            bool solved1 = (currentHour == th1) && (currentMinute == targetMinute1);

            stage1Solved = solved1;

            if (stage1Box != null)
                stage1Box.color = solved1 ? correctColor : wrongColor;

            if (stage2Box != null)
                stage2Box.color = lockedColor;
        }
        else
        {
            int th2 = NormalizeHour12(targetHour2);
            bool solved2 = (currentHour == th2) && (currentMinute == targetMinute2);

            stage2Solved = solved2;

            if (stage1Box != null)
                stage1Box.color = correctColor;

            if (stage2Box != null)
                stage2Box.color = solved2 ? correctColor : wrongColor;

            // 只在第二阶段第一次变成 solved 的那一刻弹出
            if (stage2Solved && !hasRevealed)
            {
                hasRevealed = true;
                if (revealSpriteRoot != null)
                    revealSpriteRoot.SetActive(true);
            }
        }
    }

    private int NormalizeHour12(int hour24)
    {
        int h12 = hour24 % 12;
        if (h12 == 0) h12 = 12;
        return h12;
    }

    private void GetCurrentTime(out int currentHour, out int currentMinute)
    {
        if (hourHand == null || minuteHand == null)
        {
            currentHour = 12;
            currentMinute = 0;
            return;
        }

        float minuteZ = minuteHand.eulerAngles.z;
        float minuteCW = Mathf.Repeat(-minuteZ + 90f, 360f);
        float minuteRaw = minuteCW / 6f;
        currentMinute = Mathf.RoundToInt(minuteRaw) % 60;

        float hourZ = hourHand.eulerAngles.z;
        float hourCW = Mathf.Repeat(-hourZ + 90f, 360f);
        float hourRaw = hourCW / 30f;
        int h12 = Mathf.RoundToInt(hourRaw) % 12;
        if (h12 == 0) h12 = 12;
        currentHour = h12;
    }

    public bool IsAllSolved()
    {
        return stage1Solved && stage2Solved;
    }
}

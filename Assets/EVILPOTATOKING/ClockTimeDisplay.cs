using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ClockTimeDisplay : MonoBehaviour
{
    [Header("Clock Hands")]
    public Transform hourHand;
    public Transform minuteHand;

    [Header("Target Time")]
    [Range(0, 23)] public int hour;
    [Range(0, 59)] public int minute;

    private void OnMouseDown()
    {
        SetClockToThisTime();
    }

    public void SetClockToThisTime()
    {
        int h12 = hour % 12;

        if (hourHand != null)
        {
            float hourAngle = -(h12 * 30f + minute * 0.5f);
            hourHand.localEulerAngles = new Vector3(0f, 0f, hourAngle);
        }

        if (minuteHand != null)
        {
            float minuteAngle = -(minute * 6f);
            minuteHand.localEulerAngles = new Vector3(0f, 0f, minuteAngle);
        }
    }
}
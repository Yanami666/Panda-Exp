using UnityEngine;

public class RevealClickableSquare : MonoBehaviour
{
    [Header("要显示的那个 Clickable Square（场景里的实例）")]
    public GameObject clickableSquare;

    [Header("是否把方块移动到当前物体附近")]
    public bool moveSquareToThisObject = true;

    [Tooltip("以当前物体为中心的偏移量")]
    public Vector3 offset = new Vector3(0f, 1f, 0f);

    [Header("只触发一次？")]
    public bool onlyOnce = false;

    private bool hasTriggered = false;

    void Start()
    {
        if (clickableSquare != null)
        {
            clickableSquare.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (hasTriggered && onlyOnce) return;
        if (clickableSquare == null) return;

        // 显示 square
        clickableSquare.SetActive(true);

        // 可选：移动到物体附近
        if (moveSquareToThisObject)
        {
            Vector3 targetPos = transform.position + offset;
            clickableSquare.transform.position = new Vector3(
                targetPos.x,
                targetPos.y,
                clickableSquare.transform.position.z
            );
        }

        hasTriggered = true;
    }
}
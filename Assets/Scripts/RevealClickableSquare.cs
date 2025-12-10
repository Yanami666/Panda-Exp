using UnityEngine;

public class RevealClickableSquare : MonoBehaviour
{
    [Header("要显示的那个 Clickable Square（场景里的实例）")]
    public GameObject clickableSquare;

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
        {
            clickableSquare.SetActive(false);
        }
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

        // ⭐⭐ 关键：抽屉执行完一次后，不再有 collider，避免和方块互相“抢Raycast”
        if (onlyOnce && myCollider != null)
            myCollider.enabled = false;
    }
}
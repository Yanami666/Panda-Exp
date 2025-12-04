using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChessPuzzlePiece : MonoBehaviour
{
    public int pieceID = 0;
    public LayerMask slotLayerMask;

    [HideInInspector] public bool isPlaced = false;

    private Vector3 startPos;
    private float startZ;
    private Vector3 dragOffset;

    private void Start()
    {
        startPos = transform.position;
        startZ = transform.position.z;
    }

    public void BeginDrag(Camera cam, Vector3 mouseScreenPos)
    {
        if (isPlaced || cam == null) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorld.z = startZ;
        dragOffset = transform.position - mouseWorld;
    }

    public void Drag(Camera cam, Vector3 mouseScreenPos)
    {
        if (isPlaced || cam == null) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorld.z = startZ;
        transform.position = mouseWorld + dragOffset;
    }

    public void EndDrag()
    {
        if (isPlaced) return;

        Vector2 point = transform.position;
        Collider2D hit = Physics2D.OverlapPoint(point, slotLayerMask);

        if (hit != null)
        {
            ChessPuzzleSlot slot = hit.GetComponent<ChessPuzzleSlot>();

            if (slot != null && !slot.isOccupied && slot.requiredPieceID == pieceID)
            {
                // ✅ 放在正确格子：自动对齐 + 锁死，不能再拖
                // Correct slot: snap + lock
                transform.position = slot.transform.position;
                isPlaced = true;
                slot.isOccupied = true;

                if (ChessPuzzleManager.Instance != null)
                {
                    ChessPuzzleManager.Instance.RegisterCorrectPlaced();
                }
                return;
            }
        }

        // ❌ 放错地方：现在不再回到 startPos，而是停在玩家放下的位置
        // Wrong place: DO NOT return to start, just stay where player left it.
        // （如果以后你想做“重置关卡”，startPos 以后还能用）
        // (startPos is kept for potential "reset" feature later)
    }
}
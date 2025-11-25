using UnityEngine;
using UnityEngine.InputSystem; // 新输入系统 / New Input System

/// <summary>
/// 全局棋子拖拽控制器（Input System 版本）
/// Global drag controller for chess pieces (Input System version).
/// </summary>
public class ChessPuzzleDragController : MonoBehaviour
{
    [Header("用于拖拽的相机（2D 主相机） / Camera used for drag")]
    public Camera dragCamera;

    private ChessPuzzlePiece currentPiece = null;

    private void Awake()
    {
        if (dragCamera == null)
        {
            dragCamera = Camera.main;
        }

        if (dragCamera == null)
        {
            Debug.LogError("ChessPuzzleDragController: 没有拖拽相机，请在 Inspector 里给 dragCamera 赋值一个 2D 相机。");
        }
    }

    private void Update()
    {
        if (dragCamera == null)
            return;

        // 使用新输入系统的 Mouse.current
        // Use new Input System's Mouse.current
        var mouse = Mouse.current;
        if (mouse == null)
            return;

        Vector2 mousePos = mouse.position.ReadValue();

        // 鼠标按下：尝试选中一个棋子
        // Mouse down: try pick a piece
        if (mouse.leftButton.wasPressedThisFrame)
        {
            TryPickPiece(mousePos);
        }

        // 鼠标按住：拖动当前棋子
        // Mouse held: drag current piece
        if (mouse.leftButton.isPressed && currentPiece != null)
        {
            currentPiece.Drag(
                dragCamera,
                new Vector3(mousePos.x, mousePos.y, 0f)
            );
        }

        // 鼠标抬起：结束拖拽
        // Mouse up: end drag
        if (mouse.leftButton.wasReleasedThisFrame && currentPiece != null)
        {
            currentPiece.EndDrag();
            currentPiece = null;
        }
    }

    private void TryPickPiece(Vector2 mousePos)
    {
        Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, 0f);
        Ray ray = dragCamera.ScreenPointToRay(screenPos);

        // 2D 世界空间的射线检测 / 2D raycast
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            ChessPuzzlePiece piece = hit.collider.GetComponent<ChessPuzzlePiece>();
            if (piece != null && !piece.isPlaced)
            {
                currentPiece = piece;
                currentPiece.BeginDrag(dragCamera, screenPos);
            }
        }
    }
}
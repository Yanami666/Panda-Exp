using UnityEngine;

public class GridCheck : MonoBehaviour
{
    [Header("Need the same origin/cellSize as GridMover")]
    public Transform origin;     // 左上角(0,0)格子中心
    public float cellSize = 1f;

    [Header("Grid Size")]
    public int rows = 6; // y: 0..5
    public int cols = 7; // x: 0..6

    [Header("Key")]
    public KeyCode printKey = KeyCode.C;

    void Update()
    {
        if (Input.GetKeyDown(printKey))
        {
            Vector2Int cell = WorldToCell(transform.position);
            Debug.Log($"GridCheck Cell = ({cell.x}, {cell.y})  World = {transform.position}");
        }
    }

    Vector2Int WorldToCell(Vector3 world)
    {
        if (origin == null) return new Vector2Int(999, 999);

        // (0,0)在左上角：世界坐标向下是 -Y，所以 y 用 (originY - worldY)
        float dx = (world.x - origin.position.x) / cellSize;
        float dy = (origin.position.y - world.y) / cellSize;

        int x = Mathf.RoundToInt(dx);
        int y = Mathf.RoundToInt(dy);

        // 可选：夹到范围内，避免显示 -1/7 之类
        x = Mathf.Clamp(x, 0, cols - 1);
        y = Mathf.Clamp(y, 0, rows - 1);

        return new Vector2Int(x, y);
    }
}

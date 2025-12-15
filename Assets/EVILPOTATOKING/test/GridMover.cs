using System;
using System.Collections.Generic;
using UnityEngine;

public class GridMover : MonoBehaviour
{
    [Header("Grid Size (6x7)")]
    public int rows = 6;   // Y
    public int cols = 7;   // X

    [Header("Grid Placement")]
    public Transform origin;     // 格子(0,0)对应的世界坐标（建议放在“左下角格子中心”）
    public float cellSize = 1f;  // 每个格子的世界间距（中心到中心）

    [Header("Level Cells")]
    public Vector2Int startCell;
    public Vector2Int keyCell;
    public Vector2Int exitCell;

    [Header("Walls / Blocks")]
    [Tooltip("这些坐标表示：站在该格子上往下走会撞到“下方墙”，触发回到起点")]
    public List<Vector2Int> downWallCells = new List<Vector2Int>();

    [Tooltip("完全不可进入的格子（可选）。尝试走进去时会被挡住，原地不动。")]
    public List<Vector2Int> blockedCells = new List<Vector2Int>();

    [Header("Colors")]
    public Color keyReachedColor = Color.yellow;

    public Action OnExitReached;

    Vector2Int _cell;
    bool _hasKey;

    SpriteRenderer _sr;
    Color _defaultColor;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null) _defaultColor = _sr.color;
    }

    void Start()
    {
        ResetToStart();
    }

    void Update()
    {
        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector2Int.right;

        if (dir != Vector2Int.zero)
            TryMove(dir);
    }

    public void ApplyLevel(Vector2Int start, Vector2Int key, Vector2Int exit, Color keyColor,
        List<Vector2Int> downWalls, List<Vector2Int> blocked)
    {
        startCell = start;
        keyCell = key;
        exitCell = exit;
        keyReachedColor = keyColor;

        downWallCells = downWalls ?? new List<Vector2Int>();
        blockedCells = blocked ?? new List<Vector2Int>();

        ResetToStart();
    }

    public void ResetToStart()
    {
        _cell = startCell;
        _hasKey = false;

        if (_sr != null) _sr.color = _defaultColor;

        SnapToCell(_cell);
    }

    void TryMove(Vector2Int dir)
    {
        // 规则：只有“向下走”时，若当前格子下方是墙 => 回起点
        if (dir == Vector2Int.down && ContainsCell(downWallCells, _cell))
        {
            ResetToStart();
            return;
        }

        Vector2Int next = _cell + dir;

        if (!InBounds(next))
            return;

        if (ContainsCell(blockedCells, next))
            return;

        _cell = next;
        SnapToCell(_cell);

        // 到达钥匙点：变色 + 获得钥匙
        if (!_hasKey && _cell == keyCell)
        {
            _hasKey = true;
            if (_sr != null) _sr.color = keyReachedColor;
        }

        // 到达终点：必须先到过钥匙点
        if (_hasKey && _cell == exitCell)
        {
            OnExitReached?.Invoke();
        }
    }

    void SnapToCell(Vector2Int c)
    {
        if (origin == null) return;

        Vector3 pos = origin.position + new Vector3(c.x * cellSize, c.y * cellSize, 0f);
        transform.position = pos;
    }

    bool InBounds(Vector2Int c)
    {
        return c.x >= 0 && c.x < cols && c.y >= 0 && c.y < rows;
    }

    bool ContainsCell(List<Vector2Int> list, Vector2Int cell)
    {
        if (list == null) return false;
        for (int i = 0; i < list.Count; i++)
            if (list[i] == cell) return true;
        return false;
    }
}

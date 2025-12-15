using System;
using UnityEngine;

public class GridMover : MonoBehaviour
{
    [Header("Grid (6x7)")]
    public int rows = 6;
    public int cols = 7;

    [Header("Grid Placement")]
    [Tooltip("格子(0,0)（左上角格子中心）的世界坐标")]
    public Transform origin;
    public float cellSize = 1f;

    [Header("Level Cells (0,0 is TOP-LEFT)")]
    public Vector2Int startCell;
    public Vector2Int keyCell;
    public Vector2Int exitCell;

    [Header("Walls (fixed for all levels)")]
    public LayerMask wallMask;
    public float wallCastRadius = 0.04f;

    [Header("Visual (forced by code)")]
    [Tooltip("强制把玩家放到这个 Sorting Layer。建议新建一个 Sorting Layer 叫 Player")]
    public string playerSortingLayer = "Player";

    [Tooltip("强制玩家排序（越大越在上面）")]
    public int playerOrder = 999;

    [Tooltip("玩家默认颜色（会强制 alpha=1）")]
    public Color defaultColor = Color.white;

    [Tooltip("到达 key 后的颜色（会强制 alpha=1）")]
    public Color keyReachedColor = Color.yellow;

    public Action OnExitReached;

    Vector2Int _cell;
    bool _hasKey;

    SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null)
            _sr = GetComponentInChildren<SpriteRenderer>(true);
    }

    void Start()
    {
        ResetToStart();
    }

    void LateUpdate()
    {
        ForceVisible();
    }

    void Update()
    {
        Vector2Int step = Vector2Int.zero;

        // WASD 或方向键都可移动（不改其它逻辑）
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            step = new Vector2Int(0, -1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            step = new Vector2Int(0, +1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            step = new Vector2Int(-1, 0);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            step = new Vector2Int(+1, 0);

        if (step != Vector2Int.zero)
            TryMove(step);
    }

    public void ApplyLevel(Vector2Int start, Vector2Int key, Vector2Int exit, Color keyColor)
    {
        startCell = start;
        keyCell = key;
        exitCell = exit;
        keyReachedColor = keyColor;

        ResetToStart();
    }

    public void ResetToStart()
    {
        _cell = startCell;
        _hasKey = false;

        SetPlayerColor(defaultColor);
        SnapToCell(_cell);
    }

    void TryMove(Vector2Int step)
    {
        Vector2Int next = _cell + step;

        if (!InBounds(next))
            return;

        if (HitsWallBetween(_cell, next))
        {
            ResetToStart();
            return;
        }

        _cell = next;
        SnapToCell(_cell);

        if (!_hasKey && _cell == keyCell)
        {
            _hasKey = true;
            SetPlayerColor(keyReachedColor);
        }

        if (_hasKey && _cell == exitCell)
        {
            OnExitReached?.Invoke();
        }
    }

    void SetPlayerColor(Color c)
    {
        c.a = 1f;
        if (_sr != null) _sr.color = c;
    }

    void ForceVisible()
    {
        if (_sr == null) return;

        _sr.enabled = true;

        if (!string.IsNullOrEmpty(playerSortingLayer))
            _sr.sortingLayerName = playerSortingLayer;

        _sr.sortingOrder = playerOrder;

        Color c = _sr.color;
        if (c.a < 1f) { c.a = 1f; _sr.color = c; }

        if (_sr.sprite == null)
            Debug.LogWarning("[GridMover] Player SpriteRenderer has no Sprite assigned.");
    }

    bool HitsWallBetween(Vector2Int from, Vector2Int to)
    {
        if (origin == null) return false;

        Vector2 a = CellToWorld(from);
        Vector2 b = CellToWorld(to);

        RaycastHit2D hit = Physics2D.CircleCast(
            a,
            wallCastRadius,
            (b - a).normalized,
            Vector2.Distance(a, b),
            wallMask
        );

        return hit.collider != null;
    }

    void SnapToCell(Vector2Int c)
    {
        if (origin == null) return;
        transform.position = CellToWorld(c);
    }

    Vector2 CellToWorld(Vector2Int c)
    {
        return (Vector2)origin.position + new Vector2(c.x * cellSize, -c.y * cellSize);
    }

    bool InBounds(Vector2Int c)
    {
        return c.x >= 0 && c.x < cols && c.y >= 0 && c.y < rows;
    }
}

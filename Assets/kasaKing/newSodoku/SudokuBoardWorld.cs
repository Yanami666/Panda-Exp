using UnityEngine;

public class SudokuBoardWorld : MonoBehaviour
{
    [Header("格子预制体（SudokuCellWorld）")]
    public SudokuCellWorld cellPrefab;

    [Header("棋盘起点（左上角世界坐标）")]
    public Vector2 topLeft = new Vector2(-4.0f, 4.0f);

    [Header("格子间距（中心到中心）")]
    public float cellSize = 0.9f;

    [Header("解出数独后要显示的纸条对象")]
    public GameObject noteObject;   // 拖 Note 进来（整个 Note 根物体）

    [Header("纸条在棋盘下的本地位置偏移（解出后会应用）")]
    public Vector3 noteLocalOffset = new Vector3(4.5f, -4.5f, 0f);

    [Header("可选：强制纸条渲染在最上层（不想改 Inspector 就勾这个）")]
    public bool forceNoteOnTop = true;
    public int noteSortingOrder = 50;

    public SudokuCellWorld[,] cells = new SudokuCellWorld[9, 9];

    private SudokuCellWorld currentSelected;
    private bool isSolved = false;

    // ✅ 防止反复开关 Overlay 时重复生成格子
    private bool boardGenerated = false;

    // ====== 你的数独题目（0 表示空格）======
    private int[,] puzzle = new int[9, 9]
    {
        {8,5,3, 0,7,0, 2,0,1},
        {0,6,0, 9,8,0, 5,4,3},
        {4,9,0, 5,3,1, 7,0,6},

        {9,0,7, 0,1,5, 8,6,0},
        {1,4,0, 2,9,8, 0,5,7},
        {3,8,5, 4,0,7, 1,0,9},

        {6,0,4, 0,2,3, 9,1,5},
        {5,3,8, 1,0,9, 0,7,2},
        {2,0,9, 0,5,6, 0,3,8}
    };

    // ====== 对应的正确解（用来检测是否解对）======
    private int[,] solution = new int[9, 9]
    {
        {8,5,3, 6,7,4, 2,9,1},
        {7,6,1, 9,8,2, 5,4,3},
        {4,9,2, 5,3,1, 7,8,6},

        {9,2,7, 3,1,5, 8,6,4},
        {1,4,6, 2,9,8, 3,5,7},
        {3,8,5, 4,6,7, 1,2,9},

        {6,7,4, 8,2,3, 9,1,5},
        {5,3,8, 1,4,9, 6,7,2},
        {2,1,9, 7,5,6, 4,3,8}
    };

    private void Start()
    {
        // ✅ 避免重复生成
        if (boardGenerated) return;
        boardGenerated = true;

        // ✅ 关键：用“当前相机中心”算 topLeft，保证棋盘出现在相机正中间
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 camPos = cam.transform.position;
            topLeft = new Vector2(camPos.x - 4f * cellSize, camPos.y + 4f * cellSize);
        }

        GenerateBoard();

        // 纸条一开始要隐藏
        if (noteObject != null)
            noteObject.SetActive(false);
    }

    private void Update()
    {
        HandleMouseClick();
        HandleKeyboardInput();
    }

    private void GenerateBoard()
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                Vector2 pos = new Vector2(
                    topLeft.x + c * cellSize,
                    topLeft.y - r * cellSize
                );

                SudokuCellWorld newCell =
                    Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                newCell.name = $"Cell_{r}_{c}";
                cells[r, c] = newCell;

                int startValue = puzzle[r, c];
                bool isGiven = startValue != 0;
                newCell.Init(startValue, isGiven);
            }
        }
    }

    private void HandleMouseClick()
    {
        if (isSolved) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreen = Input.mousePosition;

            float depth = -Camera.main.transform.position.z;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(mouseScreen.x, mouseScreen.y, depth)
            );

            Vector2 point = new Vector2(mouseWorld.x, mouseWorld.y);

            Collider2D hit = Physics2D.OverlapPoint(point);

            if (hit != null)
            {
                SudokuCellWorld cell = hit.GetComponent<SudokuCellWorld>();
                if (cell != null)
                    SelectCell(cell);
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (isSolved) return;

        if (currentSelected == null) return;
        if (currentSelected.isGiven) return;

        bool changed = false;

        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                currentSelected.SetValue(i);
                changed = true;
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetKeyDown(KeyCode.Delete) ||
            Input.GetKeyDown(KeyCode.Alpha0) ||
            Input.GetKeyDown(KeyCode.Keypad0))
        {
            currentSelected.SetValue(0);
            changed = true;
        }

        if (changed)
            CheckSolved();
    }

    private void SelectCell(SudokuCellWorld newCell)
    {
        if (newCell.isGiven)
            return;

        if (currentSelected != null && currentSelected != newCell)
            currentSelected.SetSelected(false);

        currentSelected = newCell;
        currentSelected.SetSelected(true);
    }

    private void CheckSolved()
    {
        if (!IsSolved())
            return;

        isSolved = true;

        if (currentSelected != null)
        {
            currentSelected.SetSelected(false);
            currentSelected = null;
        }

        // 隐藏所有格子
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (cells[r, c] != null)
                    cells[r, c].gameObject.SetActive(false);
            }
        }

        ShowNoteAtBoard();

        Debug.Log("Sudoku solved! 显示纸条。");
    }

    private void ShowNoteAtBoard()
    {
        if (noteObject == null)
        {
            Debug.LogWarning("noteObject 没有绑定！");
            return;
        }

        noteObject.SetActive(true);

        // ✅ 让纸条出现在当前相机正中间（世界坐标）
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 camPos = cam.transform.position;

            // 保持 note 原来的 Z，避免跑到相机后面/前面
            float z = noteObject.transform.position.z;

            noteObject.transform.position = new Vector3(camPos.x, camPos.y, z);
        }

        // 可选：强制排序到最上层，避免被挡住
        if (forceNoteOnTop)
        {
            var srs = noteObject.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sr in srs)
                sr.sortingOrder = noteSortingOrder;
        }
    }

    private bool IsSolved()
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                int v = cells[r, c].value;
                if (v != solution[r, c])
                    return false;
            }
        }
        return true;
    }
}
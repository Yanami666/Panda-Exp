using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SudokuGridGenerator : MonoBehaviour
{
    [Header("数独格子的预制体（TMP_InputField）")]
    public GameObject cellPrefab;

    [Header("行列数（默认 9x9）")]
    public int gridSize = 9;

    [Header("数独完成后要显示的纸条 Panel")]
    public GameObject notePanel;

    [Header("填错时显示的红色提示文本（Try Again）")]
    public TextMeshProUGUI errorText;

    private TMP_InputField[] allCells; // 81 个格子缓存
    private Coroutine hideErrorCoroutine; // 控制 2 秒后隐藏的协程

    // ====== 你的数独题目（0 表示空格） ======
    private int[,] puzzle = new int[9, 9]
    {
        {0,0,3, 0,0,0, 2,0,0},
        {0,6,0, 9,8,0, 0,4,3},
        {4,9,0, 0,3,1, 0,0,6},

        {9,0,7, 0,0,0, 8,6,0},
        {0,4,0, 0,9,8, 0,0,0},
        {0,0,5, 4,0,7, 1,0,9},

        {6,0,0, 0,0,3, 9,0,5},
        {5,0,8, 1,0,0, 0,7,2},
        {2,0,9, 0,5,6, 0,3,8}
    };
    // ========================================

    void Start()
    {
        GenerateGrid();
        ApplyPuzzle();

        // 纸条一开始隐藏
        if (notePanel != null)
        {
            notePanel.SetActive(false);
        }

        // 错误提示一开始隐藏
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }
    }

    // ------------ 生成 81 个格子 ------------

    void GenerateGrid()
    {
        allCells = new TMP_InputField[gridSize * gridSize];

        int total = gridSize * gridSize;

        for (int i = 0; i < total; i++)
        {
            GameObject cellObj = Instantiate(cellPrefab, transform);
            cellObj.name = "Cell_" + i;

            TMP_InputField cell = cellObj.GetComponent<TMP_InputField>();
            allCells[i] = cell;
        }
    }

    // ------------ 根据 puzzle 预置题目 ------------

    void ApplyPuzzle()
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                int index = r * 9 + c;
                TMP_InputField cell = allCells[index];

                int value = puzzle[r, c];
                if (value != 0)
                {
                    // 填入数字
                    cell.text = value.ToString();

                    // 原题数字：只读
                    cell.interactable = false;

                    // 给固定数字一个灰色背景（可选）
                    Image bg = cell.GetComponent<Image>();
                    if (bg != null)
                    {
                        bg.color = new Color(0.85f, 0.85f, 0.85f);
                    }
                }
            }
        }
    }

    // ------------ Button 调用的检查函数 ------------

    public void CheckSudoku()
    {
        if (!IsSudokuValid())
        {
            Debug.Log("数独还没解对 / 有空格 / 有重复数字。");

            // 显示红色 Try Again（最多显示 2 秒）
            if (errorText != null)
            {
                errorText.text = "Try Again";
                errorText.color = Color.red;
                errorText.gameObject.SetActive(true);

                // 如果之前有协程在跑，先停掉
                if (hideErrorCoroutine != null)
                {
                    StopCoroutine(hideErrorCoroutine);
                }
                hideErrorCoroutine = StartCoroutine(HideErrorAfterDelay(2f));
            }

            // 错的时候确保纸条是关着的
            if (notePanel != null)
            {
                notePanel.SetActive(false);
            }

            return;
        }

        Debug.Log("数独解对了！显示纸条。");

        // 解对了就隐藏错误提示
        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);

            if (hideErrorCoroutine != null)
            {
                StopCoroutine(hideErrorCoroutine);
                hideErrorCoroutine = null;
            }
        }

        if (notePanel != null)
        {
            notePanel.SetActive(true);
        }
    }

    // 2 秒后自动隐藏错误提示
    private IEnumerator HideErrorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (errorText != null)
        {
            errorText.gameObject.SetActive(false);
        }

        hideErrorCoroutine = null;
    }

    // ------------ 核心校验逻辑：行 / 列 / 九宫格都要 1-9 不重复 ------------

    private bool IsSudokuValid()
    {
        int[,] values = new int[9, 9];

        // 1）从所有格子里读出数字
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                int index = r * 9 + c;
                TMP_InputField cell = allCells[index];

                int v;
                if (!int.TryParse(cell.text, out v) || v < 1 || v > 9)
                {
                    // 有空格 / 非 1-9 的数字
                    return false;
                }

                values[r, c] = v;
            }
        }

        // 2）检查每一行
        for (int r = 0; r < 9; r++)
        {
            bool[] seen = new bool[10]; // 1-9 用
            for (int c = 0; c < 9; c++)
            {
                int v = values[r, c];
                if (seen[v]) return false;
                seen[v] = true;
            }
        }

        // 3）检查每一列
        for (int c = 0; c < 9; c++)
        {
            bool[] seen = new bool[10];
            for (int r = 0; r < 9; r++)
            {
                int v = values[r, c];
                if (seen[v]) return false;
                seen[v] = true;
            }
        }

        // 4）检查每个 3x3 宫格
        for (int br = 0; br < 3; br++)
        {
            for (int bc = 0; bc < 3; bc++)
            {
                bool[] seen = new bool[10];

                for (int r = br * 3; r < br * 3 + 3; r++)
                {
                    for (int c = bc * 3; c < bc * 3 + 3; c++)
                    {
                        int v = values[r, c];
                        if (seen[v]) return false;
                        seen[v] = true;
                    }
                }
            }
        }

        return true;
    }
}

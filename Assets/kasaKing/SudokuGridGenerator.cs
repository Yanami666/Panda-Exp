using UnityEngine;

public class SudokuGridGenerator : MonoBehaviour
{
    [Header("数独格子的预制体（TMP_InputField）")]
    public GameObject cellPrefab;

    [Header("行列数（默认 9x9）")]
    public int gridSize = 9;

    private bool hasGenerated = false;

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        if (hasGenerated) return; // 避免重复生成

        if (cellPrefab == null)
        {
            Debug.LogError("SudokuGridGenerator 没有绑定 cellPrefab！");
            return;
        }

        int totalCells = gridSize * gridSize;

        for (int i = 0; i < totalCells; i++)
        {
            GameObject cell = Instantiate(cellPrefab, transform);
            cell.name = "Cell_" + i;
        }

        hasGenerated = true;
    }
}

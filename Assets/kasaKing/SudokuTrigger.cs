using UnityEngine;

public class SudokuTrigger : MonoBehaviour
{
    [Header("点击后要显示的数独面板")]
    public GameObject sudokuPanel;

    void OnMouseDown()
    {
        Debug.Log("SudokuObject 被点击了！");   // ← 用来确认点击事件有没有触发

        if (sudokuPanel != null)
        {
            Debug.Log("激活 SudokuPanel");      // ← 用来确认 panel 有没有成功被赋值
            sudokuPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("SudokuTrigger 没有绑定 sudokuPanel！");
        }
    }
}

using UnityEngine;

public class SudokuClickManager : MonoBehaviour
{
    [Header("用来发射射线的相机（不填就用 Main Camera）")]
    public Camera mainCamera;

    [Header("点击纸本后要显示的数独面板")]
    public GameObject sudokuPanel;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false); // 确保一开始是隐藏的
        }
        else
        {
            Debug.LogWarning("SudokuClickManager 没有绑定 sudokuPanel！");
        }
    }

    void Update()
    {
        // 鼠标左键按下
        if (Input.GetMouseButtonDown(0))
        {
            // 鼠标位置 → 世界坐标
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos2D = new Vector2(worldPos.x, worldPos.y);

            // 在这个点发射一条 2D 射线
            RaycastHit2D hit = Physics2D.Raycast(pos2D, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("点到的物体：" + hit.collider.gameObject.name);

                // 判断有没有点到我们的 SudokuObject
                if (hit.collider.gameObject.CompareTag("SudokuObject"))
                {
                    Debug.Log("点到了 SudokuObject，打开数独面板");

                    if (sudokuPanel != null)
                    {
                        sudokuPanel.SetActive(true);
                    }
                }
            }
        }
    }
}

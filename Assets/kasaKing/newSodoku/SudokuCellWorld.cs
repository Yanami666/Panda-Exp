using UnityEngine;
using TMPro;

public class SudokuCellWorld : MonoBehaviour
{
    [Header("是否是题目给定的格子（不可修改）")]
    public bool isGiven;

    [Header("当前数字（0 表示空）")]
    [Range(0, 9)]
    public int value;

    [Header("显示数字的 TextMeshPro（世界空间）")]
    public TextMeshPro numberText;

    [Header("颜色设置")]
    public Color normalColor = Color.white;
    public Color givenColor = new Color(0.9f, 0.9f, 0.9f);
    public Color selectedColor = new Color(0.4f, 0.8f, 1f);

    private SpriteRenderer sr;
    private bool isSelected = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        RefreshVisual();
    }

    public void Init(int startValue, bool given)
    {
        isGiven = given;
        value = startValue;
        RefreshVisual();
    }

    public void SetPlayerValue(int newValue)
    {
        if (isGiven) return;
        value = Mathf.Clamp(newValue, 0, 9);
        RefreshVisual();
    }

    public void ClearValue()
    {
        if (isGiven) return;
        value = 0;
        RefreshVisual();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (numberText != null)
        {
            numberText.text = value == 0 ? "" : value.ToString();
        }

        if (sr != null)
        {
            if (isSelected)
                sr.color = selectedColor;
            else if (isGiven)
                sr.color = givenColor;
            else
                sr.color = normalColor;
        }
    }

    public void SetValue(int v)
    {
        if (isGiven) return;  // 题目格子不能改

        value = Mathf.Clamp(v, 0, 9);
        RefreshVisual();
    }
}
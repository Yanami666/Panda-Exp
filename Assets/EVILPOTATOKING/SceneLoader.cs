using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// 挂在 UI Button 上，点击按钮就切换场景。
/// 不需要在 Button 的 OnClick 里额外设置任何东西。
/// </summary>
public class SceneLoader : MonoBehaviour, IPointerClickHandler
{
    [Header("要切换到的场景名字（和 Build Settings 里的完全一样）")]
    public string targetSceneName;

    // 点击 UI 元素时会自动调用这个函数
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("SceneLoader: targetSceneName 为空，没法切场景。");
        }
    }
}

using UnityEngine;

public class GoToPcAOnClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (SceneSwapManager.Instance != null)
        {
            SceneSwapManager.Instance.GoToPcA();
        }
        else
        {
            Debug.LogError("SceneSwapManager not found in scene.");
        }
    }
}
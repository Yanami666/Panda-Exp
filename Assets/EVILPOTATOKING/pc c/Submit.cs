using UnityEngine;

public class Submit : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (PasswordPrompt.Instance == null)
        {
            Debug.LogError("场景里没有 PasswordPrompt。");
            return;
        }

        PasswordPrompt.Instance.Submit();
    }
}

using UnityEngine;

public class Submit : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (PasswordPrompt.Instance != null)
            PasswordPrompt.Instance.Submit();
    }
}

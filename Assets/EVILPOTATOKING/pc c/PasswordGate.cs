using UnityEngine;

public class PasswordGate : MonoBehaviour
{
    [Header("Password")]
    public string password = "1234";
    public string hint = "请输入密码";

    [Header("Page Switching")]
    public GameObject pageToShow;
    public GameObject[] pagesToHide;

    private void OnMouseDown()
    {
        if (PasswordPrompt.Instance == null)
        {
            Debug.LogError("场景里没有 PasswordPrompt。把 PasswordPrompt.cs 挂到 Canvas_Prompt 上。");
            return;
        }

        PasswordPrompt.Instance.Open(password, GoNext, hint);
    }

    private void GoNext()
    {
        if (pagesToHide != null)
        {
            for (int i = 0; i < pagesToHide.Length; i++)
                if (pagesToHide[i] != null) pagesToHide[i].SetActive(false);
        }

        if (pageToShow != null)
            pageToShow.SetActive(true);
    }
}

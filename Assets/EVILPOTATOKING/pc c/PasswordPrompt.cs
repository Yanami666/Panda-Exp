using System;
using TMPro;
using UnityEngine;

public class PasswordPrompt : MonoBehaviour
{
    [Header("Root (No Panel Needed)")]
    public GameObject root; // 直接拖 Canvas_Prompt 或 PromptRoot(空物体)

    [Header("TMP UI")]
    public TMP_InputField inputField;
    public TextMeshProUGUI hintText; // 可为空

    [Header("Text")]
    public string defaultHint = "请输入密码";
    public string wrongHint = "密码错误";

    private string expectedPassword = "";
    private Action onSuccess;

    public static PasswordPrompt Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Hide();
    }

    public void Open(string password, Action successAction, string hintOverride = null)
    {
        expectedPassword = password ?? "";
        onSuccess = successAction;

        if (root != null) root.SetActive(true);
        else gameObject.SetActive(true);

        if (hintText != null)
            hintText.text = string.IsNullOrEmpty(hintOverride) ? defaultHint : hintOverride;

        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
            inputField.Select();
        }
        else
        {
            Debug.LogError("PasswordPrompt: 没有绑定 TMP_InputField。请把创建出来的 TMP Input Field 父物体拖进来。");
        }
    }

    // 给“确认 Sprite”调用
    public void Submit()
    {
        if (inputField == null) return;

        string typed = inputField.text ?? "";
        if (typed == expectedPassword)
        {
            var cb = onSuccess;
            Hide();
            cb?.Invoke();
        }
        else
        {
            if (hintText != null) hintText.text = wrongHint;

            inputField.text = "";
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    public void Hide()
    {
        expectedPassword = "";
        onSuccess = null;

        if (hintText != null) hintText.text = "";
        if (inputField != null) inputField.text = "";

        if (root != null) root.SetActive(false);
        else gameObject.SetActive(false);
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordPrompt : MonoBehaviour
{
    [Header("Root (Canvas child / Prompt panel root)")]
    public GameObject root;

    [Header("TMP UI")]
    public TMP_InputField inputField;
    public TextMeshProUGUI hintText;

    [Header("Text")]
    public string defaultHint = "请输入密码";
    public string wrongHint = "密码错误";

    private string expectedPassword;
    private Action onSuccess;
    private Coroutine focusCoroutine;

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

    private void Update()
    {
        if (root == null || !root.activeSelf) return;

        // 回车提交（包含小键盘回车）
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Submit();
        }
    }

    public void Open(string password, Action successAction, string hintOverride = null)
    {
        expectedPassword = password;
        onSuccess = successAction;

        Show();

        if (hintText != null)
            hintText.text = string.IsNullOrEmpty(hintOverride) ? defaultHint : hintOverride;

        if (inputField != null)
        {
            inputField.text = "";
            inputField.interactable = true;

            if (focusCoroutine != null) StopCoroutine(focusCoroutine);
            focusCoroutine = StartCoroutine(FocusNextFrame());
        }
    }

    public void Submit()
    {
        if (inputField == null) return;

        string typed = inputField.text ?? "";

        if (typed == expectedPassword)
        {
            Hide();
            onSuccess?.Invoke();
            return;
        }

        // 错误：清空 + 提示
        inputField.text = "";
        if (hintText != null) hintText.text = wrongHint;

        // 重新聚焦方便继续输
        if (focusCoroutine != null) StopCoroutine(focusCoroutine);
        focusCoroutine = StartCoroutine(FocusNextFrame());
    }

    public void Close()
    {
        Hide();
    }

    private IEnumerator FocusNextFrame()
    {
        yield return null;

        if (inputField == null) yield break;

        EventSystem.current?.SetSelectedGameObject(null);
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void Show()
    {
        if (root != null) root.SetActive(true);
    }

    private void Hide()
    {
        if (root != null) root.SetActive(false);
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordPrompt : MonoBehaviour
{
    [Header("Root (Canvas or empty root)")]
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
            FocusInputNextFrame();
        }
        else
        {
            Debug.LogError("PasswordPrompt: TMP_InputField 未绑定");
        }
    }

    public void Submit()
    {
        if (inputField == null) return;

        string typed = inputField.text ?? "";

        if (typed == expectedPassword)
        {
            Action cb = onSuccess;
            Hide();
            cb?.Invoke();
        }
        else
        {
            if (hintText != null)
                hintText.text = wrongHint;

            inputField.text = "";
            FocusInputNextFrame();
        }
    }

    public void Hide()
    {
        expectedPassword = null;
        onSuccess = null;

        if (hintText != null)
            hintText.text = "";

        if (inputField != null)
            inputField.text = "";

        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);

        focusCoroutine = null;

        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    private void Show()
    {
        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    private void FocusInputNextFrame()
    {
        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);

        focusCoroutine = StartCoroutine(FocusRoutine());
    }

    private IEnumerator FocusRoutine()
    {
        yield return null;

        if (EventSystem.current != null && inputField != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);

            inputField.ActivateInputField();
            inputField.Select();
        }
    }
}

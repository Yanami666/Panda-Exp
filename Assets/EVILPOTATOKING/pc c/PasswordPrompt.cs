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

            StopFocusCoroutine();
            StartFocusCoroutine();
        }
    }

    public void Submit()
    {
        // 如果 UI 根本没显示，就不要继续（避免切场景/隐藏时误触发）
        if (root == null || !root.activeInHierarchy) return;
        if (inputField == null) return;

        string typed = inputField.text ?? "";

        if (typed == expectedPassword)
        {
            Hide();
            onSuccess?.Invoke();
            return;
        }

        inputField.text = "";
        if (hintText != null) hintText.text = wrongHint;

        StopFocusCoroutine();
        StartFocusCoroutine();
    }

    public void Close()
    {
        Hide();
    }

    private void StartFocusCoroutine()
    {
        if (CoroutineRunner.Instance == null)
        {
            UnityEngine.Debug.LogError("CoroutineRunner.Instance not found. Please add CoroutineRunner to a DontDestroyOnLoad object.");
            return;
        }

        focusCoroutine = CoroutineRunner.Instance.StartCoroutine(FocusNextFrame());
    }

    private void StopFocusCoroutine()
    {
        if (focusCoroutine == null) return;

        if (CoroutineRunner.Instance != null)
            CoroutineRunner.Instance.StopCoroutine(focusCoroutine);

        focusCoroutine = null;
    }

    private IEnumerator FocusNextFrame()
    {
        yield return null;

        // 如果这时 UI 已经被隐藏/切走了，就不要再抢焦点
        if (root == null || !root.activeInHierarchy) yield break;
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
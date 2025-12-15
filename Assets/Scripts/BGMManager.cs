using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("Scene -> Music")]
    public AudioClip menuMusic;
    public AudioClip normalMusic;
    public AudioClip abnormalMusic;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 0.8f;
    public float fadeDuration = 0.6f;
    public bool loop = true;

    [Header("Scenes that should use normalMusic (same BGM, no interruption)")]
    public string pcASceneName = "Pc a"; // 你的 pcA 场景名（Build Settings 里完全一致）

    private AudioSource _source;
    private string _currentKey = "";     // 用“音乐分组Key”而不是 sceneName
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
        if (_source == null) _source = gameObject.AddComponent<AudioSource>();

        _source.playOnAwake = false;
        _source.loop = loop;
        _source.volume = 0f;

        // ✅ 既监听加载，也监听 Active Scene 切换（Additive 切换时关键）
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
    }

    private void Start()
    {
        ApplyMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 场景加载时也尝试更新（但会用 Key 避免重复切歌）
        ApplyMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        // ✅ Additive 的 SetActiveScene 会触发这里
        ApplyMusicForScene(newScene.name);
    }

    private void ApplyMusicForScene(string sceneName)
    {
        string key = GetMusicKey(sceneName);
        if (key == _currentKey) return; // ✅ 同一组音乐：不做任何事，不会打断播放

        AudioClip target = GetClipForKey(key);
        _currentKey = key;

        // 没有音乐：淡出并停
        if (target == null)
        {
            StartFadeTo(null);
            return;
        }

        // 如果正在播的就是目标：不切
        if (_source.clip == target && _source.isPlaying)
            return;

        // 如果当前没有播任何东西（比如第一次启动）：直接切到目标（仍然会淡入）
        StartFadeTo(target);
    }

    // ✅ 把多个 scene 映射到同一个 key（这里 normal 与 pcA 共用）
    private string GetMusicKey(string sceneName)
    {
        if (sceneName == "MenuPage") return "MENU";
        if (sceneName == "NormalScene") return "NORMAL";
        if (sceneName == pcASceneName) return "NORMAL";     // ✅ pcA = NORMAL 组（同BGM不打断）
        if (sceneName == "AbnormalScene") return "ABNORMAL";

        return "NONE";
    }

    private AudioClip GetClipForKey(string key)
    {
        if (key == "MENU") return menuMusic;
        if (key == "NORMAL") return normalMusic;
        if (key == "ABNORMAL") return abnormalMusic;
        return null;
    }

    private void StartFadeTo(AudioClip nextClip)
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeTo(nextClip));
    }

    private IEnumerator FadeTo(AudioClip nextClip)
    {
        // Fade out
        float startVol = _source.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            _source.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }
        _source.volume = 0f;

        // Switch clip
        _source.Stop();
        _source.clip = nextClip;

        if (nextClip == null)
            yield break;

        _source.loop = loop;
        _source.Play();

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            _source.volume = Mathf.Lerp(0f, volume, t / fadeDuration);
            yield return null;
        }
        _source.volume = volume;
    }
}
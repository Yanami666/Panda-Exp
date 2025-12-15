using System;
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

    private AudioSource _source;
    private string _currentSceneName = "";
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // In case manager starts inside some scene already loaded
        ApplyMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicForScene(scene.name);
    }

    private void ApplyMusicForScene(string sceneName)
    {
        if (sceneName == _currentSceneName) return;

        AudioClip target = GetClipForScene(sceneName);
        _currentSceneName = sceneName;

        // No clip assigned: fade out and stop
        if (target == null)
        {
            StartFadeTo(null);
            return;
        }

        // Same clip already playing: do nothing
        if (_source.clip == target && _source.isPlaying)
            return;

        StartFadeTo(target);
    }

    private AudioClip GetClipForScene(string sceneName)
    {
        // Use exact scene names as in Build Settings / your screenshot
        if (sceneName == "MenuPage") return menuMusic;
        if (sceneName == "NormalScene") return normalMusic;
        if (sceneName == "AbnormalScene") return abnormalMusic;

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
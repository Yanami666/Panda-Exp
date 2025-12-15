using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance { get; private set; }

    [Header("Scene Names (must match Build Settings exactly)")]
    public string normalSceneName = "NormalScene";
    public string pcASceneName = "Pc a";

    [Header("Keep PC_A loaded to preserve progress")]
    public bool keepPcALoaded = true;

    private bool pcALoadRequested = false;

    // 记录 NormalScene 切走前每个 root 的 active 状态
    private readonly Dictionary<int, bool> normalRootActiveById = new Dictionary<int, bool>();
    private readonly Dictionary<string, bool> normalRootActiveByNameFallback = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToPcA()
    {
        StartCoroutine(GoToPcACoroutine());
    }

    public void GoToNormal()
    {
        StartCoroutine(GoToNormalCoroutine());
    }

    private IEnumerator GoToPcACoroutine()
    {
        // 1) 记录 NormalScene 当前 root 的 active 状态，然后隐藏 NormalScene
        CacheNormalSceneRootStates();
        SetSceneRootsActive(normalSceneName, false);

        // 2) Additive 加载 pc_a（只加载一次）
        if (!IsSceneLoaded(pcASceneName) && !pcALoadRequested)
        {
            pcALoadRequested = true;
            AsyncOperation op = SceneManager.LoadSceneAsync(pcASceneName, LoadSceneMode.Additive);
            while (!op.isDone) yield return null;
        }

        // 3) 切 Active Scene 到 pc_a
        Scene pcScene = SceneManager.GetSceneByName(pcASceneName);
        if (pcScene.IsValid() && pcScene.isLoaded)
            SceneManager.SetActiveScene(pcScene);

        // 4) 确保 pc_a 的 root 可见
        SetSceneRootsActive(pcASceneName, true);
    }

    private IEnumerator GoToNormalCoroutine()
    {
        Scene normal = SceneManager.GetSceneByName(normalSceneName);

        // 极少数情况：NormalScene 没加载（比如你从别处直接进 pc_a）
        if (!normal.IsValid() || !normal.isLoaded)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(normalSceneName, LoadSceneMode.Single);
            while (!op.isDone) yield return null;
            yield break;
        }

        SceneManager.SetActiveScene(normal);

        // ✅ 关键：按记录恢复 NormalScene 每个 root 的 active 状态（不会全开 overlay）
        RestoreNormalSceneRootStates();

        if (keepPcALoaded)
        {
            // 隐藏 pc_a（不卸载，进度保留）
            SetSceneRootsActive(pcASceneName, false);
        }
        else
        {
            // 卸载 pc_a（会重置进度）
            if (IsSceneLoaded(pcASceneName))
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(pcASceneName);
                while (op != null && !op.isDone) yield return null;
            }
        }
    }

    // ---------------- helpers ----------------

    private bool IsSceneLoaded(string sceneName)
    {
        Scene s = SceneManager.GetSceneByName(sceneName);
        return s.IsValid() && s.isLoaded;
    }

    private void CacheNormalSceneRootStates()
    {
        normalRootActiveById.Clear();
        normalRootActiveByNameFallback.Clear();

        Scene normal = SceneManager.GetSceneByName(normalSceneName);
        if (!normal.IsValid() || !normal.isLoaded) return;

        foreach (var go in normal.GetRootGameObjects())
        {
            if (go == gameObject) continue; // 不记录/不操作管理器自己

            normalRootActiveById[go.GetInstanceID()] = go.activeSelf;

            // 兜底：如果返回时 InstanceID 变了（极少），按 name 找回
            if (!normalRootActiveByNameFallback.ContainsKey(go.name))
                normalRootActiveByNameFallback[go.name] = go.activeSelf;
        }
    }

    private void RestoreNormalSceneRootStates()
    {
        Scene normal = SceneManager.GetSceneByName(normalSceneName);
        if (!normal.IsValid() || !normal.isLoaded) return;

        foreach (var go in normal.GetRootGameObjects())
        {
            if (go == gameObject) continue;

            int id = go.GetInstanceID();
            if (normalRootActiveById.TryGetValue(id, out bool wasActive))
            {
                go.SetActive(wasActive);
            }
            else if (normalRootActiveByNameFallback.TryGetValue(go.name, out bool wasActiveByName))
            {
                go.SetActive(wasActiveByName);
            }
            // 如果是新出现的 root（切换期间生成的），这里不强改它，保持当前状态
        }
    }

    private void SetSceneRootsActive(string sceneName, bool active)
    {
        Scene s = SceneManager.GetSceneByName(sceneName);
        if (!s.IsValid() || !s.isLoaded) return;

        foreach (var go in s.GetRootGameObjects())
        {
            if (go == gameObject) continue; // 不要把管理器关掉
            go.SetActive(active);
        }
    }
}
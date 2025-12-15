using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;
    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                // 场景里找现成的
                _instance = FindFirstObjectByType<CoroutineRunner>();

                // 还没有就自动创建
                if (_instance == null)
                {
                    GameObject go = new GameObject("_CoroutineRunner");
                    _instance = go.AddComponent<CoroutineRunner>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
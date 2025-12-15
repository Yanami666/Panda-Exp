using System.Collections.Generic;
using UnityEngine;

public class UnlockAllChecker : MonoBehaviour
{
    [Header("All unlockable objects share this Tag")]
    public string unlockableTag = "Unlockable";

    [Header("Show this when ALL are unlocked")]
    public GameObject finalRewardObject;

    [Header("Check when?")]
    public bool checkOnStart = true;

    [Tooltip("If true, will keep checking every X seconds (useful if unlock happens later)")]
    public bool periodicCheck = true;
    public float checkInterval = 0.5f;

    private float timer;

    private void Start()
    {
        if (finalRewardObject != null)
            finalRewardObject.SetActive(false);

        if (checkOnStart)
            Evaluate();

        timer = checkInterval;
    }

    private void Update()
    {
        if (!periodicCheck) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = checkInterval;
            Evaluate();
        }
    }

    /// <summary>
    /// Call this manually right after any puzzle unlock to update instantly.
    /// </summary>
    public void Evaluate()
    {
        var objs = GameObject.FindGameObjectsWithTag(unlockableTag);

        // No unlockables found => treat as not completed (safer)
        if (objs == null || objs.Length == 0)
        {
            SetFinal(false);
            return;
        }

        bool allUnlocked = true;

        for (int i = 0; i < objs.Length; i++)
        {
            var u = objs[i].GetComponent<UnlockableItem>();
            if (u == null)
            {
                Debug.LogWarning($"[UnlockAllChecker] Object {objs[i].name} has tag '{unlockableTag}' but no UnlockableItem.", objs[i]);
                allUnlocked = false;
                continue;
            }

            if (!u.IsUnlocked())
            {
                allUnlocked = false;
                break;
            }
        }

        SetFinal(allUnlocked);
    }

    private void SetFinal(bool show)
    {
        if (finalRewardObject == null) return;

        if (finalRewardObject.activeSelf != show)
            finalRewardObject.SetActive(show);
    }

    // Optional helper for debugging: clear ALL unlock states found in current scene.
    public void ResetAllFoundUnlocks()
    {
        var objs = GameObject.FindGameObjectsWithTag(unlockableTag);
        foreach (var go in objs)
        {
            var u = go.GetComponent<UnlockableItem>();
            if (u != null) u.ResetUnlocked();
        }
        Evaluate();
    }
}
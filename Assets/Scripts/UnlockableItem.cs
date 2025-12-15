using UnityEngine;

[DisallowMultipleComponent]
public class UnlockableItem : MonoBehaviour
{
    [Header("Unique ID (must be unique across all unlockables)")]
    public string unlockId = "UniqueID";

    [Header("Auto-apply visibility on Start")]
    public bool applyVisibilityOnStart = true;

    [Tooltip("If true: when unlocked -> SetActive(true), locked -> SetActive(false)")]
    public bool controlActiveState = true;

    private const string Prefix = "UNLOCKED_";

    private void Start()
    {
        if (applyVisibilityOnStart)
            ApplyVisibility();
    }

    /// <summary>
    /// Mark this item as unlocked (saved persistently).
    /// </summary>
    public void MarkUnlocked()
    {
        if (string.IsNullOrWhiteSpace(unlockId))
        {
            Debug.LogError($"[UnlockableItem] unlockId is empty on {name}", this);
            return;
        }

        PlayerPrefs.SetInt(Prefix + unlockId, 1);
        PlayerPrefs.Save();

        ApplyVisibility();
    }

    /// <summary>
    /// Returns whether this item is unlocked.
    /// </summary>
    public bool IsUnlocked()
    {
        if (string.IsNullOrWhiteSpace(unlockId)) return false;
        return PlayerPrefs.GetInt(Prefix + unlockId, 0) == 1;
    }

    /// <summary>
    /// Apply visibility according to unlocked state.
    /// </summary>
    public void ApplyVisibility()
    {
        if (!controlActiveState) return;

        bool unlocked = IsUnlocked();
        if (gameObject.activeSelf != unlocked)
            gameObject.SetActive(unlocked);
    }

    /// <summary>
    /// Optional: reset this unlock (for testing).
    /// </summary>
    public void ResetUnlocked()
    {
        if (string.IsNullOrWhiteSpace(unlockId)) return;
        PlayerPrefs.DeleteKey(Prefix + unlockId);
        PlayerPrefs.Save();
        ApplyVisibility();
    }
}
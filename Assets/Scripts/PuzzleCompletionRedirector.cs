using UnityEngine;

/// <summary>
/// After a puzzle is completed, redirect the ClickableSquare to show the clue instead of reopening the puzzle overlay.
/// Attach this to the same GameObject as ClickableSquare (e.g. ClickableSquare_collage).
/// </summary>
[DisallowMultipleComponent]
public class PuzzleCompletionRedirector : MonoBehaviour
{
    [Header("Completion flag (PlayerPrefs key)")]
    [Tooltip("A unique key saved in PlayerPrefs when the puzzle is completed.")]
    public string completionKey = "PUZZLE_DONE_collage";

    [Header("Targets")]
    [Tooltip("The original puzzle overlay/root that ClickableSquare would open.")]
    public GameObject puzzleOverlay;

    [Tooltip("The clue object to show after completion (can be in scene root).")]
    public GameObject clueObject;

    [Header("Optional: If you want the clue to appear at camera center like overlays")]
    public bool moveClueToCameraCenter = true;

    private ClickableSquare clickableSquare;

    private void Awake()
    {
        clickableSquare = GetComponent<ClickableSquare>();
        if (clickableSquare == null)
        {
            Debug.LogError("[PuzzleCompletionRedirector] No ClickableSquare found on this GameObject.", this);
        }
    }

    private void Start()
    {
        ApplyRedirectIfNeeded();
    }

    /// <summary>
    /// Call this when puzzle is completed (or on Start it will auto-apply).
    /// </summary>
    public void MarkCompleted()
    {
        PlayerPrefs.SetInt(completionKey, 1);
        PlayerPrefs.Save();
        ApplyRedirectIfNeeded();
    }

    public bool IsCompleted()
    {
        return PlayerPrefs.GetInt(completionKey, 0) == 1;
    }

    private void ApplyRedirectIfNeeded()
    {
        if (clickableSquare == null) return;

        if (IsCompleted())
        {
            // Redirect: clicking the square will show clueObject instead of puzzleOverlay
            if (clueObject != null)
            {
                clickableSquare.objectToActivate = clueObject;
            }

            // (Optional) prevent reopening overlay even if someone else tries
            if (puzzleOverlay != null)
            {
                // Not forcing disable here; you can keep it off by your close logic.
                // puzzleOverlay.SetActive(false);
            }
        }
        else
        {
            // Not completed yet: keep opening the puzzle overlay
            if (puzzleOverlay != null)
            {
                clickableSquare.objectToActivate = puzzleOverlay;
            }
        }
    }

    private void LateUpdate()
    {
        // If completed and you want the clue to stay centered when opened:
        if (!IsCompleted()) return;
        if (!moveClueToCameraCenter) return;
        if (clueObject == null) return;
        if (!clueObject.activeInHierarchy) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 camPos = cam.transform.position;
        Vector3 p = clueObject.transform.position;
        p.x = camPos.x;
        p.y = camPos.y;
        clueObject.transform.position = p;
    }
}
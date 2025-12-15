using System.Collections.Generic;
using UnityEngine;

public class GridGame : MonoBehaviour
{
    [System.Serializable]
    public class LockSet
    {
        public Sprite closed;
        public Sprite lit;
        public Sprite open;
    }

    [System.Serializable]
    public class Level
    {
        public Vector2Int startCell;
        public Vector2Int keyCell;
        public Vector2Int exitCell;

        public LockSet lockSet = new LockSet();
        public Color keyReachedColor = Color.yellow;
    }

    [Header("References")]
    public GridMover player;

    [Header("Levels (3)")]
    public List<Level> levels = new List<Level>(3);

    [Header("Renderers")]
    [Tooltip("三把锁在场景里的 SpriteRenderer（锁1/锁2/锁3）")]
    public SpriteRenderer[] lockRenderers = new SpriteRenderer[3];

    [Tooltip("三个图案在场景里的 SpriteRenderer（图案1/图案2/图案3）")]
    public SpriteRenderer[] patternRenderers = new SpriteRenderer[3];

    [Header("After Level 3")]
    public GameObject afterAllDoneSprite;

    int _index;

    void Start()
    {
        if (player != null)
            player.OnExitReached += HandleLevelComplete;

        // 开局：图案1显示，图案2/3隐藏
        if (patternRenderers != null && patternRenderers.Length >= 1 && patternRenderers[0] != null)
            patternRenderers[0].enabled = true;

        if (patternRenderers != null && patternRenderers.Length >= 2 && patternRenderers[1] != null)
            patternRenderers[1].enabled = false;

        if (patternRenderers != null && patternRenderers.Length >= 3 && patternRenderers[2] != null)
            patternRenderers[2].enabled = false;

        StartLevel(0);
    }

    void OnDestroy()
    {
        if (player != null)
            player.OnExitReached -= HandleLevelComplete;
    }

    void StartLevel(int index)
    {
        if (levels == null || levels.Count == 0) return;

        _index = Mathf.Clamp(index, 0, levels.Count - 1);

        if (afterAllDoneSprite != null)
            afterAllDoneSprite.SetActive(false);

        if (player != null)
        {
            var lv = levels[_index];
            player.ApplyLevel(lv.startCell, lv.keyCell, lv.exitCell, lv.keyReachedColor);
            player.enabled = true;
        }

        UpdateLocks();
    }

    void HandleLevelComplete()
    {
        // 当前关完成后：解锁“下一关图案”
        int nextPatternIndex = _index + 1; // 完成0 => 显示1；完成1 => 显示2
        if (patternRenderers != null &&
            nextPatternIndex >= 0 &&
            nextPatternIndex < patternRenderers.Length &&
            patternRenderers[nextPatternIndex] != null)
        {
            patternRenderers[nextPatternIndex].enabled = true;
        }

        int next = _index + 1;

        if (next >= levels.Count)
        {
            SetAllLocksOpen();

            if (afterAllDoneSprite != null)
                afterAllDoneSprite.SetActive(true);

            if (player != null)
                player.enabled = false;

            return;
        }

        StartLevel(next);
    }

    void UpdateLocks()
    {
        for (int i = 0; i < lockRenderers.Length; i++)
        {
            if (lockRenderers[i] == null) continue;
            if (i >= levels.Count) continue;

            var set = levels[i].lockSet;

            if (i < _index) lockRenderers[i].sprite = set.open;
            else if (i == _index) lockRenderers[i].sprite = set.lit;
            else lockRenderers[i].sprite = set.closed;
        }
    }

    void SetAllLocksOpen()
    {
        for (int i = 0; i < lockRenderers.Length; i++)
        {
            if (lockRenderers[i] == null) continue;
            if (i >= levels.Count) continue;

            lockRenderers[i].sprite = levels[i].lockSet.open;
        }
    }
}

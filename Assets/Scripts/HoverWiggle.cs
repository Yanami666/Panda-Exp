using UnityEngine;

public class HoverWiggle : MonoBehaviour
{
    [System.Serializable]
    public class WiggleTarget
    {
        public Transform target;            // 要 wiggle 的对象
        public bool vertical = true;        // true = 上下摇动，false = 左右摇动
        public float amplitude = 0.1f;      // 幅度
        public float speed = 2f;            // 速度

        [HideInInspector] public Vector3 originalPos; // 记录原始位置
    }

    public WiggleTarget[] wiggleTargets;

    private bool isHovering = false;

    private void Start()
    {
        foreach (var w in wiggleTargets)
        {
            if (w.target != null)
                w.originalPos = w.target.localPosition;
        }
    }

    private void OnMouseEnter()
    {
        isHovering = true;
    }

    private void OnMouseExit()
    {
        isHovering = false;
        foreach (var w in wiggleTargets)
        {
            if (w.target != null)
                w.target.localPosition = w.originalPos; // 回原位
        }
    }

    private void Update()
    {
        if (!isHovering) return;

        foreach (var w in wiggleTargets)
        {
            if (w.target == null) continue;

            float offset = Mathf.Sin(Time.time * w.speed) * w.amplitude;

            if (w.vertical)
                w.target.localPosition = w.originalPos + new Vector3(0, offset, 0);
            else
                w.target.localPosition = w.originalPos + new Vector3(offset, 0, 0);
        }
    }
}
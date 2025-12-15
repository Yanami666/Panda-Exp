using UnityEngine;

public class SubmitColliderGuard : MonoBehaviour
{
    [Header("The collider that receives OnMouseDown")]
    public Collider2D targetCollider;   // 拖你 Submit 物体自己的 Collider2D

    [Header("Only clickable when this root is active")]
    public GameObject promptRoot;       // 拖 PasswordPrompt 里那个 root（提示面板根）

    void Reset()
    {
        targetCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (targetCollider == null) return;

        bool canClick = (promptRoot != null && promptRoot.activeInHierarchy);
        if (targetCollider.enabled != canClick)
            targetCollider.enabled = canClick;
    }
}
using UnityEngine;

public class ClickOpen : MonoBehaviour
{
    [Header("Show Pages (can be multiple)")]
    public GameObject[] pagesToShow;

    [Header("Hide Pages")]
    public GameObject[] pagesToHide;

    [Header("Optional")]
    public bool disableColliderAfterClick = false;

    private void OnMouseDown()
    {
        if (pagesToHide != null)
        {
            for (int i = 0; i < pagesToHide.Length; i++)
            {
                if (pagesToHide[i] != null)
                    pagesToHide[i].SetActive(false);
            }
        }

        if (pagesToShow != null)
        {
            for (int i = 0; i < pagesToShow.Length; i++)
            {
                if (pagesToShow[i] != null)
                    pagesToShow[i].SetActive(true);
            }
        }

        if (disableColliderAfterClick)
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }
}

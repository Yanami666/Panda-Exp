using UnityEngine;

public class ClickOpen : MonoBehaviour
{
    [Header("Show/Hide Pages")]
    public GameObject pageToShow;
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

        if (pageToShow != null)
            pageToShow.SetActive(true);

        if (disableColliderAfterClick)
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }
}

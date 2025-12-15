using UnityEngine;

public class BackToNormalUIButton : MonoBehaviour
{
    public void BackToNormal()
    {
        SceneSwapManager.Instance.GoToNormal();
    }
}
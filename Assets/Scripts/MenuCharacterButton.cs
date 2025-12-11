using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCharacterButton : MonoBehaviour
{
    public int sceneIndexToLoad = 1;

    private void OnMouseDown()
    {
        SceneManager.LoadScene(sceneIndexToLoad);
    }
}
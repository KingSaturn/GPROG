using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Scene0"));
    }
    public void QuitButton()
    {

    }
}

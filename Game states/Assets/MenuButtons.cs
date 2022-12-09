using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Scene0", LoadSceneMode.Single);
    }
    public void QuitButton()
    {

    }
}

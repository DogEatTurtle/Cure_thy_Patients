using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1); // Clinic
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}


using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuButtons : MonoBehaviour
{
    public void RestartScene()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu(int mainMenuBuildIndex)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuBuildIndex);
    }
}


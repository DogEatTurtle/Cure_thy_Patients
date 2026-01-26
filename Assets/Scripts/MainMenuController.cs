using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject guidePanel;

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1); // Clinic
    }

    public void OpenGuide()
    {
        if (guidePanel != null)
            guidePanel.SetActive(true);
    }

    public void CloseGuide()
    {
        if (guidePanel != null)
            guidePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}


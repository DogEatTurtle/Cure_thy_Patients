using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Close these on Pause")]
    [SerializeField] private GameObject conversationPanel;
    [SerializeField] private GameObject computerPanel;
    [SerializeField] private GameObject notebookPanel;

    [Header("Disable interaction while paused")]
    [SerializeField] private LookInteractor lookInteractor; // arrasta o LookInteractor da Main Camera

    private bool isPaused;

    private void Awake()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (pausePanel == null) return;

        // Fecha paineis por baixo (evita sobreposicao)
        if (conversationPanel != null) conversationPanel.SetActive(false);
        if (computerPanel != null) computerPanel.SetActive(false);
        if (notebookPanel != null) notebookPanel.SetActive(false);

        // Desativa interacao
        if (lookInteractor != null) lookInteractor.enabled = false;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void Resume()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;

        // Reativar interacao com atraso para nao apanhar o clique do botao Resume
        if (lookInteractor != null)
            StartCoroutine(ReenableInteractorNextFrame());
    }

    private IEnumerator ReenableInteractorNextFrame()
    {
        // espera 1 frame (unscaled) para o UI terminar o clique
        yield return null;

        // espera o botao esquerdo ser largado (evita click-through)
        while (Mouse.current != null && Mouse.current.leftButton.isPressed)
            yield return null;

        lookInteractor.enabled = true;
    }

    private void OnDisable()
    {
        if (isPaused) Time.timeScale = 1f;
    }
}

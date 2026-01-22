using TMPro;
using UnityEngine;

public class DiagnosisUI : MonoBehaviour
{
    [SerializeField] private PatientSessionManager session;
    [SerializeField] private PatientSpawner spawner;
    [SerializeField] private GameObject conversationPanel; // opcional: fecha a conversa ao confirmar
    [SerializeField] private TextMeshProUGUI feedbackText; // opcional

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private GameObject notebookPanel;

    private ConversationManager convoManager;
    private DiseaseSO selectedDisease;
    private bool gameOver = false;

    private void Awake()
    {
        convoManager = FindFirstObjectByType<ConversationManager>();
    }


    public void SelectDisease(DiseaseSO disease)
    {
        if (gameOver) return;
        selectedDisease = disease;
        SetFeedback(disease != null ? $"Selected: {disease.diseaseName}" : "");
    }

    public void ConfirmDiagnosis()
    {
        if (gameOver) return;

        var patient = session.GetCurrentPatient();
        if (patient == null) { SetFeedback("No patient."); return; }
        if (selectedDisease == null) { SetFeedback("Select a disease first."); return; }

        // Fechar bloco de notas ao confirmar
        if (notebookPanel != null)
        {
            notebookPanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        bool correct = selectedDisease == patient.disease;

        if (!correct)
        {
            SetFeedback("Incorrect. Defeat.");
            ShowEndPanel(false);
            return;
        }

        SetFeedback("Correct.");

        bool hasNext = session.NextPatient();
        selectedDisease = null;

        if (convoManager != null)
            convoManager.ClearActivePatient();

        if (!hasNext)
        {
            SetFeedback("All patients treated. Victory!");
            ShowEndPanel(true);
            return;
        }


        spawner.SpawnCurrent();
    }

    private void SetFeedback(string msg)
    {
        if (feedbackText != null) feedbackText.text = msg;
    }

    private void ShowEndPanel(bool victory)
    {
        if (victoryPanel != null) victoryPanel.SetActive(victory);
        if (defeatPanel != null) defeatPanel.SetActive(!victory);

        // Fecha conversa e volta ao modo FPS
        if (conversationPanel != null) conversationPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameOver = true;
    }

}



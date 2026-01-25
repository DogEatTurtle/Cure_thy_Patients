using TMPro;
using UnityEngine;

public class DiagnosisUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PatientSessionManager session;
    [SerializeField] private PatientSpawner spawner;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject notebookPanel;        // bloco de notas (lista doenças)
    [SerializeField] private GameObject conversationPanel;    // painel conversa (input) - opcional fechar ao confirmar

    [Header("End Screen")]
    [SerializeField] private GameObject endPanel;             // painel final (único)
    [SerializeField] private TextMeshProUGUI endResultText;   // TMP_Text onde vai "Pacientes curados - X/5"

    [Header("Optional Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;    // opcional ("Correct/Incorrect")

    [SerializeField] private DiseaseButtonSelectionUI selectionUI;

    private DiseaseSO selectedDisease;
    private int curedCount = 0;
    private int totalPatients = 5;
    private bool gameOver = false;

    private ConversationManager convoManager;

    private void Awake()
    {
        convoManager = FindFirstObjectByType<ConversationManager>();
    }

    private void Start()
    {
        // apanha total real se existir
        if (session != null && session.Patients != null && session.Patients.Count > 0)
            totalPatients = session.Patients.Count;

        if (endPanel != null)
            endPanel.SetActive(false);
    }

    public void SelectDisease(DiseaseSO disease)
    {
        if (gameOver) return;

        selectedDisease = disease;

        if (feedbackText != null && disease != null)
            feedbackText.text = $"Selected: {disease.diseaseName}";
    }

    public void ConfirmDiagnosis()
    {
        if (gameOver) return;

        var patient = session.GetCurrentPatient();
        if (patient == null)
        {
            SetFeedback("No patient.");
            return;
        }

        if (selectedDisease == null)
        {
            SetFeedback("Select a disease first.");
            return;
        }

        // Fecha bloco de notas ao confirmar (como queres)
        if (notebookPanel != null)
            notebookPanel.SetActive(false);

        // Volta a FPS (como queres)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // limpar outlines
        if (selectionUI != null) selectionUI.ClearAll();

        // limpar UI de conversa (texto do paciente anterior)
        if (convoManager != null) convoManager.ClearConversationUI();

        bool correct = selectedDisease == patient.disease;
        if (correct) curedCount++;

        SetFeedback(correct ? "Correct." : "Incorrect.");

        // limpar seleção
        selectedDisease = null;

        // (recomendado) limpar paciente ativo para obrigar a clicar no novo paciente
        if (convoManager != null)
            convoManager.ClearActivePatient(); // remove esta linha se não tiveres este método

        // opcional: fechar conversa quando confirmas
        if (conversationPanel != null)
            conversationPanel.SetActive(false);

        // avança sempre
        bool hasNext = session.NextPatient();

        if (!hasNext)
        {
            ShowEndPanel();
            return;
        }

        // spawn do próximo
        spawner.SpawnCurrent();
    }

    private void ShowEndPanel()
    {
        gameOver = true;

        // garantir UI de gameplay fechada
        if (notebookPanel != null) notebookPanel.SetActive(false);
        if (conversationPanel != null) conversationPanel.SetActive(false);

        // mostrar painel final
        if (endPanel != null) endPanel.SetActive(true);

        if (endResultText != null)
            endResultText.text = $"Diagnosed patients - {curedCount}/{totalPatients}";

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    private void SetFeedback(string msg)
    {
        if (feedbackText != null)
            feedbackText.text = msg;
    }
}




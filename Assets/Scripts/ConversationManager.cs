using TMPro;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private PatientSessionManager session;
    [SerializeField] private OllamaClient ollama;

    [Header("UI")]
    [SerializeField] private TMP_InputField inputQuestion;
    [SerializeField] private TextMeshProUGUI outputText;

    private bool busy = false;

    private PatientInstance activePatient;

    public async void Ask()
    {
        if (busy) return;

        var patient = activePatient ?? session.GetCurrentPatient();
        if (patient == null) return;

        string question = inputQuestion.text.Trim();
        if (string.IsNullOrEmpty(question)) return;

        patient.UnlockFactsForQuestion(question);

        busy = true;
        outputText.text = "…";

        try
        {
            string systemPrompt = PromptBuilder.BuildSystemPrompt();
            string userPrompt = PromptBuilder.BuildUserPrompt(patient, question);

            string reply = await ollama.ChatOnceAsync(systemPrompt, userPrompt);
            outputText.text = reply;
            inputQuestion.text = "";
        }
        finally
        {
            busy = false;
        }
    }

    public void Submit()
    {
        if (busy) return;
        if (inputQuestion == null) return;

        var text = inputQuestion.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        Ask();
    }

    // Para ligar ao evento OnSubmit (se tiveres) ou OnEndEdit (string)
    public void SubmitFromInput(string _)
    {
        Submit();
    }


    public void SetActivePatient(PatientInstance p)
    {
        activePatient = p;
    }

    public void NextPatient()
    {
        bool hasNext = session.NextPatient();
        inputQuestion.text = "";
        outputText.text = hasNext ? "Next patient is ready." : "Session finished.";
    }
    public void ClearActivePatient()
    {
        activePatient = null;
    }

    public void ClearConversationUI()
    {
        if (outputText != null) outputText.text = "";
        if (inputQuestion != null) inputQuestion.text = "";
        busy = false; // segurança
        ClearActivePatient(); // se tens este método
    }


}


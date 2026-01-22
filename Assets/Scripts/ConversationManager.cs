using System.Threading.Tasks;
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

    public async void Ask()
    {
        if (busy) return;

        var patient = session.GetCurrentPatient();
        if (patient == null) return;

        string question = inputQuestion.text.Trim();
        if (string.IsNullOrEmpty(question)) return;

        busy = true;
        outputText.text = "…";

        string systemPrompt = PromptBuilder.BuildSystemPrompt();
        string userPrompt = PromptBuilder.BuildUserPrompt(patient, question);

        string reply = await ollama.ChatOnceAsync(systemPrompt, userPrompt);

        outputText.text = reply;

        inputQuestion.text = "";
        busy = false;
    }

    public void NextPatient()
    {
        bool hasNext = session.NextPatient();
        outputText.text = hasNext ? "Next patient is ready." : "Session finished.";
    }
}


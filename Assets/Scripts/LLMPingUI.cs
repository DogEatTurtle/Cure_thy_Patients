using TMPro;
using UnityEngine;

public class LLMPingUI : MonoBehaviour
{
    [SerializeField] private OllamaClient ollama;
    [SerializeField] private TextMeshProUGUI output;

    public async void Ping()
    {
        output.text = "…";

        string systemPrompt =
@"You are roleplaying as a patient.
Speak like a normal person. No medical advice.
Answer in 1-2 sentences.";

        string userPrompt = "Doctor: What brings you in today?";

        string reply = await ollama.ChatOnceAsync(systemPrompt, userPrompt);
        output.text = reply;
    }
}


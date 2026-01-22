using System;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

using OllamaSharp;
using OllamaSharp.Models;

public class Ollama_Handler : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField InputText;
    public TMP_Text OutputText;

    [Header("Ollama")]
    public string OllamaUrl = "http://localhost:11434";
    public string Model = "qwen2.5:7b"; 

    public void Run()
    {
        _ = ProcessTheConversation(); // fire-and-forget (não bloqueia o Unity)
    }

    private async Task ProcessTheConversation()
    {
        if (InputText == null || OutputText == null)
        {
            Debug.LogError("Liga o InputText e o OutputText no Inspector.");
            return;
        }

        var prompt = InputText.text?.Trim();
        if (string.IsNullOrEmpty(prompt))
        {
            OutputText.text = "Escreve uma pergunta primeiro.";
            return;
        }

        try
        {
            Debug.Log("Running...");

            OutputText.text = ""; // limpa output

            var ollama = new OllamaApiClient(new Uri(OllamaUrl))
            {
                SelectedModel = Model
            };

            // Request explícito (mais compatível)
            var req = new GenerateRequest
            {
                Model = Model,
                Prompt = prompt,
                Stream = true
            };

            // Streaming: vai recebendo tokens e actualizando o UI
            await foreach (var token in ollama.GenerateAsync(req))
            {
                if (!string.IsNullOrEmpty(token?.Response))
                    OutputText.text += token.Response;
            }

            Debug.Log("Done!");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            OutputText.text = "Erro: " + ex.Message;
        }
    }
}




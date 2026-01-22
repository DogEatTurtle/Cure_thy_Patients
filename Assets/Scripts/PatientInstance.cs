using System.Collections.Generic;

public class PatientInstance
{
    public DiseaseSO disease;
    public PersonalitySO personality;

    // O jogo controla o que é permitido revelar ao LLM
    public List<string> factsAllowed = new();
    public List<string> factsHiddenUntilAsked = new();

    // Histórico curto (opcional)
    public List<(string role, string text)> chatHistory = new();

    public PatientInstance(DiseaseSO d, PersonalitySO p)
    {
        disease = d;
        personality = p;

        // Por defeito: permite todos os factos da doença
        if (d != null && d.patientFriendlyFacts != null)
            factsAllowed.AddRange(d.patientFriendlyFacts);
    }
}



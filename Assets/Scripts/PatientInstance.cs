using System;
using System.Collections.Generic;

public class PatientInstance
{
    public DiseaseSO disease;
    public PersonalitySO personality;

    public List<string> factsAllowed = new();
    public List<string> factsHiddenUntilAsked = new();

    public List<(string role, string text)> chatHistory = new();

    public PatientInstance(DiseaseSO d, PersonalitySO p)
    {
        disease = d;
        personality = p;

        if (d == null || d.patientFriendlyFacts == null) return;

        // Começa genérico (bom para "Hello")
        factsAllowed.Add("I haven't been feeling quite right.");

        // Tudo o resto fica escondido
        factsHiddenUntilAsked.AddRange(d.patientFriendlyFacts);
    }

    public void UnlockFactsForQuestion(string question)
    {
        if (factsHiddenUntilAsked.Count == 0) return;
        if (string.IsNullOrWhiteSpace(question)) return;

        string q = question.ToLowerInvariant();

        // Perguntas abertas: liberta 1 facto "seguinte"
        if (IsOpenQuestion(q))
        {
            MoveNextHiddenToAllowed();
            return;
        }

        // Keywords -> tenta libertar 1 facto que pareça relacionado
        TryMoveByKeyword(q, new[] { "fever", "temperature", "febre" }, new[] { "fever", "febre" });
        TryMoveByKeyword(q, new[] { "cough", "tosse" }, new[] { "cough", "tosse" });
        TryMoveByKeyword(q, new[] { "throat", "garganta" }, new[] { "throat", "garganta" });
        TryMoveByKeyword(q, new[] { "urine", "pee", "burn", "urinar", "ardor" }, new[] { "urine", "urinar", "ardor" });
        TryMoveByKeyword(q, new[] { "vomit", "nausea", "diarr", "vómit", "náuse", "diarre" }, new[] { "vomit", "nause", "diarr", "vómit", "náuse", "diarre" });
        TryMoveByKeyword(q, new[] { "tired", "fatigue", "cansa", "fadig" }, new[] { "tired", "fatigue", "cansa", "fadig" });
        TryMoveByKeyword(q, new[] { "ache", "pain", "dor", "aches" }, new[] { "ache", "pain", "dor" });

        // Se não encontrou nada por keyword, liberta 1 facto “seguinte” (para não ficar bloqueado)
        if (factsAllowed.Count <= 1)
            MoveNextHiddenToAllowed();
    }

    private bool IsOpenQuestion(string q)
    {
        return q.Contains("what brings") ||
               q.Contains("what's wrong") ||
               q.Contains("whats wrong") ||
               q.Contains("tell me") ||
               q.Contains("describe") ||
               q.Contains("symptoms") ||
               q.Contains("que se passa") ||
               q.Contains("o que se passa") ||
               q.Contains("o que tem") ||
               q.Contains("sintomas");
    }

    private void MoveNextHiddenToAllowed()
    {
        if (factsHiddenUntilAsked.Count == 0) return;
        var fact = factsHiddenUntilAsked[0];
        factsHiddenUntilAsked.RemoveAt(0);
        factsAllowed.Add(fact);
    }

    private void TryMoveByKeyword(string q, string[] questionKeywords, string[] factKeywords)
    {
        bool questionMatch = false;
        foreach (var k in questionKeywords)
            if (q.Contains(k)) { questionMatch = true; break; }
        if (!questionMatch) return;

        for (int i = 0; i < factsHiddenUntilAsked.Count; i++)
        {
            string fact = factsHiddenUntilAsked[i];
            string f = fact.ToLowerInvariant();

            foreach (var fk in factKeywords)
            {
                if (f.Contains(fk))
                {
                    factsHiddenUntilAsked.RemoveAt(i);
                    factsAllowed.Add(fact);
                    return; // libertar só 1 facto por pergunta
                }
            }
        }
    }
}




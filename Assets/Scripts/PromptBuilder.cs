using System.Text;

public static class PromptBuilder
{
    public static string BuildSystemPrompt()
    {
        return
@"You are roleplaying as a patient in a medical interview inside a game prototype.
Speak like a normal person (non-medical language).
Do NOT invent new symptoms or facts.
Do NOT give medical advice.
Do NOT mention treatments.
No more than 40 words per answer.
Only use the facts provided.
If asked about something not in your facts, say you are not sure or you haven't noticed it.
Keep answers reasonably short unless the personality is very talkative.";
    }

    public static string BuildUserPrompt(PatientInstance p, string playerQuestion)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Personality profile: {p.personality.profileName}");
        sb.AppendLine($"Speaking style notes: {p.personality.speakingStyleNotes}");
        sb.AppendLine($"Controls: talkativeness={p.personality.talkativeness:0.00}, directness={p.personality.directness:0.00}, cooperativeness={p.personality.cooperativeness:0.00}, dramatization={p.personality.dramatization:0.00}");
        sb.AppendLine();

        sb.AppendLine("Facts you may mention (symptoms/experiences):");
        foreach (var f in p.factsAllowed)
            sb.AppendLine($"- {f}");

        if (p.factsHiddenUntilAsked.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("Facts you must NOT mention unless asked directly:");
            foreach (var f in p.factsHiddenUntilAsked)
                sb.AppendLine($"- {f}");
        }

        sb.AppendLine();
        sb.AppendLine("Doctor's question:");
        sb.AppendLine(playerQuestion);

        return sb.ToString();
    }
}


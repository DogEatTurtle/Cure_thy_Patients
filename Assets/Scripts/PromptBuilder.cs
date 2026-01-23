using System.Text;

public static class PromptBuilder
{
    public static string BuildSystemPrompt()
    {
        return
@"You are roleplaying as a patient in a medical interview inside a game prototype.
Speak like a normal person (non-medical language).

Hard rules:
- Do NOT invent any symptoms, timelines, causes, or background (work/family/events).
- Do NOT give medical advice.
- Do NOT mention treatments.
- Do NOT name any disease.
- Never guess or suggest diagnoses. If the doctor asks ""what do you think it is?"", say you don't know.

You will receive INPUT_TYPE.
- If INPUT_TYPE=GREETING: reply with a short greeting and ask what the doctor would like to know.
  Do NOT mention symptoms, timelines, or background.
- If INPUT_TYPE=MEDICAL: answer only the doctor's specific question. Reveal at most ONE allowed fact per reply.
  Do NOT list symptoms.

Only use the facts provided by the game.
If asked about something not in your facts, say you are not sure or you haven't noticed it.
Keep it short (max ~40 words unless very talkative).";
    }

    public static string BuildUserPrompt(PatientInstance p, string playerQuestion)
    {
        var sb = new StringBuilder();

        string inputType = IsGreetingOrSmallTalk(playerQuestion) ? "GREETING" : "MEDICAL";
        sb.AppendLine($"INPUT_TYPE: {inputType}");
        sb.AppendLine();

        sb.AppendLine($"Personality profile: {p.personality.profileName}");
        sb.AppendLine($"Speaking style notes: {p.personality.speakingStyleNotes}");
        sb.AppendLine($"Controls: talkativeness={p.personality.talkativeness:0.00}, directness={p.personality.directness:0.00}, cooperativeness={p.personality.cooperativeness:0.00}, dramatization={p.personality.dramatization:0.00}");
        sb.AppendLine();

        sb.AppendLine("Facts you may mention:");
        foreach (var f in p.factsAllowed)
            sb.AppendLine($"- {f}");

        sb.AppendLine();
        sb.AppendLine("Doctor's message:");
        sb.AppendLine(playerQuestion);

        return sb.ToString();
    }

    private static bool IsGreetingOrSmallTalk(string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return true;

        string t = q.Trim().ToLowerInvariant();
        t = t.Replace(".", "").Replace("!", "").Replace(",", "").Replace(";", "").Replace(":", "");

        // Cumprimentos (PT + EN)
        bool startsWithGreeting =
            t.StartsWith("hi") || t.StartsWith("hello") || t.StartsWith("hey") ||
            t.StartsWith("olá") || t.StartsWith("ola") ||
            t.StartsWith("bom dia") || t.StartsWith("boa tarde") || t.StartsWith("boa noite") ||
            t.StartsWith("good morning") || t.StartsWith("good afternoon") || t.StartsWith("good evening");

        // Small talk comum
        bool smallTalk =
            t == "how are you" || t == "how are you doing" ||
            t == "como estás" || t == "como esta" || t == "como está";

        // Só é GREETING se for mesmo cumprimento/small talk.
        // Isto evita classificar perguntas curtas médicas como "GREETING" (ex: "Fever?")
        if (smallTalk) return true;

        if (startsWithGreeting)
        {
            // Mensagens curtas de cumprimento tipo "hello there", "olá doutor"
            if (t.Length <= 25) return true;
        }

        return false;
    }
}




using TMPro;
using UnityEngine;

public class DiagnosisUI : MonoBehaviour
{
    [SerializeField] private PatientSessionManager session;
    [SerializeField] private TextMeshProUGUI feedbackText; // opcional

    private DiseaseSO selectedDisease;

    public void SelectDisease(DiseaseSO disease)
    {
        selectedDisease = disease;
        if (feedbackText != null)
            feedbackText.text = disease != null ? $"Selected: {disease.diseaseName}" : "";
    }

    public void ConfirmDiagnosis()
    {
        var patient = session.GetCurrentPatient();
        if (patient == null) { SetFeedback("No patient."); return; }

        if (selectedDisease == null) { SetFeedback("Select a disease first."); return; }

        bool correct = selectedDisease == patient.disease;
        SetFeedback(correct ? "Correct." : "Incorrect.");

        bool hasNext = session.NextPatient();
        selectedDisease = null;

        if (!hasNext)
            SetFeedback("Session finished.");
    }

    private void SetFeedback(string msg)
    {
        if (feedbackText != null) feedbackText.text = msg;
    }
}


using System.Collections.Generic;
using UnityEngine;

public class PatientSessionManager : MonoBehaviour
{
    [Header("Pools")]
    [SerializeField] private List<DiseaseSO> allDiseases;           // 10
    [SerializeField] private List<PersonalitySO> allPersonalities;  // 5

    [Header("Session")]
    [SerializeField] private int patientsPerSession = 5;

    [Header("Debug")]
    [SerializeField] private bool debugLogging = true;

    public List<PatientInstance> Patients { get; private set; } = new();
    public int CurrentIndex { get; private set; } = 0;

    private void Start()
    {
        BuildSession();
    }

    public void BuildSession()
    {
        Patients.Clear();
        CurrentIndex = 0;

        // Segurança básica
        if (allDiseases == null || allDiseases.Count < patientsPerSession)
        {
            Debug.LogError($"[PatientSessionManager] Not enough diseases. Need at least {patientsPerSession}.");
            return;
        }

        if (allPersonalities == null || allPersonalities.Count < patientsPerSession)
        {
            Debug.LogError($"[PatientSessionManager] Not enough personalities. Need at least {patientsPerSession}.");
            return;
        }

        var diseases = new List<DiseaseSO>(allDiseases);
        var personalities = new List<PersonalitySO>(allPersonalities);

        Shuffle(diseases);
        Shuffle(personalities);

        // Personalidades: sem repetição (tens 5)
        // Doenças: sem repetição (tiras 5 de 10)
        for (int i = 0; i < patientsPerSession; i++)
        {
            var d = diseases[i];
            var p = personalities[i];
            Patients.Add(new PatientInstance(d, p));
        }

        if (debugLogging)
        {
            Debug.Log($"[PatientSessionManager] Built session with {Patients.Count} patients.");

            for (int i = 0; i < Patients.Count; i++)
            {
                var pt = Patients[i];
                Debug.Log($"[{i}] Personality: {pt.personality.profileName} | Disease: {pt.disease.diseaseName}");
            }
        }
    }

    public PatientInstance GetCurrentPatient()
    {
        if (Patients.Count == 0) return null;
        return Patients[Mathf.Clamp(CurrentIndex, 0, Patients.Count - 1)];
    }

    public bool NextPatient()
    {
        CurrentIndex++;
        return CurrentIndex < Patients.Count;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}



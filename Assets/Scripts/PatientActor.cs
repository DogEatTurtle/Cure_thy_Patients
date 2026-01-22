using UnityEngine;

public class PatientActor : MonoBehaviour
{
    public PatientInstance Patient { get; private set; }

    public void Assign(PatientInstance patient)
    {
        Patient = patient;
    }
}


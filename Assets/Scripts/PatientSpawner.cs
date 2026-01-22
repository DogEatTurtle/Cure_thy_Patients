using UnityEngine;

public class PatientSpawner : MonoBehaviour
{
    [SerializeField] private PatientSessionManager session;
    [SerializeField] private Transform spawnPoint;

    [Header("5 test prefabs (capsules)")]
    [SerializeField] private GameObject[] patientPrefabs; // tamanho 5

    private GameObject currentInstance;

    private void Start()
    {
        SpawnCurrent();
    }

    public void SpawnCurrent()
    {
        // limpar o anterior
        if (currentInstance != null)
            Destroy(currentInstance);

        int idx = session.CurrentIndex; // 0..4

        if (idx < 0 || idx >= patientPrefabs.Length)
        {
            Debug.LogError("Patient prefabs array must have 5 elements.");
            return;
        }

        currentInstance = Instantiate(patientPrefabs[idx], spawnPoint.position, spawnPoint.rotation);

        var actor = currentInstance.GetComponent<PatientActor>();
        if (actor != null)
        {
            actor.Assign(session.GetCurrentPatient());
        }
        else
        {
            Debug.LogWarning("Spawned patient prefab has no PatientActor component.");
        }
    }

    public void SpawnNext()
    {
        bool hasNext = session.NextPatient();
        if (!hasNext)
        {
            Debug.Log("No more patients.");
            return;
        }

        SpawnCurrent();
    }
}


using UnityEngine;

public class DescriptionSwitcher : MonoBehaviour
{
    [Header("Assign the 10 TMP text GameObjects here (one per disease)")]
    [SerializeField] private GameObject[] descriptionTexts;

    private void Awake()
    {
        HideAll();
    }

    public void Show(GameObject textToShow)
    {
        HideAll();
        if (textToShow != null)
            textToShow.SetActive(true);
    }

    public void Close(GameObject uiToDisable)
    {
        HideAll();

        if (uiToDisable != null)
            uiToDisable.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void HideAll()
    {
        if (descriptionTexts == null) return;

        foreach (var go in descriptionTexts)
        {
            if (go != null) go.SetActive(false);
        }
    }
}


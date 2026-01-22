using UnityEngine;

public class UIRegistry : MonoBehaviour
{
    public static UIRegistry Instance { get; private set; }

    [SerializeField] private GameObject conversationPanel;
    public GameObject ConversationPanel => conversationPanel;

    private void Awake()
    {
        Instance = this;
        Debug.Log($"[UIRegistry] Awake. conversationPanel = {(conversationPanel == null ? "NULL" : conversationPanel.name)}");
    }
}


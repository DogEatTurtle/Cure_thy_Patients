using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactLayers = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("Cursor / UI")]
    [SerializeField] private bool unlockCursorOnOpen = true;

    [Header("Hover Prompt UI (TMP)")]
    [SerializeField] private TMP_Text hoverPromptText; // arrasta aqui o TMP_Text (ou mete um TMP_Text num GO que queres ligar/desligar)
    [SerializeField] private bool hidePromptOnClick = true;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private Color debugRayColor = Color.cyan;

    private Highlightable _currentHighlighted;

    private void Start()
    {
        SetPromptVisible(false);
    }

    void Update()
    {
        var origin = rayOrigin != null ? rayOrigin.position : transform.position;
        var direction = (rayOrigin != null) ? rayOrigin.forward : transform.forward;

        if (drawDebugRay)
            Debug.DrawRay(origin, direction * maxDistance, debugRayColor);

        Highlightable found = null;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, interactLayers, triggerInteraction))
        {
            found = hit.collider.GetComponentInParent<Highlightable>();
            if (found == null) found = hit.collider.GetComponent<Highlightable>();
            if (found == null) found = hit.collider.GetComponentInChildren<Highlightable>();
            if (found == null) found = hit.transform.GetComponentInParent<Highlightable>();
        }

        if (found != _currentHighlighted)
        {
            if (_currentHighlighted != null) _currentHighlighted.SetHighlighted(false);

            _currentHighlighted = found;

            if (_currentHighlighted != null)
            {
                _currentHighlighted.SetHighlighted(true);
                SetPromptVisible(true);
            }
            else
            {
                SetPromptVisible(false);
            }
        }

        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_currentHighlighted != null)
            {
                if (hidePromptOnClick) SetPromptVisible(false);

                _currentHighlighted.OpenUI();

                if (unlockCursorOnOpen)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            else
            {
                Debug.Log("Left click but nothing highlighted (ensure object has a Collider and Highlightable component).");
            }
        }
    }

    private void SetPromptVisible(bool on)
    {
        if (hoverPromptText == null) return;
        hoverPromptText.gameObject.SetActive(on);
    }

    void OnDisable()
    {
        if (_currentHighlighted != null)
        {
            _currentHighlighted.SetHighlighted(false);
            _currentHighlighted = null;
        }
        SetPromptVisible(false);
    }
}

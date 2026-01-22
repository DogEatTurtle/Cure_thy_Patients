using UnityEngine;
using UnityEngine.InputSystem;

public class LookInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Transform rayOrigin;                   // if null, uses this.transform
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask interactLayers = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("Cursor / UI")]
    [SerializeField] private bool unlockCursorOnOpen = true;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRay = true;
    [SerializeField] private Color debugRayColor = Color.cyan;

    private Highlightable _currentHighlighted;

    void Update()
    {
        var origin = rayOrigin != null ? rayOrigin.position : transform.position;
        var direction = (rayOrigin != null) ? rayOrigin.forward : transform.forward;

        if (drawDebugRay)
            Debug.DrawRay(origin, direction * maxDistance, debugRayColor);

        // Raycast forward from origin
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, interactLayers, triggerInteraction))
        {
            // Try several ways to find the Highlightable component to be robust to your hierarchy
            Highlightable found = hit.collider.GetComponentInParent<Highlightable>();
            if (found == null)
                found = hit.collider.GetComponent<Highlightable>();
            if (found == null)
                found = hit.collider.GetComponentInChildren<Highlightable>();
            if (found == null)
                found = hit.transform.GetComponentInParent<Highlightable>(); // fallback

            if (found != _currentHighlighted)
            {
                if (_currentHighlighted != null) _currentHighlighted.SetHighlighted(false);
                _currentHighlighted = found;
                if (_currentHighlighted != null) _currentHighlighted.SetHighlighted(true);
            }
        }
        else
        {
            if (_currentHighlighted != null)
            {
                _currentHighlighted.SetHighlighted(false);
                _currentHighlighted = null;
            }
        }

        // Left click (Input System)
        if (Mouse.current == null)
        {
            // No mouse device present on this platform
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_currentHighlighted != null)
            {
                _currentHighlighted.OpenUI();

                if (unlockCursorOnOpen)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            else
            {
                // Optional: helpful log while debugging
                Debug.Log("Left click but nothing highlighted (ensure object has a Collider and Highlightable component).");
            }
        }
    }

    void OnDisable()
    {
        if (_currentHighlighted != null)
        {
            _currentHighlighted.SetHighlighted(false);
            _currentHighlighted = null;
        }
    }
}
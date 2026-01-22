using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] private float sensitivity = 2f;      // Intuitive slider for sensitivity
    [SerializeField] private bool invertY = false;        // Invert vertical look
    [SerializeField] private float clampAngle = 85f;      // Max up/down angle in degrees
    [SerializeField] private bool lockCursor = true;      // Lock & hide cursor at start

    private float _pitch = 0f; // rotation around X (up/down)
    private float _yaw = 0f;   // rotation around Y (left/right)

    void Start()
    {
        // Initialize current rotation from transform so look doesn't jump
        Vector3 angles = transform.localEulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // Toggle cursor lock with Escape using the new Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return; // don't rotate when cursor is unlocked

        // Ensure a mouse device exists (platforms without a mouse will be null)
        if (Mouse.current == null)
            return;

        // Read mouse delta from the new Input System
        Vector2 delta = Mouse.current.delta.ReadValue();

        // Scale the delta for a similar feel to the old Input.GetAxis approach
        float mouseX = delta.x * sensitivity * 0.1f;
        float mouseY = delta.y * sensitivity * 0.1f;

        // Apply inversion and accumulate
        _yaw += mouseX;
        _pitch += invertY ? mouseY : -mouseY;

        // Clamp pitch to avoid flipping
        _pitch = Mathf.Clamp(_pitch, -clampAngle, clampAngle);

        // Apply rotation
        transform.localEulerAngles = new Vector3(_pitch, _yaw, 0f);
    }

    void OnDisable()
    {
        // Restore cursor when script is disabled
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

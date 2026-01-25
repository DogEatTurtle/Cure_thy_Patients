using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Mouse Look")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float clampAngle = 85f;
    [SerializeField] private bool lockCursor = true;

    private float _pitch = 0f;
    private float _yaw = 0f;

    void Start()
    {
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
        // Não roda a câmara durante pausa
        if (Time.timeScale == 0f)
            return;

        // Não roda quando o cursor está desbloqueado (UI aberta, pausa, etc.)
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (Mouse.current == null)
            return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        float mouseX = delta.x * sensitivity * 0.1f;
        float mouseY = delta.y * sensitivity * 0.1f;

        _yaw += mouseX;
        _pitch += invertY ? mouseY : -mouseY;

        _pitch = Mathf.Clamp(_pitch, -clampAngle, clampAngle);

        transform.localEulerAngles = new Vector3(_pitch, _yaw, 0f);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Orbit camera driven by mouse-drag + scroll (desktop) OR gamepad right
/// stick + triggers (controller) - both work interchangeably, no setup
/// needed to switch between them at runtime.
///
/// Requires the Input System package (Window > Package Manager > Input
/// System), and Project Settings > Player > Active Input Handling set to
/// "Input System Package (New)" or "Both".
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float mouseRotateSpeed = 15f;
    public float gamepadRotateSpeed = 120f;
    public float zoomSpeed = 10f;
    public float minDistance = 2f;
    public float maxDistance = 40f;

    private float _yaw = 0f;
    private float _pitch = 15f;
    private float _distance = 12f;

    private void LateUpdate()
    {
        if (target == null) return;

        HandleMouse();
        HandleGamepad();
        _distance = Mathf.Clamp(_distance, minDistance, maxDistance);
        _pitch = Mathf.Clamp(_pitch, -80f, 80f);

        var rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        var position = target.position + rotation * new Vector3(0f, 0f, -_distance);
        transform.SetPositionAndRotation(position, rotation);
    }

    private void HandleMouse()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.isPressed)
        {
            Vector2 delta = mouse.delta.ReadValue();
            _yaw += delta.x * mouseRotateSpeed * Time.deltaTime;
            _pitch -= delta.y * mouseRotateSpeed * Time.deltaTime;
        }

        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
            _distance -= scroll * zoomSpeed * Time.deltaTime;
    }

    private void HandleGamepad()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        Vector2 stick = gamepad.rightStick.ReadValue();
        _yaw += stick.x * gamepadRotateSpeed * Time.deltaTime;
        _pitch -= stick.y * gamepadRotateSpeed * Time.deltaTime;

        float zoomIn = gamepad.rightTrigger.ReadValue();
        float zoomOut = gamepad.leftTrigger.ReadValue();
        _distance -= (zoomIn - zoomOut) * zoomSpeed * Time.deltaTime;
    }
}

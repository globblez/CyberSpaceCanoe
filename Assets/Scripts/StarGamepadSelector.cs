using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lets a gamepad browse the constellation without a cursor: left
/// stick / D-pad cycles the highlighted star, South button (A / Cross)
/// opens its popup, East button (B / Circle) closes it, North button
/// (Y / Triangle) toggles free roam. Works alongside mouse clicks -
/// StarClickHandler and this can both drive the same InfoPopupUI.
/// </summary>
public class StarGamepadSelector : MonoBehaviour
{
    public ConstellationController constellation;
    public InfoPopupUI popup;
    [Tooltip("Seconds between repeat moves while the stick is held past the deadzone.")]
    public float moveRepeatDelay = 0.25f;
    [Range(0.1f, 0.9f)] public float stickDeadzone = 0.6f;

    private int _currentIndex = -1;
    private float _moveTimer;
    private StarHighlight _currentHighlight;

    private void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null || constellation == null || constellation.StarCount == 0) return;

        HandleNavigation(gamepad);
        HandleConfirmCancel(gamepad);
    }

    private void HandleNavigation(Gamepad gamepad)
    {
        if (_currentIndex < 0)
        {
            SetSelected(0); // auto-highlight the first star once a controller is detected
            return;
        }

        int direction = 0;
        if (gamepad.dpad.right.wasPressedThisFrame) direction = 1;
        else if (gamepad.dpad.left.wasPressedThisFrame) direction = -1;

        _moveTimer -= Time.deltaTime;
        Vector2 stick = gamepad.leftStick.ReadValue();
        if (direction == 0 && _moveTimer <= 0f && Mathf.Abs(stick.x) > stickDeadzone)
        {
            direction = stick.x > 0 ? 1 : -1;
            _moveTimer = moveRepeatDelay;
        }

        if (direction != 0)
        {
            int next = (_currentIndex + direction + constellation.StarCount) % constellation.StarCount;
            SetSelected(next);
        }
    }

    private void HandleConfirmCancel(Gamepad gamepad)
    {
        if (_currentIndex < 0) return;

        if (gamepad.buttonSouth.wasPressedThisFrame)
            popup?.Show(constellation.GetStarInfo(_currentIndex));
        else if (gamepad.buttonEast.wasPressedThisFrame)
            popup?.Hide();
        else if (gamepad.buttonNorth.wasPressedThisFrame)
            constellation.ToggleFreeRoam();
    }

    private void SetSelected(int index)
    {
        if (_currentHighlight != null) _currentHighlight.SetSelected(false);

        _currentIndex = index;
        var t = constellation.GetStarTransform(_currentIndex);
        _currentHighlight = t != null ? t.GetComponent<StarHighlight>() : null;
        if (_currentHighlight != null) _currentHighlight.SetSelected(true);
    }
}

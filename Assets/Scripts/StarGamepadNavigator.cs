using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller navigation for hand-placed constellations. Cycles through
/// whatever StarPresenters are on the currently active ConstellationRoot
/// (via ConstellationSwitcher) - works no matter how many stars each
/// constellation has, with zero per-constellation setup.
///
/// D-pad/left stick: cycle stars | A: open popup | B: close popup
/// Y: toggle free roam | Left/Right bumper: back to selection / re-enter
/// </summary>
public class StarGamepadNavigator : MonoBehaviour
{
    public ConstellationSwitcher switcher;
    public InfoPopupUI popup;
    public float moveRepeatDelay = 0.25f;
    [Range(0.1f, 0.9f)] public float stickDeadzone = 0.6f;

    private int _currentIndex = -1;
    private float _moveTimer;
    private StarPresenter _currentHighlight;
    private ConstellationRoot _trackedRoot;

    private void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null || switcher == null) return;

        var active = switcher.ActiveConstellation;

        if (active == null)
        {
            ResetSelection();
            return;
        }

        if (active != _trackedRoot)
        {
            _trackedRoot = active;
            ResetSelection();
        }

        if (gamepad.leftShoulder.wasPressedThisFrame)
        {
            switcher.BackToSelection();
            return;
        }

        HandleNavigation(gamepad, active);
        HandleConfirmCancel(gamepad, active);
    }

    private void HandleNavigation(Gamepad gamepad, ConstellationRoot active)
    {
        if (active.Stars == null || active.Stars.Length == 0) return;

        if (_currentIndex < 0)
        {
            SetSelected(active, 0);
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
            int next = (_currentIndex + direction + active.Stars.Length) % active.Stars.Length;
            SetSelected(active, next);
        }
    }

    private void HandleConfirmCancel(Gamepad gamepad, ConstellationRoot active)
    {
        if (_currentIndex < 0 || active.Stars.Length == 0) return;

        if (gamepad.buttonSouth.wasPressedThisFrame)
            popup?.Show(active.Stars[_currentIndex]);
        else if (gamepad.buttonEast.wasPressedThisFrame)
            popup?.Hide();
        else if (gamepad.buttonNorth.wasPressedThisFrame)
            active.ToggleFreeRoam();
    }

    private void SetSelected(ConstellationRoot active, int index)
    {
        if (_currentHighlight != null) _currentHighlight.SetSelected(false);
        _currentIndex = index;
        _currentHighlight = active.Stars[index];
        _currentHighlight.SetSelected(true);
    }

    private void ResetSelection()
    {
        if (_currentHighlight != null) _currentHighlight.SetSelected(false);
        _currentHighlight = null;
        _currentIndex = -1;
    }
}

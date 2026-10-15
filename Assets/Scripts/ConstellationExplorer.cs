using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// The click flow for the whole project (replaces ConstellationSwitcher):
///
///   1. Overview  - every constellation is visible, nothing on screen.
///                  Click a constellation: its name appears next to it.
///   2. Named     - click the same constellation again: the others hide and
///                  you see all of its stars.
///   3. Expanded  - click a star: a small info panel appears beside it.
///
/// Click empty space (or press Esc) to go back one step.
/// Press F while expanded to toggle free roam (stars spread to true distance).
///
/// Clicks are read with a raycast through the Input System, so this does not
/// depend on the old OnMouseDown message. Stars need a Collider.
/// </summary>
public class ConstellationExplorer : MonoBehaviour
{
    [Tooltip("Every constellation parent (each one has ConstellationRoot).")]
    public ConstellationRoot[] constellations;

    public InfoPopupUI popup;

    [Tooltip("Leave empty to use Camera.main.")]
    public Camera viewCamera;

    [Tooltip("Optional: aims an OrbitCamera at the constellation you open.")]
    public OrbitCamera orbitCamera;

    public float maxPickDistance = 5000f;

    [Header("Controller")]
    [Tooltip("Optional small UI Image used as an on-screen cursor when a gamepad is connected.")]
    public RectTransform gamepadCursor;
    [Tooltip("Cursor speed in pixels per second at full stick.")]
    public float cursorSpeed = 700f;

    private enum State { Overview, Named, Expanded }

    private State _state = State.Overview;
    private ConstellationRoot _selected;
    private bool _starPanelOpen;

    private void Start()
    {
        if (viewCamera == null) viewCamera = Camera.main;
        if (popup != null) popup.Hide();
        SetAllConstellationsVisible(true);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.escapeKey.wasPressedThisFrame) GoBack();

            if (keyboard.fKey.wasPressedThisFrame && _state == State.Expanded && _selected != null)
                _selected.ToggleFreeRoam();
        }

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            // Ignore clicks that land on UI (like the info panel itself).
            if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
                PickAt(mouse.position.ReadValue());
        }

        UpdateGamepad();
    }

    private Vector2 _cursorPos;
    private bool _cursorReady;

    // Controller: left stick moves an on-screen cursor, A clicks, B goes back, Y toggles free roam.
    private void UpdateGamepad()
    {
        var pad = Gamepad.current;
        if (pad == null)
        {
            if (gamepadCursor != null) gamepadCursor.gameObject.SetActive(false);
            return;
        }

        if (!_cursorReady)
        {
            _cursorPos = new Vector2(Screen.width, Screen.height) * 0.5f;
            _cursorReady = true;
        }

        _cursorPos += pad.leftStick.ReadValue() * cursorSpeed * Time.deltaTime;
        _cursorPos.x = Mathf.Clamp(_cursorPos.x, 0f, Screen.width);
        _cursorPos.y = Mathf.Clamp(_cursorPos.y, 0f, Screen.height);

        if (gamepadCursor != null)
        {
            gamepadCursor.gameObject.SetActive(true);
            gamepadCursor.position = _cursorPos;
        }

        if (pad.buttonSouth.wasPressedThisFrame) PickAt(_cursorPos);   // A / Cross
        if (pad.buttonEast.wasPressedThisFrame) GoBack();              // B / Circle

        if (pad.buttonNorth.wasPressedThisFrame && _state == State.Expanded && _selected != null)
            _selected.ToggleFreeRoam();                                // Y / Triangle
    }

    // Shared by mouse clicks and the gamepad cursor.
    private void PickAt(Vector2 screenPosition)
    {
        if (viewCamera == null) return;

        Ray ray = viewCamera.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, maxPickDistance))
        {
            GoBack(); // clicked empty space
            return;
        }

        var root = hit.collider.GetComponentInParent<ConstellationRoot>();
        var star = hit.collider.GetComponentInParent<StarPresenter>();
        HandleClick(root, star);
    }

    private void HandleClick(ConstellationRoot root, StarPresenter star)
    {
        switch (_state)
        {
            case State.Overview:
                if (root != null) SelectConstellation(root);        // first click: show its name
                break;

            case State.Named:
                if (root == null) GoBack();
                else if (root == _selected) ExpandSelected();       // second click: show all its stars
                else SelectConstellation(root);                     // clicked a different one
                break;

            case State.Expanded:
                if (star != null && root == _selected)
                {
                    if (popup != null) popup.Show(star);            // small panel next to the star
                    _starPanelOpen = true;
                }
                else GoBack();
                break;
        }
    }

    private void SelectConstellation(ConstellationRoot root)
    {
        _selected = root;
        _state = State.Named;
        if (popup != null) popup.ShowConstellation(root);
    }

    private void ExpandSelected()
    {
        _state = State.Expanded;
        _starPanelOpen = false;

        foreach (var c in constellations)
            if (c != null && c != _selected) c.gameObject.SetActive(false);

        if (popup != null) popup.Hide();
        if (orbitCamera != null) orbitCamera.target = _selected.transform;
    }

    private void GoBack()
    {
        switch (_state)
        {
            case State.Named:
                if (popup != null) popup.Hide();
                _selected = null;
                _state = State.Overview;
                break;

            case State.Expanded:
                if (_starPanelOpen)
                {
                    if (popup != null) popup.Hide();                // close the star panel first
                    _starPanelOpen = false;
                }
                else
                {
                    _selected.ResetToFlatInstant();
                    SetAllConstellationsVisible(true);
                    _selected = null;
                    _state = State.Overview;
                }
                break;
        }
    }

    private void SetAllConstellationsVisible(bool visible)
    {
        foreach (var c in constellations)
            if (c != null) c.gameObject.SetActive(visible);
    }
}

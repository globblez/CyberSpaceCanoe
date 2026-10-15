using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Put this directly on each star GameObject your friends placed in the
/// scene (needs a Collider on the same object for clicking to work).
/// Fill in the fields in the Inspector - no separate data asset needed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class StarPresenter : MonoBehaviour
{
    [Header("Info shown in the popup")]
    public string starName;
    [TextArea(3, 8)] public string infoText;
    public Sprite infoImage;
    [Tooltip("True distance from Earth, in light years.")]
    public float distanceLightYears = 500f;
    [Tooltip("Star's radius compared to the Sun (e.g. 1000 = 1000x the Sun's radius). Used to show an Earth-comparison in the popup.")]
    public float radiusInSolarRadii = 1f;

    [Header("Gamepad highlight")]
    public float pulseSpeed = 4f;
    public float pulseAmount = 0.25f;

    public event System.Action<StarPresenter> OnClicked;

    private Vector3 _baseScale;
    private bool _selected;
    private Renderer _renderer;
    private Collider _collider;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _collider = GetComponent<Collider>();
    }

    private void Start() => _baseScale = transform.localScale;

    /// <summary>
    /// World-space point at the visual center of this star. Imported .obj
    /// pieces often keep their transform at (0,0,0) with the geometry offset
    /// inside the mesh, so transform.position would point at the wrong place.
    /// </summary>
    public Vector3 AnchorWorldPosition
    {
        get
        {
            if (_renderer != null && _renderer.bounds.size.sqrMagnitude > 0f) return _renderer.bounds.center;
            if (_collider != null) return _collider.bounds.center;
            return transform.position;
        }
    }

    // Mouse / touch click
    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        OnClicked?.Invoke(this);
    }

    // Called by the gamepad navigator
    public void SetSelected(bool selected)
    {
        _selected = selected;
        if (!selected) transform.localScale = _baseScale;
    }

    private void Update()
    {
        if (!_selected) return;
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = _baseScale * pulse;
    }
}

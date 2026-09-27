using UnityEngine;

/// <summary>
/// Add alongside StarClickHandler on the star prefab. Gives a gentle
/// pulsing scale so the player can see which star is currently selected
/// via gamepad (there's no cursor to show that otherwise).
/// </summary>
public class StarHighlight : MonoBehaviour
{
    public float pulseSpeed = 4f;
    public float pulseAmount = 0.25f;

    private Vector3 _baseScale;
    private bool _selected;

    // Start (not Awake) so this runs after ConstellationController has
    // already applied the star's sizeMultiplier to localScale.
    private void Start() => _baseScale = transform.localScale;

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

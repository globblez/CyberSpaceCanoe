using UnityEngine;

/// <summary>
/// Assign your six constellation root GameObjects (Orion, Ursa Major, Ursa
/// Minor, Bootes, Taurus, Scorpius) in the Inspector. Wire each selection
/// button's OnClick to SelectConstellation(index) in that same order, and
/// your "back" button to BackToSelection().
/// </summary>
public class ConstellationSwitcher : MonoBehaviour
{
    [Tooltip("Assign in the same order as your selection buttons.")]
    public ConstellationRoot[] constellations;

    public GameObject selectionPanel;
    public GameObject viewerPanel;
    public OrbitCamera orbitCamera;
    public InfoPopupUI popup;

    private ConstellationRoot _active;

    private void Awake()
    {
        // Make sure only one constellation is visible at a time to start.
        foreach (var c in constellations)
            if (c != null) c.gameObject.SetActive(false);
    }

    public void SelectConstellation(int index)
    {
        if (index < 0 || index >= constellations.Length || constellations[index] == null) return;

        if (_active != null)
        {
            _active.OnStarClicked -= HandleStarClicked;
            _active.gameObject.SetActive(false);
        }

        _active = constellations[index];
        _active.gameObject.SetActive(true);
        _active.ResetToFlatInstant();
        _active.OnStarClicked += HandleStarClicked;

        if (orbitCamera != null) orbitCamera.target = _active.transform;
        popup?.Hide();

        if (selectionPanel != null) selectionPanel.SetActive(false);
        if (viewerPanel != null) viewerPanel.SetActive(true);
    }

    public void BackToSelection()
    {
        if (_active != null) _active.gameObject.SetActive(false);
        _active = null;

        popup?.Hide();
        if (viewerPanel != null) viewerPanel.SetActive(false);
        if (selectionPanel != null) selectionPanel.SetActive(true);
    }

    public void ToggleFreeRoamOnActive() => _active?.ToggleFreeRoam();

    public ConstellationRoot ActiveConstellation => _active;

    private void HandleStarClicked(StarPresenter star) => popup?.Show(star);
}

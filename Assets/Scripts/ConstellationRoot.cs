using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on the root GameObject of each constellation prefab (the one
/// containing all the hand-placed stars as children). It automatically
/// finds every StarPresenter underneath - no manual star list needed.
///
/// Whatever position each star was hand-placed at is treated as its
/// "sky view" position (exactly as seen from Earth). Free Roam pushes
/// each star back along local Z by its distanceLightYears, then eases
/// back to the original placement when toggled off.
/// </summary>
public class ConstellationRoot : MonoBehaviour
{
    [Tooltip("Scene units per light year when expanded into free roam.")]
    public float distanceScale = 0.02f;
    [Tooltip("Seconds for the flat <-> free-roam transition.")]
    public float transitionDuration = 1.2f;

    [Header("Label shown when the constellation is clicked")]
    [Tooltip("Leave empty to use this GameObject's name.")]
    public string displayName;
    [TextArea(2, 5)] public string description;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;

    private Renderer[] _renderers;

    /// <summary>World-space center of everything visible in this constellation.</summary>
    public Vector3 CenterWorld
    {
        get
        {
            if (_renderers == null) _renderers = GetComponentsInChildren<Renderer>(true);
            if (_renderers.Length == 0) return transform.position;

            Bounds b = _renderers[0].bounds;
            for (int i = 1; i < _renderers.Length; i++) b.Encapsulate(_renderers[i].bounds);
            return b.center;
        }
    }

    public StarPresenter[] Stars { get; private set; }
    public event System.Action<StarPresenter> OnStarClicked;

    private Vector3[] _flatPositions;
    private bool _freeRoam;
    private Coroutine _transitionRoutine;

    private void Awake()
    {
        Stars = GetComponentsInChildren<StarPresenter>(includeInactive: true);
        _flatPositions = new Vector3[Stars.Length];
        for (int i = 0; i < Stars.Length; i++)
        {
            _flatPositions[i] = Stars[i].transform.localPosition;
            Stars[i].OnClicked += star => OnStarClicked?.Invoke(star);
        }
    }

    public void ToggleFreeRoam() => SetFreeRoam(!_freeRoam);

    public void SetFreeRoam(bool enabled)
    {
        if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
        _freeRoam = enabled;
        _transitionRoutine = StartCoroutine(AnimateTransition(enabled));
    }

    /// <summary>Resets to flat sky view instantly, no animation - call when switching constellations.</summary>
    public void ResetToFlatInstant()
    {
        if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
        _freeRoam = false;
        for (int i = 0; i < Stars.Length; i++)
            Stars[i].transform.localPosition = _flatPositions[i];
    }

    private IEnumerator AnimateTransition(bool toFreeRoam)
    {
        var start = new Vector3[Stars.Length];
        var end = new Vector3[Stars.Length];

        for (int i = 0; i < Stars.Length; i++)
        {
            start[i] = Stars[i].transform.localPosition;
            var flat = _flatPositions[i];
            end[i] = toFreeRoam
                ? new Vector3(flat.x, flat.y, flat.z - Stars[i].distanceLightYears * distanceScale)
                : flat;
        }

        float t = 0f;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, t / transitionDuration);
            for (int i = 0; i < Stars.Length; i++)
                Stars[i].transform.localPosition = Vector3.Lerp(start[i], end[i], f);
            yield return null;
        }

        for (int i = 0; i < Stars.Length; i++)
            Stars[i].transform.localPosition = end[i];
    }
}

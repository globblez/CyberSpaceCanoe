using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns a constellation's stars, draws connecting lines, and can smoothly
/// morph between:
///   - "Sky view": all stars flattened to z=0, exactly as seen from Earth
///   - "Free roam": each star pushed back along z by its real distance,
///     revealing how spread out they actually are in space.
/// </summary>
public class ConstellationController : MonoBehaviour
{
    [Header("Setup")]
    public ConstellationData data;
    public GameObject starPrefab;      // must have a StarClickHandler on it
    public LineRenderer linePrefab;

    [Header("Layout")]
    [Tooltip("Scene units per light year when expanded into free roam.")]
    public float distanceScale = 0.02f;
    [Tooltip("Seconds for the flat <-> free-roam transition.")]
    public float transitionDuration = 1.2f;

    private readonly List<Transform> _starTransforms = new List<Transform>();
    private readonly List<LineRenderer> _lines = new List<LineRenderer>();
    private bool _freeRoam;
    private Coroutine _transitionRoutine;

    public event System.Action<StarInfo> OnStarClicked;

    public int StarCount => data != null ? data.stars.Length : 0;
    public StarInfo GetStarInfo(int index) => data.stars[index];
    public Transform GetStarTransform(int index) => _starTransforms[index];

    private void Start()
    {
        if (data != null) Build(data);
    }

    public void Build(ConstellationData constellation)
    {
        Clear();
        data = constellation;

        foreach (var star in data.stars)
        {
            var go = Instantiate(starPrefab, transform);
            go.transform.localPosition = FlatPosition(star);
            go.transform.localScale = Vector3.one * star.sizeMultiplier;
            go.name = star.starName;

            var handler = go.GetComponent<StarClickHandler>();
            if (handler != null)
            {
                handler.starInfo = star;
                handler.OnClicked += HandleStarClicked;
            }

            _starTransforms.Add(go.transform);
        }

        foreach (var c in data.connections)
        {
            var line = Instantiate(linePrefab, transform);
            line.positionCount = 2;
            line.SetPosition(0, _starTransforms[c.a].localPosition);
            line.SetPosition(1, _starTransforms[c.b].localPosition);
            _lines.Add(line);
        }
    }

    public void ToggleFreeRoam()
    {
        SetFreeRoam(!_freeRoam);
    }

    public void SetFreeRoam(bool enabled)
    {
        if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);
        _freeRoam = enabled;
        _transitionRoutine = StartCoroutine(AnimateTransition(enabled));
    }

    private System.Collections.IEnumerator AnimateTransition(bool toFreeRoam)
    {
        var startPositions = new Vector3[_starTransforms.Count];
        var endPositions = new Vector3[_starTransforms.Count];

        for (int i = 0; i < _starTransforms.Count; i++)
        {
            startPositions[i] = _starTransforms[i].localPosition;
            endPositions[i] = toFreeRoam ? TruePosition(data.stars[i]) : FlatPosition(data.stars[i]);
        }

        float t = 0f;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, t / transitionDuration);

            for (int i = 0; i < _starTransforms.Count; i++)
                _starTransforms[i].localPosition = Vector3.Lerp(startPositions[i], endPositions[i], f);

            RefreshLines();
            yield return null;
        }

        for (int i = 0; i < _starTransforms.Count; i++)
            _starTransforms[i].localPosition = endPositions[i];
        RefreshLines();
    }

    private void RefreshLines()
    {
        for (int i = 0; i < data.connections.Length; i++)
        {
            var c = data.connections[i];
            _lines[i].SetPosition(0, _starTransforms[c.a].localPosition);
            _lines[i].SetPosition(1, _starTransforms[c.b].localPosition);
        }
    }

    private Vector3 FlatPosition(StarInfo star) => new Vector3(star.skyPosition.x, star.skyPosition.y, 0f);

    private Vector3 TruePosition(StarInfo star) =>
        new Vector3(star.skyPosition.x, star.skyPosition.y, -star.distanceLightYears * distanceScale);

    private void HandleStarClicked(StarInfo star) => OnStarClicked?.Invoke(star);

    private void Clear()
    {
        foreach (var t in _starTransforms) if (t != null) Destroy(t.gameObject);
        foreach (var l in _lines) if (l != null) Destroy(l.gameObject);
        _starTransforms.Clear();
        _lines.Clear();
    }
}

using UnityEngine;

/// <summary>
/// Data for a single star in a constellation. Create instances via
/// Assets > Create > Constellations > Star, or build them at runtime.
/// </summary>
[CreateAssetMenu(fileName = "NewStar", menuName = "Constellations/Star")]
public class StarInfo : ScriptableObject
{
    [Header("Identity")]
    public string starName;

    [Header("Popup content")]
    [TextArea(3, 8)] public string infoText;
    public Sprite infoImage;

    [Header("Position as seen from Earth (flat sky plane, arbitrary units)")]
    public Vector2 skyPosition;

    [Header("True distance from Earth, in light years")]
    public float distanceLightYears = 500f;

    [Header("Visual size multiplier (relative brightness / actual size)")]
    public float sizeMultiplier = 1f;
}

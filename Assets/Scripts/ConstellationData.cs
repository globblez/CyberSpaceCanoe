using UnityEngine;

/// <summary>
/// One constellation: its stars, the connective lines between them (by index),
/// and where the Orion-Nebula-style "extra objects" go too — StarInfo covers both.
/// </summary>
[CreateAssetMenu(fileName = "NewConstellation", menuName = "Constellations/Constellation")]
public class ConstellationData : ScriptableObject
{
    public string constellationName;
    public StarInfo[] stars;

    [Tooltip("Each entry connects stars[a] to stars[b] with a line.")]
    public Connection[] connections;

    [System.Serializable]
    public struct Connection
    {
        public int a;
        public int b;
    }
}

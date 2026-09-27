#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Editor-only utility. Menu: Constellations > Build Orion Sample Data.
/// Generates StarInfo assets + a ConstellationData asset for Orion using
/// the values from your reference sheet (sky position is a rough manual
/// layout matching the diagram; adjust freely in the Inspector afterward).
/// </summary>
public static class OrionDataBuilder
{
    private const string Folder = "Assets/Constellations/Orion";

    private struct StarSeed
    {
        public string name; public Vector2 pos; public float dist; public float size;
        public StarSeed(string n, float x, float y, float d, float s) { name = n; pos = new Vector2(x, y); dist = d; size = s; }
    }

    [MenuItem("Constellations/Build Orion Sample Data")]
    public static void Build()
    {
        Directory.CreateDirectory(Folder);

        var seeds = new[]
        {
            new StarSeed("Meissa",       1.0f,  5.0f, 1200f, 1.0f),
            new StarSeed("Betelgeuse",  -1.0f,  3.5f,  700f, 3.0f),
            new StarSeed("Bellatrix",    2.3f,  3.3f,  245f, 1.5f),
            new StarSeed("Alnitak",      0.6f,  0.6f, 1000f, 1.4f),
            new StarSeed("Alnilam",      1.2f,  0.7f, 1250f, 1.6f),
            new StarSeed("Mintaka",      1.8f,  0.9f, 1200f, 1.3f),
            new StarSeed("Orion Nebula", 1.3f, -1.0f, 1300f, 0.8f),
            new StarSeed("Saiph",       -1.0f, -3.2f,  650f, 1.2f),
            new StarSeed("Rigel",        1.9f, -3.4f,  850f, 2.4f),
        };

        var stars = new StarInfo[seeds.Length];
        for (int i = 0; i < seeds.Length; i++)
        {
            var s = ScriptableObject.CreateInstance<StarInfo>();
            s.starName = seeds[i].name;
            s.skyPosition = seeds[i].pos;
            s.distanceLightYears = seeds[i].dist;
            s.sizeMultiplier = seeds[i].size;
            s.infoText = $"{seeds[i].name} is part of Orion, roughly {seeds[i].dist:0} light years away.";
            AssetDatabase.CreateAsset(s, $"{Folder}/{seeds[i].name.Replace(" ", "_")}.asset");
            stars[i] = s;
        }

        var connections = new[]
        {
            new ConstellationData.Connection{a=0,b=1}, // Meissa-Betelgeuse
            new ConstellationData.Connection{a=0,b=2}, // Meissa-Bellatrix
            new ConstellationData.Connection{a=1,b=3}, // Betelgeuse-Alnitak
            new ConstellationData.Connection{a=2,b=5}, // Bellatrix-Mintaka
            new ConstellationData.Connection{a=3,b=4}, // Alnitak-Alnilam
            new ConstellationData.Connection{a=4,b=5}, // Alnilam-Mintaka
            new ConstellationData.Connection{a=3,b=7}, // Alnitak-Saiph
            new ConstellationData.Connection{a=5,b=8}, // Mintaka-Rigel
            new ConstellationData.Connection{a=7,b=8}, // Saiph-Rigel
        };

        var constellation = ScriptableObject.CreateInstance<ConstellationData>();
        constellation.constellationName = "Orion";
        constellation.stars = stars;
        constellation.connections = connections;
        AssetDatabase.CreateAsset(constellation, $"{Folder}/Orion.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Orion sample data built at " + Folder);
    }
}
#endif

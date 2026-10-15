using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// Put this on the "Stars" parent ONLY (not on constellations or dots).
/// Dots are found by their MESH, named after the diagram labels
/// (Betelgeuse, Eta UMi, Mizar & Alcor, Andromeda Galaxy, ...).
/// Right-click the component header > "Apply Styles".
///
/// How it works: each dot's real position and size come from its mesh bounds
/// (so offset/baked meshes work). A glow sprite is added as a CHILD called
/// "StarGlow"; the original dot keeps its collider and scripts, only its
/// MeshRenderer is switched off. Lines are created inside each constellation.
[ExecuteAlways]
public class StarStyler : MonoBehaviour
{
    [System.Serializable]
    public class ExtendedObject
    {
        public string nameContains;
        [Tooltip("Real photo. Leave empty to keep the original model for now.")]
        public Sprite image;
        [Tooltip("Photo size as a multiple of the original object's largest dimension")]
        public float sizeMultiplier = 2f;
    }

    [Header("Stars")]
    [Tooltip("Glow diameter as a multiple of the original dot's diameter (then scaled by brightness)")]
    public float glowScale = 3f;
    public bool billboardToCamera = true;
    public bool hideOriginalDots = true;

    [Header("Constellation lines")]
    public bool drawLines = true;
    [Tooltip("0 = automatic (relative to dot size). Otherwise world units.")]
    public float lineWidth = 0f;
    public Color lineColor = new Color(0.6f, 0.75f, 1f, 0.4f);

    [Header("Galaxies / nebulae / clusters (drop real photos here)")]
    public List<ExtendedObject> extendedObjects = new List<ExtendedObject>
    {
        new ExtendedObject { nameContains = "Andromeda Galaxy", sizeMultiplier = 2f },
        new ExtendedObject { nameContains = "Orion Nebula",     sizeMultiplier = 3f },
        new ExtendedObject { nameContains = "Pleiades",         sizeMultiplier = 3f },
    };

    // ---------------------------------------------------------------- data

    struct StarInfo
    {
        public Color color; public float size;
        public StarInfo(Color c, float s) { color = c; size = s; }
    }

    static readonly Color Red = new Color(1.00f, 0.40f, 0.25f);
    static readonly Color Org = new Color(1.00f, 0.68f, 0.40f);
    static readonly Color Yel = new Color(1.00f, 0.90f, 0.60f);
    static readonly Color YWh = new Color(1.00f, 0.96f, 0.80f);
    static readonly Color Wht = new Color(0.96f, 0.96f, 1.00f);
    static readonly Color BWh = new Color(0.75f, 0.85f, 1.00f);
    static readonly Color Blu = new Color(0.60f, 0.72f, 1.00f);

    static Dictionary<string, StarInfo> db;
    static void S(string name, Color c, float size) { db[Norm(name)] = new StarInfo(c, size); }

    static void BuildDb()
    {
        db = new Dictionary<string, StarInfo>();

        // Orion
        S("Betelgeuse", Red, 3.5f); S("Rigel", BWh, 3.5f); S("Bellatrix", BWh, 2.2f);
        S("Saiph", Blu, 1.8f); S("Meissa", Blu, 1.5f);
        S("Alnitak", Blu, 2.0f); S("Alnilam", Blu, 2.3f); S("Mintaka", Blu, 1.9f);

        // Ursa Major
        S("Alkaid", BWh, 1.8f); S("Mizar & Alcor", Wht, 2.0f); S("Alioth", Wht, 2.2f);
        S("Megrez", Wht, 1.5f); S("Dubhe", Org, 2.2f); S("Merak", Wht, 1.9f); S("Phecda", Wht, 1.8f);

        // Ursa Minor
        S("Polaris", YWh, 2.4f); S("Pherkad", Wht, 1.5f); S("Eta UMi", Wht, 1.0f);
        S("Zeta UMi", Wht, 1.2f); S("Kochab", Org, 2.0f); S("Urodelus", Yel, 1.0f); S("Yildun", Wht, 1.1f);

        // Canis Major
        S("Sirius", BWh, 4.0f); S("Gamma CMa", Blu, 1.3f); S("Theta CMa", Org, 1.3f);
        S("Iota CMa", Blu, 1.0f); S("Omicron2 CMa", Blu, 1.4f); S("Delta CMa", YWh, 2.4f);
        S("Eta CMa", Blu, 2.0f); S("Epsilon CMa", BWh, 2.8f); S("Omicron1 CMa", Org, 1.4f);
        S("Nu2 CMa", Yel, 1.2f); S("Beta CMa", Blu, 2.2f);

        // Bootes
        S("Arcturus", Org, 3.4f); S("Nekkar", Yel, 1.4f); S("Thiba", Wht, 1.6f);
        S("Seginus", Wht, 1.6f); S("Rho Boo", Org, 1.3f); S("Epsilon Boo", Org, 2.0f);
        S("Muphrid", Yel, 1.6f); S("Zeta Boo", Wht, 1.2f);

        // Taurus
        S("Aldebaran", Org, 3.0f); S("Beta Tau", BWh, 2.3f); S("Zeta Tau", Blu, 1.6f);
        S("Epsilon Tau", Org, 1.6f); S("Delta Tau", Yel, 1.4f); S("Gamma Tau", Org, 1.6f);
        S("Theta Tau", Yel, 1.5f); S("Lambda Tau", BWh, 1.4f); S("Omicron Tau", Org, 1.2f);

        // Scorpius
        S("Antares", Red, 3.4f); S("Acrab", Blu, 1.7f); S("Dschubba", Blu, 2.0f);
        S("Fang", Blu, 1.5f); S("Sigma Sco", Blu, 1.5f); S("Tau Sco", Blu, 1.6f);
        S("Epsilon Sco", Org, 1.9f); S("Zeta Sco", Org, 1.5f); S("Eta Sco", Wht, 1.5f);
        S("Theta Sco", YWh, 2.2f); S("Iota Sco", YWh, 1.4f); S("Lambda Sco", Blu, 2.5f);
        S("Upsilon Sco", Blu, 2.0f);

        // Andromeda
        S("Alpheratz", BWh, 2.0f); S("Mirach", Red, 2.2f); S("Almach", Org, 1.9f);
        S("Zeta And", Org, 1.2f); S("Eta And", Yel, 1.0f); S("Epsilon And", Yel, 1.2f);
        S("Delta And", Org, 1.6f); S("Pi And", Blu, 1.3f); S("Sigma And", Wht, 1.1f);
        S("Mu And", Wht, 1.3f); S("Iota And", Blu, 1.1f); S("Kappa And", Blu, 1.1f);
        S("Omicron And", Blu, 1.0f); S("Lambda And", Yel, 1.2f); S("51 And", Org, 1.0f);
    }

    static readonly string[] segments =
    {
        // Orion
        "Meissa-Betelgeuse", "Meissa-Bellatrix", "Betelgeuse-Alnitak", "Bellatrix-Mintaka",
        "Alnitak-Alnilam", "Alnilam-Mintaka", "Alnitak-Saiph", "Mintaka-Rigel", "Saiph-Rigel",
        // Ursa Major
        "Alkaid-Mizar & Alcor", "Mizar & Alcor-Alioth", "Alioth-Megrez", "Megrez-Dubhe",
        "Megrez-Phecda", "Dubhe-Merak", "Merak-Phecda",
        // Ursa Minor
        "Pherkad-Eta UMi", "Eta UMi-Zeta UMi", "Zeta UMi-Kochab", "Kochab-Pherkad",
        "Zeta UMi-Urodelus", "Urodelus-Yildun", "Yildun-Polaris",
        // Canis Major
        "Gamma CMa-Theta CMa", "Gamma CMa-Iota CMa", "Theta CMa-Iota CMa", "Iota CMa-Sirius",
        "Sirius-Omicron2 CMa", "Omicron2 CMa-Delta CMa", "Delta CMa-Eta CMa",
        "Delta CMa-Epsilon CMa", "Epsilon CMa-Omicron1 CMa", "Omicron1 CMa-Nu2 CMa",
        "Nu2 CMa-Beta CMa", "Beta CMa-Sirius",
        // Bootes
        "Nekkar-Thiba", "Nekkar-Seginus", "Thiba-Epsilon Boo", "Epsilon Boo-Arcturus",
        "Seginus-Rho Boo", "Rho Boo-Arcturus", "Arcturus-Muphrid", "Arcturus-Zeta Boo",
        // Taurus
        "Beta Tau-Epsilon Tau", "Epsilon Tau-Delta Tau", "Delta Tau-Gamma Tau",
        "Gamma Tau-Lambda Tau", "Lambda Tau-Omicron Tau", "Zeta Tau-Aldebaran",
        "Aldebaran-Theta Tau", "Theta Tau-Gamma Tau",
        // Scorpius
        "Acrab-Dschubba", "Dschubba-Fang", "Dschubba-Sigma Sco", "Sigma Sco-Antares",
        "Antares-Tau Sco", "Tau Sco-Epsilon Sco", "Epsilon Sco-Zeta Sco", "Zeta Sco-Eta Sco",
        "Eta Sco-Theta Sco", "Theta Sco-Iota Sco", "Iota Sco-Lambda Sco", "Lambda Sco-Upsilon Sco",
        // Andromeda
        "Alpheratz-Delta And", "Eta And-Zeta And", "Zeta And-Epsilon And", "Epsilon And-Delta And",
        "Delta And-Pi And", "Pi And-Mirach", "Pi And-Sigma And", "Sigma And-Iota And",
        "Iota And-Omicron And", "Iota And-Kappa And", "Kappa And-Lambda And",
        "Mirach-Mu And", "Mu And-51 And", "Mirach-Almach",
    };

    // ------------------------------------------------------------- runtime

    const string GlowName = "StarGlow";
    const string LinesRootName = "ConstellationLines";

    static Sprite glowSprite;

    [SerializeField, HideInInspector] List<Transform> glows = new List<Transform>();
    readonly Dictionary<string, Vector3> centers = new Dictionary<string, Vector3>();
    readonly Dictionary<string, Transform> owners = new Dictionary<string, Transform>();

    void Start() { if (Application.isPlaying) ApplyStyles(); }

    [ContextMenu("Apply Styles")]
    public void ApplyStyles()
    {
        if (db == null) BuildDb();
        if (glowSprite == null) glowSprite = MakeGlowSprite();

        ClearGenerated();
        glows.Clear(); centers.Clear(); owners.Clear();

        var ignored = new List<string>();
        float diaSum = 0f; int diaCount = 0;

        foreach (Transform t in GetComponentsInChildren<Transform>(true))
        {
            if (t == transform) continue;

            // Dots are identified by their mesh. Groups, glows and lines have none.
            var mf = t.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            string key = Norm(t.name);

            // Real position and size from the mesh bounds (works for offset/baked meshes)
            Bounds mb = mf.sharedMesh.bounds;
            Vector3 center = t.TransformPoint(mb.center);
            Vector3 ls = t.lossyScale;
            float diameter = Mathf.Max(mb.size.x * Mathf.Abs(ls.x),
                                       mb.size.y * Mathf.Abs(ls.y),
                                       mb.size.z * Mathf.Abs(ls.z));
            if (diameter <= 0f) diameter = 1f;

            // 1) galaxy / nebula / cluster
            ExtendedObject ext = FindExtended(key);
            if (ext != null)
            {
                centers[key] = center; owners[key] = t;
                if (ext.image == null) continue;   // keep the original model until a photo is set
                MakeGlow(t, center, ext.image, Color.white, diameter * ext.sizeMultiplier);
                HideDot(t);
                continue;
            }

            // 2) known stars
            StarInfo info;
            bool found = db.TryGetValue(key, out info);
            if (!found)
                foreach (var kv in db)
                    if (kv.Key.Length >= 7 && key.Contains(kv.Key)) { info = kv.Value; found = true; break; }

            if (!found) { ignored.Add(t.name); continue; }

            MakeGlow(t, center, glowSprite, info.color, diameter * glowScale * info.size / 2f);
            HideDot(t);
            centers[key] = center; owners[key] = t;
            diaSum += diameter; diaCount++;
        }

        if (drawLines) DrawLines(diaCount > 0 ? diaSum / diaCount : 1f);

        if (ignored.Count > 0)
            Debug.LogWarning("StarStyler: ignored mesh objects with unrecognized names: " +
                             string.Join(", ", ignored));
    }

    void MakeGlow(Transform dot, Vector3 center, Sprite sprite, Color color, float worldSize)
    {
        var go = new GameObject(GlowName);
        go.transform.SetParent(dot, true);
        go.transform.position = center;
        go.transform.rotation = CameraRotation();

        float native = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
        if (native <= 0f) native = 1f;
        float k = worldSize / native;
        Vector3 ls = dot.lossyScale;
        go.transform.localScale = new Vector3(k / SafeAbs(ls.x), k / SafeAbs(ls.y), k / SafeAbs(ls.z));

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        glows.Add(go.transform);
    }

    void HideDot(Transform t)
    {
        if (!hideOriginalDots) return;
        var r = t.GetComponent<MeshRenderer>();
        if (r) r.enabled = false;
    }

    ExtendedObject FindExtended(string key)
    {
        foreach (var e in extendedObjects)
            if (!string.IsNullOrEmpty(e.nameContains) && key.Contains(Norm(e.nameContains)))
                return e;
        return null;
    }

    void ClearGenerated()
    {
        var doomed = new List<GameObject>();
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
            if (t != transform && (t.name == GlowName || t.name == LinesRootName))
                doomed.Add(t.gameObject);

        foreach (var g in doomed)
        {
            if (!g) continue;
            g.name += "_old";
            g.SetActive(false);
            if (Application.isPlaying) Destroy(g); else DestroyImmediate(g);
        }
    }

    void DrawLines(float avgDiameter)
    {
        var mat = new Material(Shader.Find("Sprites/Default"));
        var holders = new Dictionary<Transform, Transform>();
        var missing = new HashSet<string>();
        float width = lineWidth > 0f ? lineWidth : avgDiameter * 0.08f;

        foreach (string seg in segments)
        {
            string[] p = seg.Split('-');
            string ka = Norm(p[0]), kb = Norm(p[1]);
            bool okA = centers.ContainsKey(ka), okB = centers.ContainsKey(kb);
            if (!okA) missing.Add(p[0]);
            if (!okB) missing.Add(p[1]);
            if (!okA || !okB) continue;

            // Put the line inside the constellation group so it hides/shows with it
            Transform group = owners[ka].parent != null ? owners[ka].parent : transform;
            Transform holder;
            if (!holders.TryGetValue(group, out holder))
            {
                var h = new GameObject(LinesRootName);
                h.transform.SetParent(group, false);
                holder = h.transform;
                holders[group] = holder;
            }

            var go = new GameObject("Line_" + seg);
            go.transform.SetParent(holder, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = 2;
            lr.SetPosition(0, centers[ka]);
            lr.SetPosition(1, centers[kb]);
            lr.startWidth = lr.endWidth = width;
            lr.sharedMaterial = mat;
            lr.startColor = lr.endColor = lineColor;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
        }

        if (missing.Count > 0)
            Debug.LogWarning("StarStyler: no star found for: " + string.Join(", ", missing) +
                             ". Check those names match the diagram labels.");
    }

    // ----------------------------------------------------------- billboard

    void LateUpdate()
    {
        if (Application.isPlaying) Face(CameraRotation());
    }

    void Face(Quaternion rot)
    {
        if (!billboardToCamera) return;
        foreach (var g in glows) if (g) g.rotation = rot;
    }

#if UNITY_EDITOR
    void OnEnable()  { SceneView.duringSceneGui += OnSceneGui; }
    void OnDisable() { SceneView.duringSceneGui -= OnSceneGui; }
    void OnSceneGui(SceneView sv)
    {
        if (!Application.isPlaying && sv != null && sv.camera != null)
            Face(sv.camera.transform.rotation);
    }
#endif

    static Quaternion CameraRotation()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && SceneView.lastActiveSceneView != null)
            return SceneView.lastActiveSceneView.camera.transform.rotation;
#endif
        return Camera.main != null ? Camera.main.transform.rotation : Quaternion.identity;
    }

    // ------------------------------------------------------------- helpers

    static float SafeAbs(float v) { v = Mathf.Abs(v); return v < 0.0001f ? 1f : v; }

    // lowercase letters/digits only; drops "(1)" suffixes, spaces, "&", etc.
    static string Norm(string s)
    {
        s = Regex.Replace(s, @"\s*\(\d+\)\s*$", "");
        var sb = new StringBuilder();
        foreach (char c in s)
            if (char.IsLetterOrDigit(c)) sb.Append(char.ToLowerInvariant(c));
        return sb.ToString();
    }

    static Sprite MakeGlowSprite()
    {
        const int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Vector2 c = new Vector2(size / 2f, size / 2f);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c) / (size / 2f);
                float core = Mathf.Clamp01(1f - d * 3.5f);
                float halo = Mathf.Pow(Mathf.Clamp01(1f - d), 3f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(core + halo * 0.6f)));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}

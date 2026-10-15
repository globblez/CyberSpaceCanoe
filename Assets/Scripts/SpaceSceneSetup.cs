using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Drop this on an empty GameObject (e.g. "SpaceEnvironment") anywhere in
/// your scene. On start, it sets the camera background to black, disables
/// fog, adds a soft directional "sun" light so your constellation models
/// aren't flat black on their unlit side, and scatters a procedural
/// starfield around the scene - no downloaded skybox needed.
/// </summary>
public class SpaceSceneSetup : MonoBehaviour
{
    [Header("Starfield")]
    public int starCount = 2000;
    public float starFieldRadius = 500f;
    public float minStarSize = 0.5f;
    public float maxStarSize = 2f;
    public Color starColor = Color.white;

    [Header("Lighting")]
    public Color ambientColor = new Color(0.02f, 0.02f, 0.05f);
    public Color sunColor = new Color(0.9f, 0.9f, 1f);
    public float sunIntensity = 0.8f;

    private void Awake()
    {
        SetupCameraAndLighting();
        GenerateStarfield();
    }

    private void SetupCameraAndLighting()
    {
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;
        }

        RenderSettings.skybox = null;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.fog = false;

        var lightGO = new GameObject("SpaceSunLight");
        lightGO.transform.SetParent(transform);
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = sunColor;
        light.intensity = sunIntensity;
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private void GenerateStarfield()
    {
        var starfieldGO = new GameObject("Starfield");
        starfieldGO.transform.SetParent(transform);

        var ps = starfieldGO.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(minStarSize, maxStarSize);
        main.startColor = starColor;
        main.maxParticles = Mathf.Max(starCount, 1000);
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.enabled = false; // we emit a static field manually below

        var shape = ps.shape;
        shape.enabled = false;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = CreateStarMaterial();

        var emitParams = new ParticleSystem.EmitParams();
        for (int i = 0; i < starCount; i++)
        {
            emitParams.position = Random.onUnitSphere * starFieldRadius;
            emitParams.startSize = Random.Range(minStarSize, maxStarSize);
            emitParams.startColor = starColor;
            ps.Emit(emitParams, 1);
        }
    }

    // Tries a few shader names so this works whether the project uses the
    // Built-in, URP, or HDRP render pipeline.
    private Material CreateStarMaterial()
    {
        Shader shader = Shader.Find("Particles/Standard Unlit")
            ?? Shader.Find("Universal Render Pipeline/Particles/Unlit")
            ?? Shader.Find("Sprites/Default")
            ?? Shader.Find("Unlit/Color");

        var mat = new Material(shader);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", starColor);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", starColor);
        return mat;
    }
}

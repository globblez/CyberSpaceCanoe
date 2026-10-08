using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class StarField : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private float starSizeMin = 0f;
    [Range(0, 100)]
    [SerializeField] private float starSizeMax = 5f; 

    private List<StarDataLoader.Star> stars; 
    private List<GameObject> starObjects;

    private readonly int starFieldScale = 400; //Distance within camera's clipping plane

    private bool orionVisible = false;

    void Start()
    {
        //Read in star data
        StarDataLoader sdl = new();
        stars = sdl.LoadData();

        starObjects = new(); //Intialize the list of game objects

        foreach(StarDataLoader.Star star in stars)
        {
            GameObject stargo = GameObject.CreatePrimitive(PrimitiveType.Quad); //Creates 2D plane game object


            stargo.transform.parent = transform; //Set parent to camera holder. Starfield will travel w/ camera
            stargo.name = $"HR {star.catalog_number}"; //Set star name

            stargo.transform.localPosition = star.position * starFieldScale;
            //stargo.transform.localScale = Vector3.one * Mathf.Lerp(starSizeMin, starSizeMax, star.size); //Lerp between min & max star size
            stargo.transform.LookAt(transform.position); //Have quads face toward the camera
            stargo.transform.Rotate(0, 180, 0);          //Redirect them from facing away

            Material material = stargo.GetComponent<MeshRenderer>().material; //Pull off material
            
            material.shader = Shader.Find("Custom/StarShader");
            material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, star.size));

            material.color = star.s_color;

            starObjects.Add(stargo);
        }
    }

    void OnValidate()
    {
        if (starObjects != null)
        {
            for (int i = 0; i < starObjects.Count; i++)
            {
                //Update size set in the shader
                Material material = starObjects[i].GetComponent<MeshRenderer>().material;
                material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[i].size));
            }
        }
    }

    private readonly (int[], List<(int, int)>) orionConst = (
        new int[] { 2061, 1907, 1790, 1948, 1903, 1852, 2004, 1713 },
        new() { (2061, 1907), (1907, 1790), (1852, 1713), 
                (1903, 1852), (1948, 1903), (1948, 2061), 
                (1948, 2004), (1790, 1852) });

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ToggleConstellation();
    }

    void ToggleConstellation()
    {
        if (orionVisible == false)
            CreateConstellation();
        else
            DeleteConstellation();

        orionVisible = !orionVisible;
    }

    void CreateConstellation()
    {
        int[] constellation = orionConst.Item1;
        List<(int, int)> lines = orionConst.Item2;

        foreach (int starNumber in constellation)
        {
            starObjects[starNumber - 1].GetComponent<MeshRenderer>().material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax * 2, stars[starNumber - 1].size));
        }

        GameObject constellationHolder = new("Orion");
        constellationHolder.transform.parent = transform;

        foreach( (int, int) l in lines)
        {
            int s_index1 = l.Item1 - 1;
            int s_index2 = l.Item2 - 1;

            GameObject line = new("Line");
            line.transform.parent = constellationHolder.transform;

            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lineRenderer.useWorldSpace = false;


            Vector3 pos1 = starObjects[s_index1].transform.position;
            Vector3 pos2 = starObjects[s_index2].transform.position;

            Vector3 dir = (pos2 - pos1).normalized * 5;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, pos1 + dir); //Add dir to end closer to pos2
            lineRenderer.SetPosition(1, pos2 - dir); //Sub dir to end closer to pos1
        }
    }

    void DeleteConstellation()
    {
        int[] constellation = orionConst.Item1;
        List<(int, int)> lines = orionConst.Item2;

        foreach (int starNumber in constellation)
        {
            starObjects[starNumber - 1].GetComponent<MeshRenderer>().material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[starNumber - 1].size));
        }

        Destroy(GameObject.Find("Orion"));

    }
}   

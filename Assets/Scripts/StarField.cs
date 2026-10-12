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

    private bool constSwitch = true;

    private List<(int[], List<(int, int)>)> constellations;
    private Dictionary<int, GameObject> constellationsVisible = new();

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


            //NEW STUFF
            stargo.AddComponent<StarBehavior>();

            starObjects.Add(stargo);
        }

        constellations = new() {orionCon, ursaMajorCon, ursaMinorCon, 
                                bootesCon, taurusCon, scorpiusCon,
                                canisMajorCon, andromedaCon};
        
        if (constSwitch) CreateAll();
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

    private readonly (int[], List<(int, int)>) orionCon = (
        new int[] { 2061, 1907, 1790, 1948, 1903, 1852, 2004, 1713 },
        new() { (2061, 1907), (1907, 1790), (1852, 1713), 
                (1903, 1852), (1948, 1903), (1948, 2061), 
                (1948, 2004), (1790, 1852) });

    private readonly (int[], List<(int, int)>) ursaMajorCon = (
        new int[] { 5191, 5054, 4905, 4660, 4301, 4554, 4295 },
        new() { (5191, 5054), (5054, 4905), (4905, 4660), 
                (4660, 4301), (4660, 4554), (4554, 4295),
                (4295, 4301) });

    private readonly (int[], List<(int, int)>) ursaMinorCon = (
        new int[] { 5735, 6116, 5563, 5903, 6332, 6789, 424 },
        new() { (5735, 6116), (5735, 5563), (6116, 5903), 
                (5563, 5903), (5903, 6322), (6322, 6789),
                (6789, 424) });
            
    private readonly (int[], List<(int, int)>) bootesCon = (
        new int[] { 5340, 5235, 5602, 5435, 5506, 5429, 5478, 5681 },
        new() { (5602, 5435), (5602, 5681), (5435, 5429),
                (5681, 5506), (5506, 5340), (5429, 5340),
                (5340, 5235), (5340, 5478) });

    private readonly (int[], List<(int, int)>) taurusCon = (
        new int[] { 1457, 1791, 1346, 1373, 1409, 1910, 1165, 1239, 1239, 1030, 1165 }, 
        new() { (1910, 1457), (1457, 1411), (1411, 1346),
                (1791, 1409), (1409, 1373), (1373, 1346),
                (1346, 1239), (1239, 1030) });

    private readonly (int[], List<(int, int)>) scorpiusCon = (
        new int[] { 6134, 5984, 5953, 6241, 6553, 6527, 6380, 6262, 6615, 6508, 6084, 6165, 5944 },
        new() { (5984, 5953), (5953, 5944), (5953, 6084),
                (6084, 6134), (6134, 6165), (6165, 6241),
                (6241, 6262), (6262, 6380), (6380, 6553),
                (6553, 6615), (6615, 6527), (6527, 6508) });

    private readonly (int[], List<(int, int)>) canisMajorCon = (
        new int[] { 2491, 2580, 2653, 2294, 2693, 2574, 2657, 2827, 2596, 2429, 2618 },
        new() { (2574, 2657), (2574, 2596), (2657, 2596), 
                (2596, 2491), (2491, 2653), (2491, 2294),
                (2294, 2429), (2429, 2580), (2580, 2618),
                (2618, 2693), (2693, 2827), (2693, 2653) });

    private readonly (int[], List<(int, int)>) andromedaCon = (
        new int[] { 15, 337, 603, 163, 165, 8965, 8961, 68, 271, 215, 8762, 8976, 154, 269, 464 },
        new() { (271, 215), (215, 163), (163, 165), 
                (165, 15), (165, 337), (165, 154),
                (337, 603), (337, 269), (269, 464),
                (337, 154), (154, 68), (68, 8965),
                (8965, 8762), (8965, 8976), (8976, 8961) });

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ToggleConstellation();
    }

    void ToggleConstellation()
    {
        if (constSwitch == false)
            CreateAll();
        else
            DeleteAll();

        constSwitch = !constSwitch;
    }

    void CreateAll()
    {
        if (constellations.Count <= 0) 
            return;

        for (int i = 0; i < constellations.Count; i++)
        {
            CreateConstellation(i);
        }
    }

    void CreateConstellation(int index)
    {
        if (index < 0 || index >= constellations.Count) //Error checking
        {
            Debug.Log("Index is not in range. Cannot create.");
            return;
        }

        
        int[] constellation = constellations[index].Item1;
        List<(int, int)> lines = constellations[index].Item2;

        foreach (int starNumber in constellation)
        {
            starObjects[starNumber - 1].GetComponent<MeshRenderer>().material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax * 2, stars[starNumber - 1].size));
        }

        GameObject constellationHolder = new($"Constellation #{index}");
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

            Vector3 dir = (pos2 - pos1).normalized * 6;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, pos1 + dir); //Add dir to end closer to pos2
            lineRenderer.SetPosition(1, pos2 - dir); //Sub dir to end closer to pos1
        }

        constellationsVisible.Add(index, constellationHolder);
    }

    void DeleteAll()
    {
        if (constellations.Count <= 0) 
            return;

        for (int i = 0; i < constellations.Count; i++)
        {
            DeleteConstellation(i);
        }
    }

    void DeleteConstellation(int index)
    {
        if (index < 0 || index >= constellations.Count) //Error checking
        {
            Debug.Log("Index is not in range. Cannot delete.");
            return;
        }

        int[] constellation = constellations[index].Item1;
        List<(int, int)> lines = constellations[index].Item2;

        foreach (int starNumber in constellation)
        {
            starObjects[starNumber - 1].GetComponent<MeshRenderer>().material.SetFloat("_Size", Mathf.Lerp(starSizeMin, starSizeMax, stars[starNumber - 1].size));
        }

        Destroy(constellationsVisible[index]); //Delete indexed const
        constellationsVisible.Remove(index);   //Remove from visible container
    }
}

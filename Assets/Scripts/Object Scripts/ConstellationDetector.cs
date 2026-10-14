using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class ConstellationDetector : MonoBehaviour
{
    [SerializeField] private float distanceThreshold = 150f;
    [Space(20)]
    [SerializeField] GameObject constellationParent;

    public List<GameObject> constellations = new List<GameObject>();
    public Dictionary<string, float> distDictionary = new();

    private Plane[] cameraFrustum;
    private Camera cam;

    private GameObject selectableObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectableObject = null;

        foreach (Transform child in constellationParent.transform)
        {
            constellations.Add(child.gameObject);
        }

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        constellations.Sort((x, y) => 
                    x.GetComponentInChildren<ConstellationNameUI>().distance.CompareTo(
                        y.GetComponentInChildren<ConstellationNameUI>().distance));

        selectableObject = null;
        List<GameObject> inDistance = new(); 
        foreach (GameObject con in constellations)
        {
            float dist = con.GetComponentInChildren<ConstellationNameUI>().distance;

            if ((dist <= distanceThreshold) && (InCameraView(con)))
                selectableObject = con;
        }

        if (selectableObject != null)
            Debug.Log(selectableObject.name);

        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) Debug.Log("NO GAMEPAD DETECTED!!!!");

        if (gamepad.yButton.isPressed)
        {
            if (selectableObject != null)
                selectableObject.GetComponentInChildren<ConstellationNameUI>().SetActive();
            else
                Debug.Log("Nothing to select...");
        }
    }

    bool InCameraView(GameObject constellationGO)
    {
        Collider col = constellationGO.GetComponentInChildren<Collider>();
        var bounds = col.bounds;
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(cam);

        return GeometryUtility.TestPlanesAABB(cameraFrustum, bounds);
    }
}

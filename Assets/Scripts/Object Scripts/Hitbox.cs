using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string starName = "HR 1903";

    [SerializeField] private Camera centerCam;

    private MeshRenderer renderer;
    private Plane[] cameraFrustum;
    private Collider collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject starGO = GameObject.Find(starName);
        transform.position = starGO.transform.position;

        //transform.LookAt()

        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        var bounds = collider.bounds;
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(centerCam);

        if(GeometryUtility.TestPlanesAABB(cameraFrustum, bounds))
        {
            //print("I see you");
        }
        else
        {
            //print("I dont see u... :(");
        }
    }



}

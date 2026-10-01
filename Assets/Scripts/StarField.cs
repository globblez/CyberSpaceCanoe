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
}   

using UnityEngine;
using System.Collections.Generic;


public class ConstellationDetector : MonoBehaviour
{
    [SerializeField] GameObject constellationParent;

    public List<GameObject> constellations = new List<GameObject>();

    //public List<float> distList = new List<float>();

    public Dictionary<string, float> distDictionary = new(); 



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Transform[] tList = constGetComponentsInChildren<Transform>();

        foreach (Transform child in constellationParent.transform)
        {
            constellations.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {


        constellations.Sort((x, y) => x.GetComponentInChildren<ConstellationNameUI>().distance.CompareTo(y.GetComponentInChildren<ConstellationNameUI>().distance));
        //print(test);

        
    }
}

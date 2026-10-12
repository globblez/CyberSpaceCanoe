using UnityEngine;

public class StarBehavior : MonoBehaviour
{
    private GameObject ccHub;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ccHub = GameObject.Find("CC_HUB");

        transform.LookAt(ccHub.transform.position);
        transform.Rotate(0, 180, 0);
    }
}

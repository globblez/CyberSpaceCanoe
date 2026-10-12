using UnityEngine;

public class RaycastDetect : MonoBehaviour
{

    public LayerMask mask;

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            GameObject lastHit = hit.transform.gameObject;

            print(lastHit.name);
        }


        Debug.DrawRay(transform.position, transform.TransformDirection(transform.forward) * 400, Color.green);
    }
}

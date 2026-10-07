using UnityEngine;

public class RaycastDetect : MonoBehaviour
{

    public LayerMask mask;

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out var hit, Mathf.Infinity, mask))
        {
            var obj = hit.collider.gameObject;

            print(obj.name);
        }
    }
}

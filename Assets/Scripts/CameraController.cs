using UnityEngine;
using System.Collections.Generic;


public class CameraController : MonoBehaviour
{
     
    [Range(0, 0.5f)]
    [SerializeField] private float speed = 0.1f;

    // [SerializeField] private Camera centerCam;
    // [SerializeField] private Camera leftCam;
    // [SerializeField] private Camera rightCam;
    
    [SerializeField] private GameObject ccHub;


    // Update is called once per frame
    void Update()
    {
        float verticalPos = Input.GetAxis("Vertical") * speed;
        float horizontalPos = Input.GetAxis("Horizontal") * speed;
        
        // if (vert != 0 || hor != 0)
        // {
        //     print($"Vertical: {vert} \nHorizontal: {hor}");
        // }


        ccHub.transform.RotateAround(ccHub.transform.position, 
                                            ccHub.transform.right, -verticalPos);
        //Left right
        ccHub.transform.RotateAround(ccHub.transform.position, 
                                             ccHub.transform.up, horizontalPos);

        return;
    }
}

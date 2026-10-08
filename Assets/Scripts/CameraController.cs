using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


public class CameraController : MonoBehaviour
{
     
    [Range(0, 30)]
    [SerializeField] private float speed = 10.0f;

    // [SerializeField] private Camera centerCam;
    // [SerializeField] private Camera leftCam;
    // [SerializeField] private Camera rightCam;
    
    [SerializeField] private GameObject ccHub;


    // Update is called once per frame
    void Update()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return;

        // Left DPAD joystick is for pitch and yaw
        float horizontalPos = gamepad.rightStick.ReadValue().x; // = Input.GetAxis("Horizontal2");
        float verticalPos = gamepad.rightStick.ReadValue().y; // = Input.GetAxis("Vertical2");

        /*float verticalPos = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float horizontalPos = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        */

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

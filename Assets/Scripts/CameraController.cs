using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0, 0.5f)]
    [SerializeField] private float speed = 0.1f;


    // Update is called once per frame
    void Update()
    {
        float verticalPos = Input.GetAxis("Vertical") * speed;
        float horizontalPos = Input.GetAxis("Horizontal") * speed;
        
        // if (vert != 0 || hor != 0)
        // {
        //     print($"Vertical: {vert} \nHorizontal: {hor}");
        // }

        //Up  down
        Camera.main.transform.RotateAround(Camera.main.transform.position, 
                                            Camera.main.transform.right, -verticalPos);
        //Left right
        Camera.main.transform.RotateAround(Camera.main.transform.position, 
                                             Camera.main.transform.up, horizontalPos); 

        return;
    }
}

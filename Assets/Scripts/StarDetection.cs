using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class StarDetection : MonoBehaviour
{
    [SerializeField]
    private double rightAscension;

    [SerializeField]
    private double Declination;

    [SerializeField]
    private bool Check = false;

    private void Update()
    {
        if (Check == true)
        { 
            double ra_radians = math.radians(rightAscension);
            double dec_radians = math.radians(Declination);

            double x = System.Math.Cos(ra_radians);
            double y = System.Math.Sin(dec_radians);
            double z = System.Math.Sin(ra_radians);

            double y_cos = System.Math.Cos(dec_radians);
            x *= y_cos;
            z *= y_cos;

            print($"{x}, {y}, {z}");

            Vector3 newPos = new((float)x, (float)y, (float)z);
            transform.position = newPos * 400;

            Check = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
    }
}

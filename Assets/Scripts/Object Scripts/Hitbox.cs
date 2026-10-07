using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string starName = "HR 1903";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject starGO = GameObject.Find(starName);
        transform.position = starGO.transform.position;

        //transform.LookAt()
    }

}

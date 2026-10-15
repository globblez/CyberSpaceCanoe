using UnityEngine;
using TMPro;

public class StarUI : MonoBehaviour
{
    public string starName = "Star";
    public int lightyears = 0;

    float scaleMulti = 5.0f;
    Vector3 positioning = new (0, -10, 0);
    TextMeshProUGUI textBox;

    Camera centerCamera;

    private void Awake()
    {
        centerCamera = GameObject.Find("CenterCamera").GetComponent<Camera>();
    }

    private void Start()
    {
        transform.localScale = transform.localScale * scaleMulti;
        transform.position += positioning;
        textBox = GetComponentInChildren<TextMeshProUGUI>();

        SetTextBox();
        
        transform.rotation = centerCamera.transform.rotation;
        transform.LookAt(centerCamera.transform, transform.up);
        transform.Rotate(0, 180, 0);
    }

    public void SetNameLightyears(string name, int lys)
    {
        starName = name;
        lightyears = lys;

        SetTextBox();
    }

    void SetTextBox()
    {
        textBox.text = $"{starName}\n~{lightyears} ly";
    }


}

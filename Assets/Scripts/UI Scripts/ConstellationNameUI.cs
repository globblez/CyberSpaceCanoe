using UnityEngine;
using TMPro;

public class ConstellationNameUI : MonoBehaviour
{
    [SerializeField] public string starName = "Orion";

    [Space(20)]


    [SerializeField] float scaleMultiplier = 0.1f;
    [SerializeField] GameObject settingPos;
    [SerializeField] Camera centerCam;

    RectTransform rectTransform;
    Vector3 initialScale;
    TextMeshProUGUI textUI;

    public float distance;

    private void Awake()
    {
        transform.parent.position = Vector3.zero;
    }


    private void Start()
    {
        
        transform.position = settingPos.transform.position;

        initialScale = transform.localScale;

        //Change UI to star name
        textUI = GetComponentInChildren<TextMeshProUGUI>();
        textUI.text = starName;

    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = centerCam.transform.rotation;
        transform.LookAt(centerCam.transform, transform.up);
        transform.Rotate(0, 180, 0);

        distance = Vector3.Distance(centerCam.transform.position, transform.position);
        //print(distance);

        transform.localScale = initialScale * (distance / 100) * scaleMultiplier;
        //print(transform.localScale);


        if (transform.position != settingPos.transform.position)
        {
            transform.position = settingPos.transform.position;
            Debug.Log(transform.position);
        }
    }
}

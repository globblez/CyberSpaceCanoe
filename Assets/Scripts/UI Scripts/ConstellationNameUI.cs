using UnityEngine;
using TMPro;
using System.Collections;

public class ConstellationNameUI : MonoBehaviour
{
    [SerializeField] public string starName = "Orion";
    [SerializeField] private int index;

    [Space(20)]


    [SerializeField] float scaleMultiplier = 0.1f;
    [SerializeField] GameObject settingPos;
    [SerializeField] Camera centerCam;

    [SerializeField] GameObject prefab;

    private RectTransform rectTransform;
    private Vector3 initialScale;
    private TextMeshProUGUI textUI;

   
    public float distance;

    public bool activate = false;

    private bool onOffSwitch = false;
    private bool cooldown = false;

    private StarField starField;
    private int[] starArray;

    private void Awake()
    {
        transform.parent.position = Vector3.zero;
        activate = false;
    }


    private void Start()
    {
        
        transform.position = settingPos.transform.position;

        initialScale = transform.localScale;

        //Change UI to star name
        textUI = GetComponentInChildren<TextMeshProUGUI>();
        textUI.text = starName;

        starField = GameObject.Find("CameraHolder").GetComponent<StarField>();
        starArray = starField.constellations[index].Item1;
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

        //Checks if player has selected this constellation...
        if (activate)
        {
            activate = false;
            ConstellationSelected();
        }
    }

    public void SetActive()
    {
        if (cooldown) return;

        if (activate == false) activate = true;
        StartCoroutine(Cooldown());
    }

    public void ConstellationSelected()
    {
        //Do something here
        if (onOffSwitch == false)
        {
            TurnOn();
        }
        else if (onOffSwitch == true)
        {
            TurnOff();
        }

        onOffSwitch = !onOffSwitch;
    }

    IEnumerator Cooldown()
    {
        cooldown = true;

        yield return new WaitForSeconds(3.0f);

        cooldown = false;
    }

    void TurnOn()
    {

        foreach (int starNum in starArray)
        {
            GameObject starGO = GameObject.Find($"HR {starNum}");

            Vector3 spawnPos = starGO.transform.position;
            Quaternion spawnRot = Quaternion.identity;

            GameObject newGameObject = Instantiate(prefab, spawnPos, spawnRot);
            newGameObject.transform.parent = starGO.transform;

            StarUI starUI = newGameObject.GetComponentInChildren<StarUI>();
            starUI.starName = $"{starNum}";
        }

        print($"You have selected {starName}");
    }

    void TurnOff()
    {

        foreach (int starNum in starArray)
        {
            GameObject starGO = GameObject.Find($"HR {starNum}");
            Destroy(starGO.transform.GetChild(0).gameObject);
        }
    }
}

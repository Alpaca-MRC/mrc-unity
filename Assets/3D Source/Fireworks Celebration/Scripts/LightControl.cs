using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightControl : MonoBehaviour
{
    public bool enableLight=true;
    public bool enableHaze = true;
    public float lightRange=300f;
    public float lightIntensity=1f;
    [Range(0, 255)]
    public float hazeLevel = 120;

    [HideInInspector]
    public Light particleLight;
    [HideInInspector]
    public GameObject Haze;

    private ParticleSystem.MainModule partcleMain;
    private ParticleSystem.LightsModule lightPart;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    private void OnEnable()
    {
        partcleMain = Haze.GetComponent<ParticleSystem>().main;
        lightPart = particleLight.transform.parent.gameObject.GetComponent<ParticleSystem>().lights;
    }
    // Update is called once per frame
    void Update()
    {
        Haze.SetActive(enableHaze);
        lightPart.enabled = enableLight;

        particleLight.range = lightRange;
        particleLight.intensity = lightIntensity;

        
        Color tmpColor = partcleMain.startColor.color;
        tmpColor.a = hazeLevel/255f;
        partcleMain.startColor = tmpColor;
    }
}

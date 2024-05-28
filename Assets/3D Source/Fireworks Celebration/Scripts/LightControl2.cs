using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightControl2 : MonoBehaviour
{
    public bool enableLight=true;
    public bool enableHaze = true;
    public float lightRange=300f;
    public float lightIntensity=1f;
    [Range(0, 255)]
    public float hazeLevel = 200;

    [HideInInspector]
    public Light particleLight;

    private ParticleSystem.TrailModule partcleTrail;
    private ParticleSystem.LightsModule lightPart;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    private void OnEnable()
    {
        partcleTrail = gameObject.GetComponent<ParticleSystem>().trails;
        lightPart = particleLight.transform.parent.gameObject.GetComponent<ParticleSystem>().lights;
    }
    // Update is called once per frame
    void Update()
    {
        partcleTrail.enabled = enableHaze;
        lightPart.enabled = enableLight;

        particleLight.range = lightRange;
        particleLight.intensity = lightIntensity;


        Color tmpColor = partcleTrail.colorOverLifetime.color;
        tmpColor.a = hazeLevel/255f;
        partcleTrail.colorOverLifetime = tmpColor;
    }
}

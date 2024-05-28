using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNextPart : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject objNext;
    public float delaySecond;

    void Start()
    {
        Invoke(nameof(StartNextPart), delaySecond);
    }

    void StartNextPart()
    {
        objNext.SetActive(true);
    }
}

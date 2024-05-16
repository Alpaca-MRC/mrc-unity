using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameStatus : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("으악 Collision으로 맞음!: " + collision.collider.name);
    }

   void OnTriggerEnter(Collider collision) 
   {
        Debug.Log("으악 Trigger로 맞음!: " + collision.name);
   }
}

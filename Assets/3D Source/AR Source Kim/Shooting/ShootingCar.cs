using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingCar : MonoBehaviour
{
    [Header("Fire rate")]
    private int Prefab;
    [Range(0.0f, 1.0f)]
    public float fireRate = 0.1f;
    private float fireCountdown = 0f;
    public GameObject FirePoint;
    public InputActionAsset inputActionsAsset;
    //How far you can point raycast for projectiles
    public float MaxLength;
    public GameObject[] Prefabs;
    public GameObject myCar;
    void Start(){}

    void Update()
    {
        // RC카가 바라보는 방향을 총의 방향으로 설정
        FirePoint.transform.rotation = myCar.transform.rotation;

        // 단발
        // if (Input.GetButtonDown("Fire1"))
        if (inputActionsAsset.actionMaps[5].actions[3].ReadValue<float>() > 0.3f)
        {
            Instantiate(Prefabs[Prefab], FirePoint.transform.position, FirePoint.transform.rotation);
        }

        // 연사
        if (Input.GetMouseButton(1) && fireCountdown <= 0f)
        {
            Instantiate(Prefabs[Prefab], FirePoint.transform.position, FirePoint.transform.rotation);
            fireCountdown = 0;
            fireCountdown += fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }
}

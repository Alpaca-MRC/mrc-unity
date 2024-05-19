using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using TMPro;

public class FriendlyShootingCar : MonoBehaviour
{
    [Header("Fire rate")]
    private int Prefab;
    // 연사 속도
    private float fireRate = 0.05f;
    private float fireCountdown = 0f;
    public GameObject FirePoint;
    public InputActionAsset inputActionsAsset;
    // raycast 최대 거리
    public float MaxLength;
    // 최대 탄약 수
    public float maxMagazine;
    // 남은 탄약 수
    public float curMagazine;
    public GameObject[] Prefabs;
    public GameObject myCar;
    public TextMeshProUGUI curMagTxt;

    void Start(){
        maxMagazine = 200f;
        curMagazine = maxMagazine;
    }

    void Update()
    {
        // 잔여 탄약 표시
        curMagTxt.text = curMagazine.ToString();

        // 남아있는 탄약이 없으면 총을 쏘지 않음
        if (curMagazine == 0)
        {
            return;
        }
        // 예외처리 : 잔량이 0 미만으로 떨어졌다면 0으로 설정함
        else if (curMagazine < 0)
        {
            curMagazine = 0;
            return;
        }

        // RC카가 바라보는 방향을 총의 방향으로 설정
        FirePoint.transform.rotation = myCar.transform.rotation;

        // 사격
        // if (Input.GetMouseButton(1) && fireCountdown <= 0f)
        if (inputActionsAsset.actionMaps[5].actions[3].ReadValue<float>() > 0.3f && fireCountdown <= 0f)
        {
            Instantiate(Prefabs[Prefab], FirePoint.transform.position, FirePoint.transform.rotation);
            curMagazine -= 1;
            fireCountdown = 0;
            fireCountdown += fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    public void SetCurrentMagazineToFull() {
        curMagazine = maxMagazine;
    }
}

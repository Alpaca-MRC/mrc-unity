using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Body.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// 플러그인을 유니티에서 적용하기 위한 클래스
public class AndroidWrapper : MonoBehaviour
{

    // MinewBeacon 플러그인 이름
    private static readonly string pluginName = "minewBeaconAdmin";

    // Android Java 객체
    private static readonly AndroidJavaObject plugin = new(pluginName);

    // 비콘 스캔 시작
    public void StartBeaconScan()
    {
        plugin.Call("startScan");
        Debug.Log("비콘 스캔 시작");
    }


    // 비콘 스캔 중단
    public void StopBeaconScan()
    {
        plugin.Call("stopScan");
        Debug.Log("비콘 스캔 중단");

    }

    // 비콘 데이터 업데이트 콜백
    public void OnRangeBeacons(List<BeaconManager> beacons)
    {
        // 비콘 데이터 처리
        foreach (BeaconManager beacon in beacons)
        {
            // 비콘 데이터 출력
            Debug.Log("UUID: " + beacon.uuid);
            Debug.Log("Major: " + beacon.major);
            Debug.Log("Minor: " + beacon.minor);
            // 추가적인 데이터 처리 및 로직 수행
        }
    }

    void Start()
    {
        StartBeaconScan();
    }

    void Update()
    {
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProfileAPI : MonoBehaviour
{
    private readonly string profileEndpoint = "profile/cart";
    private readonly string recordEndpoint = "record";
    public APIManager apiManager;

    // 카트 변경 요청 데이터
    [Serializable]
    public class ChangeCartData
    {
        public string cartName;
    }

    [Serializable]
    public class ChangeCartResponse
    {
        public int status;
        public string code;
        public string message;
        public Data data;
    }

    [Serializable]
    public class Data
    {

    }


    // 카트 변경
    public void ChangeCart(string _cartName)
    {
        // 요청 DTO 생성
        ChangeCartData data = new()
        {
            cartName = _cartName
        };

        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(apiManager.PostRequest(profileEndpoint, jsonData, (response) =>
        {   
            ChangeCartResponse responseData = JsonUtility.FromJson<ChangeCartResponse>(response);
            Debug.Log(responseData.message);
            Debug.Log(responseData.status);
            Debug.Log("카트 변경 성공");
        }));
    }

    // 전적 조회
    public void GetRecord()
    {
        StartCoroutine(apiManager.GetRequest(recordEndpoint, (response) =>
        {
            Debug.Log("전적 조회 성공");
        }));
    }
}

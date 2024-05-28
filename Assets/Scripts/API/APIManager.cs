using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private readonly string baseUrl = "http://localhost:8080/api/";


    // GET 요청 메서드
    public IEnumerator GetRequest(string endpoint, Action<string> callback)
    {
        using UnityWebRequest request = UnityWebRequest.Get(baseUrl + endpoint);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("GET 요청 에러 : " + request.error);
        }
        else
        {
            callback(request.downloadHandler.text);
        }
    }

    // POST 요청 메서드
    public IEnumerator PostRequest(string endpoint, string jsonData, Action<string> callback)
    {
        string url = baseUrl + endpoint;
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

        Debug.Log(url);

        // HTTP 요청 객체 생성
        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonBytes),
            downloadHandler = (DownloadHandler)new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 응답 확인
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("POST 요청 에러 : " + request.error);
            Debug.LogError("GET 요청 실패 상태 코드 : " + request.responseCode);
            Debug.LogError("GET 요청 실패 내부 메시지 : " + request.downloadHandler.text);
        }
        else
        {
            callback(request.downloadHandler.text);
        }
    }
}

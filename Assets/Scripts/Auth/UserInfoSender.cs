using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum AccountType
{
    GooglePlayGames,
    Oculus
}

public class UserInfoSender : MonoBehaviour
{
    public string serverUrl = "http://localhost:8080/api/member/auth"; // 서버 URL

    [Serializable]
    public class AuthRequestData
    {
        public string userName;
        public string nickname;
        public AccountType accountType;
    }

    [Serializable]
    public class AuthResponseData
    {
        public int status;
        public string code;
        public string message;
        public Data data;
    }

    [Serializable]
    public class Data
    {
        public string accessToken;
        public string refreshToken;
        public long expiresIn;
    }

    // 사용자 정보를 서버로 전송하는 함수
    public void SendUserInfoToServer(string _userName, string _nickname, AccountType _accountType)
    {
        // 사용자 정보를 UserInfo 객체에 저장
        AuthRequestData authRequestData = new()
        {
            userName = _userName,
            nickname = _nickname,
            accountType = _accountType
        };

        // UserInfo 객체를 JSON 형식으로 직렬화
        string authRequestJson = JsonUtility.ToJson(authRequestData);

        // 서버에 HTTP POST 요청을 보냅니다.
        StartCoroutine(SendRequestToServer(authRequestJson));
    }

    // 서버에 HTTP POST 요청을 보내는 코루틴 함수
    IEnumerator SendRequestToServer(string authRequestJson)
    {
        // 요청을 보낼 URL
        string url = serverUrl;

        Debug.Log("요청 URL: " + url);
        Debug.Log("데이터: " + authRequestJson);

        // JSON 형식의 사용자 정보를 바이트 배열로 변환합니다.
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(authRequestJson);

        // HTTP 요청 객체 생성
        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw),
            downloadHandler = (DownloadHandler)new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 응답 확인
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("서버에 요청을 보낼 수 없음: " + request.error);
        }
        else
        {
            Debug.Log("서버에 요청을 성공적으로 보냄");
            Debug.Log("응답: " + request.downloadHandler.text);

            // 서버 응답을 AuthResponseData 객체로 파싱
            AuthResponseData responseData = JsonUtility.FromJson<AuthResponseData>(request.downloadHandler.text);

            // 추출한 데이터를 저장

            string accessToken = responseData.data.accessToken;
            string refreshToken = responseData.data.refreshToken;
            long expiresIn = responseData.data.expiresIn;

            Debug.Log("accessToken : " + accessToken);
            Debug.Log("refreshToken : " + refreshToken);
            Debug.Log("expiresIn : " + expiresIn);

            // AccessToken을 SecurePlayerPrefs에 저장
            // SecurePlayerPrefs.SetString("AccessToken", accessToken);

            // 추출한 데이터를 필요한 곳에 저장
            SaveAuthData(accessToken, refreshToken, expiresIn);

        }
    }

    // 추출한 데이터를 필요한 곳에 저장하는 함수
    void SaveAuthData(string accessToken, string refreshToken, long expiresIn)
    {
        // TODO: 추출한 데이터를 필요한 곳에 저장하는 로직을 구현
        // 예시: PlayerPrefs를 사용하여 데이터를 저장하는 경우
        PlayerPrefs.SetString("AccessToken", accessToken);
        PlayerPrefs.SetString("RefreshToken", refreshToken);
        PlayerPrefs.SetFloat("ExpiresIn", expiresIn);
    }
}

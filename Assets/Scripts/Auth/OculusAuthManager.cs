using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Oculus.Platform;
// using Oculus.Platform.Models;

public class OculusAuthManager : MonoBehaviour
{
    private string userName; // 사용자의 ID를 저장하는 변수
    private string nickname; // 사용자의 닉네임을 저장하는 변수;
    // private GameObject UserInfoSender;

    // Unity Awake 함수에서 초기화
    private void Awake()
    {
        Debug.Log("오큘러스 인증 매니저 시작!!");
    }

    // // Oculus 로그인
    // public void OculusAuth()
    // {
    //     // Oculus 플랫폼 비동기 초기화
    //     Debug.Log("오큘러스 인증 초기화 시작!!");
    //     Core.AsyncInitialize().OnComplete(OnInitializationCallback);
    // }

    // // Oculus 플랫폼 초기화 콜백 함수
    // private void OnInitializationCallback(Message<PlatformInitialize> msg)
    // {
    //     if (msg.IsError)
    //     {
    //         // 초기화 중 오류가 발생한 경우 오류 메시지 출력
    //         Debug.LogErrorFormat("오류 발생: {0}", msg.GetError().Message);

    //     }
    //     else
    //     {
    //         // 초기화 중 오류가 발생한 경우 오류 메시지 출력
    //         Entitlements.IsUserEntitledToApplication().OnComplete(OnIsEntitledCallback);
    //     }
    // }

    // // 사용자 권한 확인 콜백 함수
    // private void OnIsEntitledCallback(Message msg)
    // {
    //     if (msg.IsError)
    //     {
    //         // 사용자가 애플리케이션에 권한이 없는 경우 오류 메시지 출력
    //         Debug.LogErrorFormat("Oculus: 사용자의 애플리케이션 권한 확인 중 오류 발생. 오류 메시지: {0}", msg.GetError().Message);
    //     }
    //     else
    //     {
    //         Debug.Log("오큘러스 인증 성공!!");
            
    //         // 로그인된 사용자 정보 가져오기
    //         GetLoggedInUser();
    //     }
    // }

    // // 로그인된 사용자 정보 가져오기
    // private void GetLoggedInUser()
    // {
    //     Debug.Log("오큘러스 사용자 정보 가져오기!!");
    //     Users.GetLoggedInUser().OnComplete(OnLoggedInUserCallback);
    // }

    // // 로그인된 사용자 정보 콜백 함수
    // private void OnLoggedInUserCallback(Message<User> msg)
    // {
    //     if (msg.IsError)
    //     {
    //         // 로그인된 사용자 정보 가져오기 실패 시 오류 메시지 출력
    //         Debug.LogErrorFormat("Oculus: 로그인된 사용자 정보 가져오기 실패. 오류 메시지: {0}", msg.GetError().Message);
    //     }
    //     else
    //     {
    //         // 사용자 ID 저장 후 사용자 증명 가져오기
    //         userName = msg.Data.ID.ToString(); // msg.Data.OculusID 사용하지 않기
    //         String oculusId = msg.Data.OculusID;
    //         nickname = msg.Data.DisplayName;

    //         Debug.Log("userId : " + userName);
    //         // Debug.Log("oculusId : " + oculusId);
    //         Debug.Log("nickname : " + nickname);

    //         // 사용자 정보를 서버로 전송
    //         UserInfoSender userInfoSender = FindObjectOfType<UserInfoSender>();
    //         userInfoSender.SendUserInfoToServer(userName, nickname, AccountType.Oculus);

    //         if(userInfoSender != null)
    //         {
    //             // UserInfoSender 컴포넌트가 존재하는 경우
    //             userInfoSender.SendUserInfoToServer(userName, nickname, AccountType.Oculus);
    //         }
    //         else
    //         {
    //             // UserInfoSender 컴포넌트가 존재하지 않는 경우
    //             Debug.LogError("UserInfoSender 컴포넌트를 찾을 수 없습니다.");
    //         }
    //         Debug.Log("dfg");
    //     }
    // }

    // 
}

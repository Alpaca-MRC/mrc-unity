using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;
using System;
using System.Collections;
using UnityEngine.Networking;

public class GoogleAuthManager : MonoBehaviour
{
    // 구글 로그인 성공 시 UI에 표시할 텍스트를 참조하는 변수
    [SerializeField] private Text longText1;
    [SerializeField] private Text longText2;

    // 구글 플레이 게임 서비스 로그인을 시도하는 함수
    public void GPGSLogin()
    {
        // PlayGamesPlatform을 사용하여 구글 플레이 게임 서비스에 로그인을 시도합니다.
        // 로그인 결과는 ProcessAuthentication 콜백 함수에 전달됩니다.
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    // 구글 플레이 게임 서비스 로그인 결과를 처리하는 콜백 함수
    internal void ProcessAuthentication(SignInStatus status)
    {
        // 로그인이 성공했을 때
        if (status == SignInStatus.Success)
        {
            // 사용자의 닉네임과 아이디를 가져옵니다.
            string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
            string userName = PlayGamesPlatform.Instance.GetUserId();


            PlayGamesPlatform.Instance.RequestServerSideAccess(
                false,  // forceRefreshToken (선택적, 기본값은 false)
                code =>
                {
                    // 서버 인증 코드 처리
                    Debug.Log("서버 인증 코드: " + code);

                    // 코드를 서버로 전송 (예: 네트워킹 라이브러리 사용)
                    // ... (서버로 코드 전송 코드)
                });


            // UI에 로그인 성공 메시지와 사용자 정보를 표시합니다.
            longText1.text = "로그인 성공: " + displayName + " / " + userName;
            longText2.text = "사용자 아이디: " + userName;

            // 사용자 정보를 서버로 전송합니다.
            UserInfoSender userInfoSender = GetComponent<UserInfoSender>();
            if(userInfoSender != null)
            {
                userInfoSender.SendUserInfoToServer(userName, displayName, AccountType.GooglePlayGames);
            }
        }
        // 로그인이 실패했을 때
        else
        {
            // UI에 로그인 실패 메시지를 표시합니다.
            longText1.text = "로그인 실패";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationManager : MonoBehaviour
{
    // [SerializeField] private GoogleAuthManager googleAuthManager;
    // [SerializeField] private OculusAuthManager oculusAuthManager;

    [SerializeField] private Button googleButton;
    [SerializeField] private Button oculusButton;

    private void Awake()
    {
        // 플랫폼 탐지
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                // 모바일 앱을 통해 접속한 경우
                // GoogleAuthManager 활성화
                Debug.Log("모바일 접속, 구글 매니저 활성화");
                googleButton.enabled = true;
                oculusButton.enabled = false;
                break;
            default:
                // 기타 플랫폼(예: PC, Oculus 등)
                // OculusAuthManager 활성화
                Debug.Log("오큘러스 접속, 오큘러스 매니저 활성화");
                googleButton.enabled = false;
                oculusButton.enabled = true;
                break;
        }
    }
}

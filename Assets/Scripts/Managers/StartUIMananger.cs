using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    SceneChanger sceneChanger;
    public GameObject startUI, connectingUI, SucessConnectingUI, FailedConnectingUI;
    public ButtonManager buttonManager;

    private bool checkSuccess;

    void Start()
    {
        startUI.SetActive(true);
        connectingUI.SetActive(false);
        SucessConnectingUI.SetActive(false);
        FailedConnectingUI.SetActive(false);
        sceneChanger = FindAnyObjectByType<SceneChanger>();
        buttonManager = FindAnyObjectByType<ButtonManager>();
    }

    void Update()
    {
        if(checkSuccess)
        {
            StartCoroutine(OnTaskCompleted(SucessConnectingUI, 3.0f));
        }

    }

    public void OnClickConnectBtn()
    {
        StartCoroutine(OnTaskCompleted(connectingUI, 3.0f));
        try
        {
            NetworkManager.Instance.ConnectToControlUDPServer();
            NetworkManager.Instance.ConnectToCameraUDPServer();
        }
        catch (Exception e)
        {
            connectingUI.SetActive(false);
            StartCoroutine(OnTaskCompleted(FailedConnectingUI, 3.0f));  
            Debug.Log("TCP connection error2: " + e);
        }
    }

    public void OnClickStart() {
        // 연결이 성공했다면 메인 씬으로 전환시키기
        sceneChanger.GoMainScene();
    }
    
    public void OnClickOption() {
        Debug.Log("옵션");
    }

    private IEnumerator OnTaskCompleted(GameObject uiElement, float displayTime)
    {
        // UI를 활성화
        uiElement.SetActive(true);
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(displayTime);
        // UI 비활성화
        uiElement.SetActive(false);

        if (uiElement == SucessConnectingUI)
        {
            checkSuccess = false;
            buttonManager.EnableButton(); // 시작 버튼 활성화
        } else if (uiElement == connectingUI)
        {
            checkSuccess = true;
        }
    }
}

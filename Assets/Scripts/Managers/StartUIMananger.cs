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
    public GameObject startButton, spinner, completeUI;

    private bool isStartButtonClicked = false;

    void Start()
    {
        sceneChanger = FindAnyObjectByType<SceneChanger>();
        startButton.SetActive(true);
        spinner.SetActive(false);
        completeUI.SetActive(false);
    }

    void Update()
    {

    }

    public void OnClickConnectBtn()
    {
        StartCoroutine(ChangeSceneAfterTasks());
    }
    private IEnumerator ChangeSceneAfterTasks()
{
    yield return StartCoroutine(OnTaskCompleted(startButton, 1.0f));
    yield return StartCoroutine(OnTaskCompleted(spinner, 3.0f));
    yield return StartCoroutine(OnTaskCompleted(completeUI, 1.0f));
    sceneChanger.GoMainScene();
}

    private IEnumerator OnTaskCompleted(GameObject uiElement, float displayTime)
    {
        // UI를 활성화
        uiElement.SetActive(true);
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(displayTime);
        // UI 비활성화
        uiElement.SetActive(false);
    }
}

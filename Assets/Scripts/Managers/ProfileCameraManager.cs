using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ProfileCameraManager : MonoBehaviour
{

    // 카메라 오브젝트를 참조하기 위한 변수
    public XROrigin xrOrigin;

    public GameObject myProfileUI;
    public GameObject cartUI;
    // public GameObject characterUI;
    // public GameObject recordUI;

    void Start()
    {
        ActivateUI(myProfileUI);
    }

    public void OnCartButtonClicked()
    {
        Debug.Log("cart 버튼 클릭!!");
        SetCameraPositionAndRotation(new Vector3(-6f, 2.5f, -175.3f), Quaternion.Euler(0f, -90f, 0f));
        ActivateUI(cartUI);
    }

    public void OnRecordButtonClicked()
    {
        Debug.Log("record 버튼 클릭!!");
        // ActivateUI(recordUI);
    }

    // 카메라의 위치와 회전을 설정하는 메서드
    private void SetCameraPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        if (xrOrigin != null)
        {
            Transform xrRigTransform = xrOrigin.transform;

            xrRigTransform.SetPositionAndRotation(position, rotation);
        }
        else
        {
            Debug.LogError("XROrigin을 찾을 수 없습니다.");
        }
    }

    // UI 활성화 및 비활성화 메서드
    private void ActivateUI(GameObject uiObject)
    {
        // 모든 UI를 비활성화
        myProfileUI.SetActive(false);
        cartUI.SetActive(false);
        // recordUI.SetActive(false);

        // 전달받은 UI를 활성화
        uiObject.SetActive(true);
    }
}

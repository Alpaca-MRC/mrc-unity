using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EnemyMoveCar : MonoBehaviour
{
    public InputActionAsset inputActionsAsset;
    private float maxSpeed;
    private float acceleration; // 초당 가속도
    private float currentSpeed; // 현재 속도
    private float turnSpeed;
    private float horizontalInput;
    
    private float isAPressed;
    private float isBPressed;
    [SerializeField]
    // 0 --> 메인페이지
    // 1 --> MR 인게임
    private int gameMode;
    private Vector3 initialPosition;
    private Quaternion initialRotation;


    void Start() {
        // 초기 위치와 회전 상태를 저장
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        switch (gameMode) {
            // 메인페이지
            case 0:
                maxSpeed = 15.0f;
                acceleration = 5.0f;
                currentSpeed = 0.0f;
                turnSpeed = 40f;
                break;
            // MR 인게임
            case 1:
                maxSpeed = 4.0f;
                acceleration = 1.5f;
                currentSpeed = 0.0f;
                turnSpeed = 80f;
                break;
            // 실수로 gamemode 입력하지 않았을때는 MR을 기준으로 함
            default:
                maxSpeed = 2.0f;
                acceleration = 0.7f;
                currentSpeed = 0.0f;
                turnSpeed = 60f;
                break;
        }
    }

    void Update()
    {
        isAPressed = inputActionsAsset.actionMaps[10].actions[0].ReadValue<float>();
        isBPressed = inputActionsAsset.actionMaps[10].actions[1].ReadValue<float>();

        horizontalInput = Input.GetAxis("Horizontal"); 
        // 전진 또는 후진 버튼이 눌렀을 경우
        if (isAPressed == 1 || isBPressed == 1) {
            // 가속
            if (isAPressed == 1 && currentSpeed < maxSpeed) {
                if (currentSpeed < 0) {
                    currentSpeed += acceleration * Time.deltaTime * 5; // 더 빠른 가속
                } else {
                    currentSpeed += acceleration * Time.deltaTime;
                }
            }
            
            // 감속 또는 후진
            if (isBPressed == 1) {
                if (currentSpeed > 0) {
                    currentSpeed -= acceleration * Time.deltaTime * 5; // 더 빠른 감속을 위해 가속도의 5배 적용
                } else {
                    currentSpeed -= acceleration * Time.deltaTime; // 후진
                }
            }
        }

        // 눌리지 않았을 경우 (0이 될때까지 가속 / 감속)
        else {
            if (currentSpeed > 0) {
                currentSpeed -= acceleration * Time.deltaTime;
            } else if (currentSpeed < 0) {
                currentSpeed += acceleration * Time.deltaTime;
            }
        }

        RaycastHit hit;

        // 앞이 박는다면
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.4f))
        {
            var arPlane = hit.transform.GetComponent<ARPlane>();
            // 만약 친것이 arPlane의 어떤것이라면
            if (arPlane != null)
            {
                if (arPlane.classification == PlaneClassification.Wall)
                {
                    // 후진은 허용 (앞이 박았으므로)
                    if (currentSpeed < 0)
                    {
                        transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
                    }
                }
            }
        }

        // 뒤가 박는다면
        else if (Physics.Raycast(transform.position, transform.forward * -1f, out hit, 0.4f))
        {
            // 만약 친것이 arPlane의 어떤것이라면
            var arPlane = hit.transform.GetComponent<ARPlane>();
            if (arPlane != null)
            {
                if (arPlane.classification == PlaneClassification.Wall)
                {
                    // 전진은 허용 (뒤가 박았으므로)
                    if (currentSpeed > 0)
                    {
                        transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
                    }
                }
            }
        }
        else 
        {
            // 전진 또는 후진 실행
            transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
        }
        
        // 회전
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other) {
        Debug.Log("Collision으로 "+ other.collider.name +"를 박긴 했네요");
    }
}

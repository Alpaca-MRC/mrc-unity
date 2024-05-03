using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMove : MonoBehaviour
{
    public InputActionAsset inputActionsAsset;

    private float maxSpeed = 15.0f;
    private float acceleration = 5.0f; // 초당 가속도
    private float currentSpeed = 0.0f; // 현재 속도
    private float turnSpeed = 40f;
    private float horizontalInput;
    
    private float isAPressed;
    private float isBPressed;

    void Start() {}

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

        // 전진 또는 후진 실행
        transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);

        // 회전
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);
    }
}

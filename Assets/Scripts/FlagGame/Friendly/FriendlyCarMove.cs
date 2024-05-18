using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FriendlyCarMove : MonoBehaviour
{

    // 게임 상태 관리
    [SerializeField]
    private FlagManager flagManager;     // 플래그 상태 관리
    [SerializeField]
    private HpBarScript hpBarScript;

    // 아군 차량 상태
    private int maxHealth;  // 최대 체력
    private int curHealth;  // 현재 체력
    private bool isStun;  // 스턴 여부 -> 스턴시 update 이동 판단하지 않음

    // 이동 관리
    [SerializeField]
    private InputActionAsset inputActionsAsset;
    private float maxSpeed;
    private float acceleration; // 초당 가속도
    private float currentSpeed; // 현재 속도
    private float turnSpeed;
    private float horizontalInput;
    
    private float isAPressed;
    private float isBPressed;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // 스턴 효과
    [SerializeField]
    private GameObject _stunEffectPrefab;
    private GameObject stunEffect;


    void Start() 
    {
        // 초기 위치와 회전 상태를 저장
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // 속도 제어
        maxSpeed = 4.0f;
        acceleration = 1.5f;
        currentSpeed = 0.0f;
        turnSpeed = 80f;

        // 상태 초기화
        maxHealth = 70;
        curHealth = maxHealth;
        isStun = false;
    }

    void Update()
    {
        if (isStun) {
            return;
        }

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
                // 바닥이랑 박은 것이라면 설정 문제이므로 정상 진행
                else if (arPlane.classification == PlaneClassification.Floor)
                {
                    // 전진 또는 후진 실행
                    transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
                } 
            }
            else 
            {
                // 전진 또는 후진 실행
                transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
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
                // 바닥이랑 박은 것이라면 설정 문제이므로 정상 진행
                else if (arPlane.classification == PlaneClassification.Floor)
                {
                    // 전진 또는 후진 실행
                    transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
                } 
            }
            else
            {
                // 전진 또는 후진 실행
                transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
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

    // 피격시
    void OnCollisionEnter(Collision other) {
        // 충돌한 물체가 적이면서 총알이면
        if (other.gameObject.layer == 7 && other.collider.name.Contains("Bullet")) {
            
            // 이미 스턴 상태라면 피를 깎지 않음
            if (isStun) return;

            // 피를 1 깎는다
            curHealth -= 1;
            float hpPercentage = (float) curHealth / maxHealth;
            hpBarScript.UpdateHealthBar(hpPercentage);
            // 피가 0이 된다면
            if (curHealth <= 0) {
                Exhaustion();
            }
        }
    }

    void Exhaustion()
    {
        // 1초 스턴(테스트 상황에 따라 추가 구현)
        StartCoroutine(Stun(1f));

        // 플래그 떨어뜨리기
        flagManager.Drop();
    }

    // 2. 스턴
    IEnumerator Stun(float time)
    {
        // 스턴 상태 설정
        isStun = true;
        stunEffect = Instantiate(_stunEffectPrefab, gameObject.transform.position, gameObject.transform.rotation);

        // 플레이어가 플래그 소유했을 경우 떨어뜨림
        if (flagManager.flagState == FlagState.OnPlayer)
        {
            flagManager.Drop();
        }

        // 주어진 시간만큼 대기
        yield return new WaitForSeconds(time);

        // 스턴 상태를 해제
        isStun = false;
        Destroy(stunEffect);

        // 체력 다시 채워주기
        curHealth = maxHealth;
        hpBarScript.UpdateHealthBar(1f);
    }
}


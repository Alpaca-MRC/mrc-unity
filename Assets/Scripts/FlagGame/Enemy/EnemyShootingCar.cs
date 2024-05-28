using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class EnemyShootingCar : MonoBehaviour
{
    [Header("Fire rate")]
    private int Prefab;
    // 연사 속도
    private float fireRate = 0.05f;
    private float fireCountdown = 0f;
    public GameObject FirePoint;
    public GameObject[] Prefabs;
    public GameManager gameManager;
    public FlagManager flagManager;

    // 최대 탄약 수
    public float maxMagazine;
    // 남은 탄약 수
    public float curMagazine;
    public TextMeshProUGUI curMagTxt;

    // 플레이어 카트를 바라보는 각도 범위
    public float shootingAngleThreshold = 30f;

    void Start(){
        maxMagazine = 200f;
        curMagazine = maxMagazine;
    }

    void Update()
    {
        // 잔여 탄약 표시
        curMagTxt.text = curMagazine.ToString();
        
        // 남아있는 탄약이 없으면 총을 쏘지 않음
        if (curMagazine == 0)
        {
            gameObject.GetComponent<EnemyController>().isMagazineEmpty = true;
            return;
        }
        // 예외처리 : 잔량이 0 미만으로 떨어졌다면 0으로 설정함
        else if (curMagazine < 0)
        {
            curMagazine = 0;
            return;
        }

        // RC카가 바라보는 방향을 총의 방향으로 설정
        FirePoint.transform.rotation = transform.rotation;

        // 사격
        if (fireCountdown <= 0f && CanShootPlayer() && flagManager.flagState == FlagState.OnPlayer && gameManager.gameState == GameState.InProgress)
        {
            Instantiate(Prefabs[Prefab], FirePoint.transform.position, FirePoint.transform.rotation);
            curMagazine -= 1;
            fireCountdown = 0;
            fireCountdown += fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    // 적 카트가 플레이어 카트를 정면으로 볼 수 있는지 확인하는 메서드
    bool CanShootPlayer()
    {
        // 적 카트가 바라보는 방향
        Vector3 enemyForward = transform.forward;

        // 플레이어 카트를 향하는 벡터
        Vector3 toPlayer = (gameManager.friendlyCart.transform.position - transform.position).normalized;

        // 적 카트와 플레이어 카트 사이의 각도 계산
        float angleToPlayer = Vector3.Angle(enemyForward, toPlayer);

        // 각도가 일정한 임계값 이내인 경우에만 사격 가능
        return angleToPlayer < shootingAngleThreshold;
    }

    // 탄창 아이템 획득 시 재장전
    public void SetCurrentMagazineToFull() {
        curMagazine = maxMagazine;
        gameObject.GetComponent<EnemyController>().isMagazineEmpty = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyShootingCar : MonoBehaviour
{
    [Header("Fire rate")]
    private int Prefab;
    [Range(0.0f, 1.0f)]
    public float fireRate = 0f;
    private float fireCountdown = 0f;
    public GameObject FirePoint;
    public GameObject[] Prefabs;
    public GameManager gameManager;
    public FlagManager flagManager;


    // 플레이어 카트를 바라보는 각도 범위
    public float shootingAngleThreshold = 30f;

    void Update()
    {
        // RC카가 바라보는 방향을 총의 방향으로 설정
        FirePoint.transform.rotation = transform.rotation;

        // 연사
        if (fireCountdown <= 0f && CanShootPlayer() && flagManager.flagState == FlagState.OnPlayer && gameManager.gameState == GameState.InProgress)
        {
            // Debug.Log("슈팅");
            Instantiate(Prefabs[Prefab], FirePoint.transform.position, FirePoint.transform.rotation);
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
}

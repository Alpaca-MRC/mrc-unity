using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlagState
{
    OnBoard,    // 플래그가 보드에 놓여있음
    OnPlayer,   // 플래그가 플레이어 카트에 보유됨
    OnEnemy     // 플래그가 적 카트에 보유됨
}

public class FlagManager : MonoBehaviour
{
    private bool canCollide = true; // 충돌 여부를 제어하는 변수
    public FlagState flagState;     // 플래그 상태

    public Transform playerGoal;    // 플레이어 골대
    public Transform enemyGoal;     // 적 골대
    private float dropDistance;

    public EnemyController enemyController; // 적 카트 컨트롤러
    public GameManager gameManager;         // 게임 매니저

    void Start()
    {
        canCollide = true;
        dropDistance = 10f;
    }
    
    void Update()
    {
        if (canCollide == false)
        {
            canCollide = true;
        }
    }

    // 두 골대 중간에 위치에 플래그 생성
    void GenerateFlag()
    {    
        transform.SetParent(null);
        Vector3 newPosition = (playerGoal.position + enemyGoal.position) / 2;
        newPosition.y = 0f;
        transform.position = newPosition;
    }    

    // 플래그의 접촉 이벤트
    // 카트위에 있을 때 일어나는 접촉의 경우 추가 처리가 필요
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.tag);

        Debug.Log("canCollide : " + canCollide);

        // 충돌이 가능한 상태인지 확인
        if (!canCollide) return;

        // 플레이어 카트와 접촉
        if (other.CompareTag("Player"))
        {
            // 카트의 위치로 플래그 이동
            transform.SetParent(other.transform);
            // 원점이 아니라 카트의 위로 이동
            transform.position = other.transform.position;
            flagState = FlagState.OnPlayer;
        }
        // 적 카트와 접촉
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("Get Flag!!");
            transform.SetParent(other.transform);
            transform.position = other.transform.position;
            flagState = FlagState.OnEnemy;
            Drop();
        }
        // 골대와 접촉
        else if (other.CompareTag("Goal"))
        {
            Debug.Log("골인!!!!!!!!!!!!");

            flagState = FlagState.OnBoard;
            gameManager.IncreaseScore("enemy");

            // 카트에서 떨어지고 위치 초기화
            GenerateFlag();
        } 
        else
        {
            Debug.Log("누구랑 부디혔나? : " + other.tag);
        }

        if(canCollide)
        {
            canCollide = false;
        }
        StartCoroutine(ResetCollision());
            
    }

    private void OnTriggerExit(Collider other) {
        StartCoroutine(ResetCollision());
    }

    // 일정 시간 후 충돌을 다시 활성화
    private IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(0.2f);
        canCollide = true;
    }

    // 카트에 소유되었던 플래그가 떨어지는 메서드
    // 카트 후방으로 일정거리 이상 떨어뜨리기
    public void Drop()
    {
        // 카트에서 플래그를 제거하고 부모를 초기화하여 플래그를 카트에서 분리
        transform.SetParent(null);
        
        // 카트 뒤에 떨어뜨리기
        Vector3 dropPosition = transform.forward * (-1f) - transform.forward * dropDistance;
        dropPosition.y = 0f;
        transform.position = dropPosition;

        flagState = FlagState.OnBoard;

        StartCoroutine(ResetCollision());
    } 
}

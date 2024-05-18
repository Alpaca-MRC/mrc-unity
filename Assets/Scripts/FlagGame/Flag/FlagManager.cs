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
    // 상태관리
    public GameManager gameManager;

    private bool canCollide = true; // 충돌 여부를 제어하는 변수
    public FlagState flagState;     // 플래그 상태

    public float dropDistance;     // 플래그 드랍 거리
    private float dropHeight;       // 플래그 드랍 높이
    public Vector3 spawnPosition;  // 초기 위치


    void Start()
    {
        Initialization();
    }

    void Update()
    {
        if (canCollide == false) canCollide = true;
    }

    // 초기화
    public void Initialization()
    {
        flagState = FlagState.OnBoard;
        canCollide = true;
        dropHeight = 0.5f;
    }

    // 두 골대 중간에 위치에 플래그 생성
    public void RespawnFlag()
    {   
        // flag가 카트에 달려있을 경우 부모 해제
        if (gameObject != null) 
        {
            gameObject.transform.SetParent(null);
        }
        // 초기 위치로 flag 위치 재설정
        gameObject.transform.position = spawnPosition;
    }    

    // 플래그의 접촉 이벤트
    // 카트위에 있을 때 일어나는 접촉의 경우 추가 처리가 필요
    private void OnTriggerEnter(Collider other)
    {   
        // 충돌이 가능한 상태에서만 혀용
        if (!canCollide) return;

        // Flag가 바닥에 있다면 (플레이어와 접촉이 허용됨)
        if (flagState == FlagState.OnBoard) 
        {
            // 플레이어 카트와 접촉
            if (other.CompareTag("Player"))
            {
                // 이미 플레이어 소유면 무시
                if (flagState == FlagState.OnPlayer) return;

                // 플레이어 카트의 자식으로 설정
                transform.SetParent(other.transform);

                // 원점이 아니라 카트의 위로 이동
                transform.position = other.transform.position + new Vector3(0f, 0.05f, 0f);
                flagState = FlagState.OnPlayer;
                canCollide = false;
            }
            // 적 카트와 접촉
            else if (other.CompareTag("Enemy"))
            {
                // 이미 적 소유면 무시
                if (flagState == FlagState.OnEnemy) return;

                // 적 카트의 자식으로 설정
                transform.SetParent(other.transform);

                // 원점이 아니라 카트의 위로 이동
                transform.position = other.transform.position + new Vector3(0f, 0.05f, 0f);
                flagState = FlagState.OnEnemy;
                canCollide = false;
            }
        }
        // Flag가 플레이어 혹은 적에게 있다면 (골대와의 접촉이 허용됨)
        else
        {
            // 골대와 접촉
            if (other.CompareTag("Goal"))
            {
                // 플레이어의 골이면
                // 플레이어 골대와 접촉했을 때만 골로 인정됨 (layer 6번 --> Friendly)
                if (flagState == FlagState.OnPlayer && other.gameObject.layer == 6)
                {
                    Debug.Log("야호 플레이어 골");
                    flagState = FlagState.OnBoard;
                    gameManager.IncreaseScore(0);
                    canCollide = false;
                    // 카트에서 떨어지고 위치 초기화
                    RespawnFlag();
                }

                // 적의 골이면
                // 적 골대와 접촉했을 때만 골로 인정됨 (layer 7번 --> Enemy)
                else if (flagState == FlagState.OnEnemy && other.gameObject.layer == 7)
                {
                    Debug.Log("야호 적 골");
                    flagState = FlagState.OnBoard;
                    gameManager.IncreaseScore(1);
                    canCollide = false;
                    // 카트에서 떨어지고 위치 초기화
                    RespawnFlag();
                }
            } 
        }

        // canCollide True로 설정 (골대와 충돌 가능하도록)
        if(!canCollide) StartCoroutine(ResetCollision());
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
        // 분리하기 전 부모 오브젝트(카트) 가져오기
        Transform cart = transform.parent;

        if (cart == null) return;

        // 카트에서 플래그를 제거하고 부모를 초기화하여 플래그를 카트에서 분리
        transform.SetParent(null);

        // 플래그의 낙하 지점
        Vector3 dropPosition = cart.position - cart.forward * dropDistance;
        dropPosition.y = 0f;

        // 플래그가 낙하하는 애니메이션
        StartCoroutine(DropAnimation(dropPosition, 0.5f));
    } 

    // 플래그가 낙하하는 애니메이션
    IEnumerator DropAnimation(Vector3 dropPosition, float duration)
    {
        float timer = 0f;
        Vector3 initialPosition = transform.position;

        while (timer < duration)
        {
            // 포물선을 그리며 낙하하는 동안의 위치 계산
            float t = timer / duration;
            Vector3 newPosition = Vector3.Lerp(initialPosition, dropPosition, t) + dropHeight * Mathf.Sin(t * Mathf.PI) * Vector3.up;
            // 플래그 위치 설정
            transform.position = newPosition;
            // 시간 업데이트
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = dropPosition;

        flagState = FlagState.OnBoard;
        StartCoroutine(ResetCollision());
    }
}

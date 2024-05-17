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

    private float dropDistance;
    private float dropHeight;

    public EnemyController enemyController; // 적 카트 컨트롤러
    public GameManager gameManager;         // 게임 매니저
    
    public GameObject _flagPrefab;
    public GameObject flag;

    void Start()
    {
        Initialization();
    }
    
    void Update()
    {
        if (canCollide == false)
        {
            canCollide = true;
        }
    }

    void Initialization()
    {
        canCollide = true;
        dropDistance = 15f;
        dropHeight = 5f;

        // 골대 지정
    }

    // 두 골대 중간에 위치에 플래그 생성
    public void GenerateFlag()
    {    
        if (flag != null) flag.transform.SetParent(null);

        Vector3 newPosition = (gameManager.gateOnePosition.position + gameManager.gateTwoPosition.position) / 2;
        newPosition.y = 0f;
        flag = Instantiate(_flagPrefab, newPosition, Quaternion.Euler(0, 0, 0));
    }    

    // 플래그의 접촉 이벤트
    // 카트위에 있을 때 일어나는 접촉의 경우 추가 처리가 필요
    private void OnTriggerEnter(Collider other)
    {        
        // 충돌이 가능한 상태인지 확인
        if (!canCollide) return;

        // 플레이어 카트와 접촉
        if (other.CompareTag("Player"))
        {
            if (flagState == FlagState.OnPlayer) return;

            // 카트의 위치로 플래그 이동
            flag.transform.SetParent(other.transform);
            // 원점이 아니라 카트의 위로 이동
            flag.transform.position = other.transform.position;
            flagState = FlagState.OnPlayer;
            canCollide = false;

        }
        // 적 카트와 접촉
        else if (other.CompareTag("Enemy"))
        {
            if (flagState == FlagState.OnEnemy) return;

            flag.transform.SetParent(other.transform);
            flag.transform.position = other.transform.position;
            flagState = FlagState.OnEnemy;
            canCollide = false;
            // Drop();
        }
        // 골대와 접촉
        else if (other.CompareTag("Goal"))
        {
            if (flagState == FlagState.OnBoard) return;

            Debug.Log("골인!!!!!!!!!!!!");

            flagState = FlagState.OnBoard;
            gameManager.IncreaseScore("enemy");
            canCollide = false;

            // 카트에서 떨어지고 위치 초기화
            GenerateFlag();
        } 
        else
        {
            // Debug.Log("누구랑 부딪혔나? : " + other.tag);
        }

        if(!canCollide)
        {
            StartCoroutine(ResetCollision());
        }
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
        // 분리하기 전 부모 오브젝트(카트) 가져오기
        Transform cart = transform.parent;

        // 카트에서 플래그를 제거하고 부모를 초기화하여 플래그를 카트에서 분리
        flag.transform.SetParent(null);

        Debug.Log("드롭!!!!!");
                
        // 카트 뒤에 떨어뜨리기

        // 플래그의 낙하 지점
        Vector3 dropPosition = cart.position - cart.forward * dropDistance;
        dropPosition.y = 0f;

        // 플래그가 낙하하는 애니메이션
        StartCoroutine(DropAnimation(dropPosition, 0.5f));

        flagState = FlagState.OnBoard;

        StartCoroutine(ResetCollision());
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
            flag.transform.position = newPosition;

            // 시간 업데이트
            timer += Time.deltaTime;
            yield return null;
        }

        // 최종 낙하 지점에 위치 설정
        // transform.position = dropPosition;
    }
}

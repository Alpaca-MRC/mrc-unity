using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public enum MovingState
{
    Stop, Stunned, MovingToFlag, MovingToPlayer, MovingToGoal, CanMove
}

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private FlagManager flagManager;     // 플래그 상태 관리
    [SerializeField]
    private GameManager gameManager;     // 게임 매니저
    [SerializeField]
    private EnemyShootingCar enemyShootingCar;  // 사격 매니저
    [SerializeField]
    private EnemyHpBar enemyHpBar;        // HP 바 관리

    // 카트 상태 관리
    private int maxHealth;  // 최대 체력
    private int curHealth;  // 현재 체력
    private bool isStun;  // 스턴 여부 -> 스턴시 update 이동 판단하지 않음

    // 게임 상태 관리
    public bool isPlayerInRange;        // 플레이어 카트의 사격 범위 안에 있는지 여부
    public bool isShooting;             // 사격 중인지 여부

    // 스턴 효과
    [SerializeField]
    private GameObject _stunEffectPrefab;
    private GameObject stunEffect;

    private void Start()
    {
        // 게임 변수 초기화
        InitializationGameSetting();
    }

    private void Update()
    {
        if (gameManager.gameState == GameState.InProgress && !isStun) ActionCommend();
    }

    // 게임 변수 초기화
    void InitializationGameSetting()
    {
        // 상태 초기화
        maxHealth = 70;
        curHealth = maxHealth;
        isStun = false;
        isPlayerInRange = false;
        isShooting = false;
    }

    // 상태에 따른 액션
    void ActionCommend()
    {
        // 1. 플래그 바닥에 있는 경우
        // 1-1. 플래그를 향해 이동
        if (flagManager.flagState == FlagState.OnBoard) MoveToFlag();

        // 2. 플래그가 플레이어에게 있는 경우
        else if (flagManager.flagState == FlagState.OnPlayer)
        {
            // 플레이어를 향해 이동
            MoveToPlayer();

        }
        // 3. 플래그가 적에게 있는 경우
        // 3-1. 골대를 향해 이동
        else if (flagManager.flagState == FlagState.OnEnemy) MoveToGoal();

    }

    // ################## 카트 체력에 따른 이벤트 ########################

    // 피격시
    void OnCollisionEnter(Collision other) {

        // 충돌한 물체가 아군이면서 총알이면
        if (other.gameObject.layer == 6 && other.collider.name.Contains("Bullet")) {

            // 이미 스턴 상태라면 피를 깎지 않음
            if (isStun) return;

            // 피를 1 깎는다
            curHealth -= 1;
            float hpPercentage = (float) curHealth / maxHealth;
            enemyHpBar.UpdateHealthBar(hpPercentage);
            // 피가 0이 된다면
            if (curHealth == 0) {
                Exhaustion();
            }
        }
    }
    
    // 1. 체력 소진
    void Exhaustion()
    {
        // 1초 스턴(테스트 상황에 따라 추가 구현)
        StartCoroutine(Stun(1f));

        if (flagManager.flagState == FlagState.OnEnemy)
        {
            // 플래그 떨어뜨리기
            flagManager.Drop();
        } 
    }

    // 2. 스턴
    IEnumerator Stun(float time)
    {
        // 스턴 상태 설정
        isStun = true;
        stunEffect = Instantiate(_stunEffectPrefab, gameObject.transform.position, gameObject.transform.rotation);
        
        // 적이 플래그 소유했을 경우 떨어뜨림
        if (flagManager.flagState == FlagState.OnEnemy)
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
        enemyHpBar.UpdateHealthBar(1f);
    }


    // ##################### 이동 액션 ######################

    // 1. 플래그를 향해 이동
    void MoveToFlag()
    {
        // 플래그 방향
        Vector3 flagDirection = (flagManager.gameObject.transform.position - transform.position).normalized;
        flagDirection.y = 0f;

        // 적 카트가 플래그 방향으로 일정한 속도로 이동하도록 설정
        float moveSpeed = 0.5f;
        transform.position += moveSpeed * Time.deltaTime * flagDirection;

        // 적 카트가 플래그를 향해 정면이 보이도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(flagDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // 2. 플레이어를 향해 이동
    void MoveToPlayer()
    {
        // 플레이어 방향 벡터 계산
        Vector3 playerDirection = (gameManager.friendlyCart.transform.position - transform.position).normalized;
        playerDirection.y = 0f;

        // 적 카트가 플레이어 방향으로 일정한 속도로 이동하도록 설정
        float moveSpeed = 0.5f;
        transform.position += moveSpeed * Time.deltaTime * playerDirection;

        // 적 카트가 플레이어를 향해 정면이 보이도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    
        // 회전이 완료되면 사격 준비 On
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            isPlayerInRange = true;
        }
    }

    // 3. 골대를 향해 이동
    void MoveToGoal()
    {
        // 골대 방향 벡터 계산
        Vector3 goalDirection = (gameManager.gateTwoPosition.position - transform.position).normalized;
        goalDirection.y = 0f;

        // 적 카트가 골대 방향으로 일정한 속도로 이동하도록 설정
        float moveSpeed = 0.5f;
        transform.position += moveSpeed * Time.deltaTime * goalDirection;

        // 적 카트가 골대를 향해 정면이 보이도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(goalDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // ####### 사격 액션 #######



    // 추가 구현 사항
    
    // 4. 사격을 피하는 움직임
    // 4-1. 적 카트가 플래그를 소유하고 있고, 
    //      플레이어가 추격하며 카트가 사격 범위 안에 들어올 경우
    //      골대를 향해가면서도 사격 범위에서 벗어나기 위해 좌우 무빙
    //      좌우 방향 선택은 골대의 위치에 의존하게 해줘
    // 4-2. 적 카트가 플래그를 소유하고 있지 않지만,
    //      플레이어가 사격하고 있는 경우
    // void EvadeShoots()
    // {
    //     // 플래그를 소유하고 있고, 플레이어가 추격 중인 경우에만 실행

    // }

    // 5. 사격 범위에 상대가 있는 여부
    // ShootingEnemyCar에 있음

}

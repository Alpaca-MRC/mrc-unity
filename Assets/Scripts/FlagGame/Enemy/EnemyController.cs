using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public enum MovingState
{
    Stop, Stunned, MovingToFlag, MovingToPlayer, MovingToGoal, CanMove
}

public class EnemyController : MonoBehaviour
{
    // 모델
    public Transform playerCart;    // 플레이어 카트 위치
    public Transform flag;          // 플래그 위치
    public Transform playerGoal;    // 플레이어 골대
    public Transform enemyGoal;     // 적 골대
    // public GameObject enemyCartPrefab;  // 적 카트
    public EnemyCartModel enemyCart;

    // 게임 상태 관리
    public FlagManager flagManager;     // 플래그 상태 관리
    public MovingState movingState;     // 적 카트의 움직임 상태
    public bool isPlayerInRange;        // 플레이어 카트의 사격 범위 안에 있는지 여부
    public bool isLowHealth;            // 체력이 20% 이하인지 여부
    public bool isGameStarted;          // 게임 시작됐는지 여부
    public bool isShooting;             // 사격 중인지 여부

    public GameManager gameManager;     // 게임 매니저

    private void Start()
    {
        // 초기 설정
        GenerateEnemy();

        // 게임 변수 초기화
        InitializationGameSetting();

        // 카트 생성
        enemyCart = new EnemyCartModel();
        // isPlayerInRange = true;
        // flagState = FlagState.OnPlayer;
        // flagState = FlagState.OnEnemy;
    }

    private void Update()
    {
        // 프레임마다 행동 호출
        if (gameManager.gameState == GameState.InProgress && movingState != MovingState.Stunned)
        {
            ActionCommend();
        }
    }

    // 게임 변수 초기화
    void InitializationGameSetting()
    {
        flagManager.flagState = FlagState.OnBoard;
        movingState = MovingState.Stop;
        isPlayerInRange = false;
        isLowHealth = false;
    }

    // 적 생성(초기화)
    void GenerateEnemy()
    {
        Vector3 enemyPosition = GetRandomSpawnPosition();
        
        // 현재 게임 오브젝트를 적 위치로 옮기기
        transform.position = enemyPosition;
    }

    // 적 생성 위치 반환
    Vector3 GetRandomSpawnPosition()
    {
        // 플래그를 중심으로 플레이어 카트와 대칭인 점에서 생성
        Vector3 flagPosition = flag.position;
        Vector3 playerCartPosition = playerCart.position;
        Vector3 spawnPosition = flagPosition + (flagPosition - playerCartPosition);
        spawnPosition.y = 0f; // x, z 평면에서 생성
        
        return spawnPosition;
    }

    // 상태에 따른 액션
    void ActionCommend()
    {

        // 1. 플래그 바닥에 있는 경우
        // 1-1. 플래그를 향해 이동
        if (flagManager.flagState == FlagState.OnBoard)
        {
            // Debug.Log("Flag를 향해 이동");
            MoveToFlag();

        }
        // 2. 플래그가 플레이어에게 있는 경우
        else if (flagManager.flagState == FlagState.OnPlayer)
        {
            // 플레이어가 사격 범위 안에 있을 때
            if (isPlayerInRange)
            {
                Attack();
            }
            // Debug.Log("Player를 향해 이동");
            MoveToPlayer();

        }
        // 3. 플래그가 적에게 있는 경우
        // 3-1. 골대를 향해 이동
        else if (flagManager.flagState == FlagState.OnEnemy)
        {
            // Debug.Log("Goal를 향해 이동");
            MoveToGoal();
        }
    }

    // ################## 카트 체력에 따른 이벤트 ########################

    
    // 1. 체력 소진
    public void Exhaustion()
    {
        // 1초 스턴(테스트 상황에 따라 추가 구현)
        StartCoroutine(Stun(1f));

        // 플래그 떨어뜨리기
        flagManager.Drop();
        
        // 체력 다시 채워주기
        enemyCart.Heal();
    }

    // 2. 스턴
    // 스턴 상태를 해제하는 코루틴
    IEnumerator Stun(float time)
    {
        Debug.Log("스턴 ㅠㅠㅠㅠ");
        movingState = MovingState.Stunned;

        // 주어진 시간만큼 대기
        yield return new WaitForSeconds(time);

        // 스턴 상태를 해제
        movingState = MovingState.Stop;
    }


    // ##################### 이동 액션 ######################
    
    // 1. 플래그를 향해 이동
    void MoveToFlag()
    {
        // 플래그 방향 벡터 계산
        Vector3 flagDirection = (flag.position - transform.position).normalized;
        flagDirection.y = 0f;

        // 적 카트가 플래그 방향으로 일정한 속도로 이동하도록 설정
        float moveSpeed = 20f;
        transform.position += flagDirection * moveSpeed * Time.deltaTime;

        // 적 카트가 플래그를 향해 정면이 보이도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(flagDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // 2. 플레이어를 향해 이동
    void MoveToPlayer()
    {
        // 플레이어 방향 벡터 계산
        Vector3 playerDirection = (playerCart.position - transform.position).normalized;
        playerDirection.y = 0f;

        // 적 카트가 플레이어 방향으로 일정한 속도로 이동하도록 설정
        float moveSpeed = 20f;
        transform.position += playerDirection * moveSpeed * Time.deltaTime;

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
        Vector3 goalDirection = (enemyGoal.position - transform.position).normalized;
        goalDirection.y = 0f;

        // 적 카트가 골대 방향으로 일정한 속도로 이동하도록 설정
        float moveSpeed = 20f;
        transform.position += goalDirection * moveSpeed * Time.deltaTime;

        // 적 카트가 골대를 향해 정면이 보이도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(goalDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // 4. 사격을 피하는 움직임
    // 4-1. 적 카트가 플래그를 소유하고 있고, 
    //      플레이어가 추격하며 카트가 사격 범위 안에 들어올 경우
    //      골대를 향해가면서도 사격 범위에서 벗어나기 위해 좌우 무빙
    //      좌우 방향 선택은 골대의 위치에 의존하게 해줘
    // 4-2. 적 카트가 플래그를 소유하고 있지 않지만,
    //      플레이어가 사격하고 있는 경우
    void EvadeShoots()
    {
        // 플래그를 소유하고 있고, 플레이어가 추격 중인 경우에만 실행

    }

    // 5. 사격 범위에 상대가 있는 여부
    // ShootingEnemyCar에 있음

    // ####### 사격 액션 #######
    void Attack()
    {
        // ShootingEnemyCar 사격 시작
        EnemyShootingCar enemyCar = GetComponent<EnemyShootingCar>();
        if (enemyCar != null)
        {
            Debug.Log("사격 메서드 호출");
            enemyCar.ShootAction();
        }
    }

        
    // 피격
    void Hit()
    {
        // 체력 달기
        if (enemyCart.curHealth > 0)
        {
            enemyCart.Hit();
        } 
        else
        {
            Exhaustion();
        }
    }
    
    
}

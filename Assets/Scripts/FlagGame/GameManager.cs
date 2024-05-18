using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using Quaternion = UnityEngine.Quaternion;

public enum GameState
{
    // 시작 전
    BeforeStart,
    // 진행중
    InProgress,
    // 일시중지
    Paused,
    // 종료
    End
}

public class GameManager : MonoBehaviour
{
    private float gameTimeInSeconds = 10f; // 게임 시간(5분)
    public GameState gameState = GameState.BeforeStart;

    // 예시 카트 포탈 prefab
    public GameObject _cartExamplePrefab;
    private GameObject cartExample;

    // 예시 카트 포탈 위치
    private Vector3 examplePosition;
    private Quaternion exampleRotation;

    // 플레이어 카트 prefab
    // public GameObject _friendlyCartPrefab;
    public GameObject friendlyCart;

    // 적 카트 prefab
    // public GameObject _enemyCartPrefab;
    public GameObject enemyCart;

    // 왼쪽 컨트롤러
    [SerializeField]
    private InputActionReference _leftActivateAction;

    [SerializeField]
    private XRRayInteractor _leftRayInteractor; // 왼쪽 컨트롤러 ray 설정

    // 골대
    // 골대 생성 controller
    public GameObject[] _gatePrefabs;       // 게이트 prefabs
    private bool _gateInstallLock = true;   // 게이트 중복 생성 방지
    private int _infoStatus = 0;        // 0일때 1번 골대 설치 > 1일때 2번 골대 설치
    public GameObject gateOne;          // 플레이어 게이트 instance
    public GameObject gateTwo;          // 적 게이트 instance
    public Transform gateOnePosition;   // 플레이어 게이트 위치
    public Transform gateTwoPosition;   // 적 게이트 위치

    // 플래그 
    public FlagManager flagManager;     // 플래그 매니저
    // public GameObject _flagPrefab;      // 플래그 prefab
    public GameObject flag;             // 플래그 인스턴스

    // 앵커
    private ARAnchorManager _anchorManager;
    private List<ARAnchor> _anchors = new();

    // 게임 UI 게임 오브젝트
    public GameObject infoGameProcessGameObject;        // 환영합니다. 게임을 준비하겠습니다.
    public TextMeshProUGUI infoText;
    public GameObject selectFirstGateModalGameObject;   // 아군 골대 위치를 지정해주세요.
    public TextMeshProUGUI selectFirstGateText;         
    public GameObject selectSecondGateModalGameObject;  // 적군 골대 위치를 지정해주세요.
    public TextMeshProUGUI selectSecondGateText;
    public GameObject infoMoveYourCartModalGameObject;  // 카트를 포탈 위치로 이동해주세요.
    public TextMeshProUGUI moveCartText;

    public GameObject infoCreateFlagModalGameObject;    // 플래그가 생성됩니다.
    public TextMeshProUGUI createFlagText;

    public GameObject readyGameObject;                  // 카트 이동 완료
    public Button _readyToGoBtn;                        // 카트 이동 완료 버튼
    public TextMeshProUGUI gameReadyText;
    public GameObject countDownGameObject;              // 카운트 다운
    public GameObject startGameObject;                  // Start
    public GameObject oneGameObject;                    // 1
    public GameObject twoGameObject;                    // 2 
    public GameObject threeGameObject;                  // 3
    public GameObject winGameObject;                    // win
    public GameObject loseGameObject;                   // lose
    
    // 게임 UI 텍스트
    public TextMeshProUGUI timer;                       // 타이머
    public TextMeshProUGUI playerScoreText;             // 아군 점수
    public TextMeshProUGUI enemyScoreText;              // 적군 점수

    // 게임 스코어
    public int playerScore;
    public int enemyScore;

    void Start()
    {
        // Ray 끄기
        _leftRayInteractor.enabled = false;

        // 준비완료 버튼 끄기
        _readyToGoBtn.gameObject.SetActive(false);
        friendlyCart.SetActive(false);
        enemyCart.SetActive(false);
        flag.SetActive(false);

        // 사용하지 않는 ui 비활성화
        ActivateUI(infoGameProcessGameObject);

        // 시작시 안내멘트 출력
        
        StartCoroutine(InforBeforeGameStart());

        // 골대 생성
        _anchorManager = GetComponent<ARAnchorManager>();

        if (_anchorManager is null) {
            Debug.LogError("--> 'ARAnchorManager'를 찾을 수 없음");
        }


        _anchorManager.anchorsChanged += OnAnchorsChanged;
        _leftActivateAction.action.performed += OnLeftActivateAction;
    }
    private Vector3 enemyCartPosition;

    IEnumerator InforBeforeGameStart() 
    {
        Debug.Log("게임 시작됐나?");
        infoText.text = "환영합니다";
        yield return new WaitForSecondsRealtime(2.0f);
        infoText.text = "게임을 준비하겠습니다";
        yield return new WaitForSecondsRealtime(2.0f);
        ActivateUI(selectFirstGateModalGameObject);
        selectFirstGateText.text = "아군 골대를 지정해주세요";
        _leftRayInteractor.enabled = true;
        _gateInstallLock = false;
    }

    // ui 오브젝트 활성화 메서드
    void ActivateUI(GameObject UIGameObject)
    {
        infoGameProcessGameObject.SetActive(false);
        selectFirstGateModalGameObject.SetActive(false);
        selectSecondGateModalGameObject.SetActive(false);
        infoMoveYourCartModalGameObject.SetActive(false);
        infoCreateFlagModalGameObject.SetActive(false);
        readyGameObject.SetActive(false);
        countDownGameObject.SetActive(false);
        winGameObject.SetActive(false);
        loseGameObject.SetActive(false);

        if (UIGameObject != null) UIGameObject.SetActive(true);
    }

    // 두번째 게이트 실치부터
    IEnumerator InfoInstallGateTwo() 
    {
        selectFirstGateText.text = "아군 골대 지정이 완료되었습니다";
        yield return new WaitForSecondsRealtime(1.5f);
        ActivateUI(selectSecondGateModalGameObject);
        selectSecondGateText.text = "적군 골대를 지정해주세요";
        _leftRayInteractor.enabled = true;
        _gateInstallLock = false;
    }

    // 카트 이동 완료 버튼
    public void OnClickCompleteReady()
    {
        Debug.Log(1);
        // 눌린 버튼 비활성화
        // 레이 끄기
        _leftRayInteractor.enabled = false;
        
        // 예시 카트 및 포탈 파괴
        Destroy(cartExample);
        
        // 플레이어 카트 생성
        // friendlyCart = Instantiate(_friendlyCartPrefab, examplePosition, exampleRotation);
        friendlyCart.SetActive(true);
        friendlyCart.transform.SetPositionAndRotation(examplePosition, exampleRotation);
        // Debug.Log("friendlyCart의 rotation이에요: " + exampleRotation);
        // 플레이어 카트 이동 및 사격 제한
        friendlyCart.GetComponent<FriendlyCarMove>().enabled = false;
        friendlyCart.GetComponent<FriendlyShootingCar>().enabled = false;
        // 적 카트 생성
        Vector3 enemyPosition = gateTwoPosition.position + (gateOnePosition.position - gateTwoPosition.position).normalized / 2f;
        enemyPosition.y = 0f;
        // enemyCart = Instantiate(_enemyCartPrefab, enemyPosition, Quaternion.identity);
        enemyCart.SetActive(true);
        enemyCart.transform.position = enemyPosition;
        enemyCart.transform.LookAt(gateOnePosition);
        // rotation 재조정
        enemyCart.transform.rotation = Quaternion.Euler(0f, enemyCart.transform.rotation.eulerAngles.y, 0f);
        // 플래그 생성
        StartCoroutine(GenerateFlag());
    }

    // 골대 사이에 플래그 생성
    IEnumerator GenerateFlag() 
    {
        ActivateUI(infoCreateFlagModalGameObject);
        createFlagText.text = "플래그가 생성됩니다.";
        Vector3 flagPosition = (gateOnePosition.position + gateTwoPosition.position) / 2f;
        flagPosition.y = 0f;
        flagManager.spawnPosition = flagPosition;
        Debug.Log("설정한 spawnPosition: " + flagManager.spawnPosition);
        // flag = Instantiate(_flagPrefab, flagPosition, Quaternion.identity);
        flag.SetActive(true);
        flag.transform.position = flagPosition;
        flag.transform.rotation = Quaternion.identity;
        yield return new WaitForSecondsRealtime(2.0f);
        ActivateUI(readyGameObject);
        gameReadyText.text = "잠시후 게임을 시작합니다.";
        yield return new WaitForSecondsRealtime(1.0f);
        StartGame();
    }

    // 게임 시작
    void StartGame()
    {
        // 타이머 초기화
        UpdateLapTime(gameTimeInSeconds);
        // 카운트 다운 시작
        StartCoroutine(StartCountdown());
        // 게임 설정 초기화
        InitializationGameSetting();
    }

    // 아군 골대 앞 카트 위치 생성(포탈)
    IEnumerator GenerateCartPortal()
    {
        selectSecondGateText.text = "적군 골대 지정이 완료되었습니다";
        // 1번골대가 2번 골대를 바라보게 하기
        gateOne.transform.LookAt(gateTwoPosition);
        gateOne.transform.rotation = Quaternion.Euler(0f, gateOne.transform.eulerAngles.y, 0f);
        gateOnePosition = gateOne.transform;
        // 2번 골대가 1번 골대를 바라보게 하기
        gateTwo.transform.LookAt(gateOnePosition);
        gateTwo.transform.rotation = Quaternion.Euler(0f, gateTwo.transform.eulerAngles.y, 0f);
        gateTwoPosition = gateTwo.transform;
        yield return new WaitForSecondsRealtime(1.5f);
        ActivateUI(infoMoveYourCartModalGameObject);
        // 골대 앞 포탈 생성
        examplePosition = gateOnePosition.position + (gateTwoPosition.position - gateOnePosition.position).normalized / 2f;
        examplePosition.y = 0f;
        cartExample = Instantiate(_cartExamplePrefab, examplePosition, Quaternion.identity);
        cartExample.transform.LookAt(gateTwoPosition);
        // 방향 설정
        cartExample.transform.rotation = Quaternion.Euler(0f, cartExample.transform.rotation.eulerAngles.y, 0f);
        exampleRotation = cartExample.transform.rotation;
        // 배치 완료 버튼 출력
        moveCartText.text = "초록색 포탈에 올려진 카트와 동일한 모양으로 RC카를 이동해주세요";
        _leftRayInteractor.enabled = true;
        _readyToGoBtn.gameObject.SetActive(true);
    }

    // 왼쪽 트리거 클릭이 발생한다면
    private void OnLeftActivateAction(InputAction.CallbackContext context)
    {
        if (_infoStatus >= 2 || _gateInstallLock) {
            return;
        }
        CheckIfRayHitsCollider(_infoStatus);
    }

    private void CheckIfRayHitsCollider(int role) {
        // 만약 Ray가 Collider를 친다면
        if (_leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) {
            // 닿은 물체가 바닥일때만 기능 및 카드가 존재하지 않을때 기능
            ARPlane hitPlane = hit.transform.GetComponent<ARPlane>();
            // if (hitPlane != null && hitPlane.classification == PlaneClassification.Floor && !_gateInstallLock) {
            if (!_gateInstallLock) {
                // 회전각 결정
                Quaternion rotation = Quaternion.Euler(0, 0, 0);
                // 물체 생성
                GameObject instance = Instantiate(_gatePrefabs[role], hit.point, rotation);
                // 추가 설치 못하도록 잠그기
                _gateInstallLock = true;
                // 잠갔다면 infoStatus 하나 올리기
                _infoStatus += 1;
                _leftRayInteractor.enabled = false;

                // 골대의 Rotation 변화(서로 바라보아야 함)를 위해 anchor 주석 처리
                // if (instance.GetComponent<ARAnchor>() == null) {
                //     ARAnchor anchor = instance.AddComponent<ARAnchor>();

                //     if (anchor != null) {
                //         _anchors.Add(anchor);
                //     }
                //     else {
                //         Debug.LogError("Anchor가 없어요...");
                //     }
                // }

                if (_infoStatus == 1) {
                    gateOne = instance;
                    gateOnePosition = instance.transform;
                    // 2번 골대 생성부터 시작
                    StartCoroutine(InfoInstallGateTwo());
                }

                if (_infoStatus == 2) {
                    gateTwo = instance;
                    gateTwoPosition = instance.transform;
                    // 카트 지정
                    StartCoroutine(GenerateCartPortal());
                }
            }
        } else {
            Debug.Log("--> 닿은게 없음");
        }
    }

    // Anchor(골대) 삭제
    private void OnAnchorsChanged(ARAnchorsChangedEventArgs args)
    {
        // 통제 범위 밖에서 anchor가 지워졌다면 우리 리스트에서도 지워주기
        foreach (var removeAnchor in args.removed) {
            _anchors.Remove(removeAnchor);
            Destroy(removeAnchor.gameObject);
        }
    }

    // 게임 초기화
    public void InitializationGameSetting()
    {
        playerScore = 0;
        enemyScore = 0;
        gameTimeInSeconds = 10f;
        playerScoreText.text = playerScore.ToString();
        enemyScoreText.text = enemyScore.ToString();
        
    }

    // 랩타입 초기화
    public void UpdateLapTime(float lapTime)
    {
        int minutes = Mathf.FloorToInt(lapTime / 60f);
        int seconds = Mathf.FloorToInt(lapTime % 60f);
        int milliseconds = Mathf.FloorToInt((lapTime * 1000f) % 1000f);
        string timerString = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds / 10);

        timer.text = "Timer: " + timerString;
    }

    // 카운트 다운
    IEnumerator StartCountdown()
    {
        ActivateUI(countDownGameObject);
        startGameObject.SetActive(false);
        threeGameObject.SetActive(false);
        twoGameObject.SetActive(false);
        oneGameObject.SetActive(false);
        // 3초 카운트 다운
        for (int i = 3; i > 0; i--)
        {
            // countdownText.text = i.ToString();
            if (i == 3) threeGameObject.SetActive(true);
            if (i == 2) {
                threeGameObject.SetActive(false);
                twoGameObject.SetActive(true);
            }
            if (i == 1) {
                twoGameObject.SetActive(false);
                oneGameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1f);
        }

        oneGameObject.SetActive(false);
        startGameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        ActivateUI(null);

        // 게임 시작
        gameState = GameState.InProgress;
        // 플레이어 카트 이동 및 사격 허용
        friendlyCart.gameObject.GetComponent<FriendlyCarMove>().enabled = true;
        friendlyCart.gameObject.GetComponent<FriendlyShootingCar>().enabled = true;
        // 적 카트 이동 및 사격
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        float timer = gameTimeInSeconds;

        while (timer > 0 && gameState == GameState.InProgress)
        {
            timer -= Time.deltaTime;

            // if (timer < 5f)
            // {
            //     enemyController.Exhaustion();
            // }

            UpdateLapTime(timer);
            yield return null;
        }

        // 게임 종료
        // EndGame();
    }

    void EndGame()
    {
        // 게임 종료 처리 구현
        gameState = GameState.End;
        Debug.Log("게임 종료");
    }

    /// <summary>
    /// 0: 플레이어 / 1: 적군
    /// </summary>
    /// <param name="role"></param>
    public void IncreaseScore(int role)
    {
        switch (role)
        {
            // 플레이어 승리시
            case 0:
                playerScore++;
                playerScoreText.text = playerScore.ToString();
                break;
                
            // 적군 승리시
            case 1:
                enemyScore++;
                enemyScoreText.text = enemyScore.ToString();
                break;
            
            default:
                break;
        }
    }
}

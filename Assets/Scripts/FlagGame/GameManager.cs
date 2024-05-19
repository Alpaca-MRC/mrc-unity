using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Type3D;
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
    public GameObject friendlyCart;

    // 적 카트 prefab
    public GameObject enemyCart;

    // 왼쪽 컨트롤러
    [SerializeField]
    private InputActionReference _leftActivateAction;

    [SerializeField]
    private XRRayInteractor _leftRayInteractor; // 왼쪽 컨트롤러 ray 설정
    [SerializeField]
    private TimerManager timerManager;
    [SerializeField]
    private ScoreManager friendlyScoreManager;
    [SerializeField]
    private ScoreManager enemyScoreManager;

    // 골대
    // 골대 생성 controller
    public GameObject[] _gatePrefabs;       // 게이트 prefabs
    private bool _gateInstallLock = true;   // 게이트 중복 생성 방지
    private int _infoStatus = 0;        // 0일때 1번 골대 설치 > 1일때 2번 골대 설치
    public GameObject gateOne;          // 플레이어 게이트 instance
    public GameObject gateTwo;          // 적 게이트 instance
    public Transform gateOnePosition;   // 플레이어 게이트 위치
    public Transform gateTwoPosition;   // 적 게이트 위치
    [SerializeField]
    private ParticleSystem lineFireworksParticle;
    [SerializeField]
    private ParticleSystem fanFireworksParticle;

    // 플래그 
    public FlagManager flagManager;     // 플래그 매니저
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
    [SerializeField]
    private GameObject InGameGameObject;
    public GameObject loseGameObject;                   // lose

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

        // 폭죽 비활성화
        if (lineFireworksParticle.isPlaying) lineFireworksParticle.Stop();
        if (fanFireworksParticle.isPlaying) fanFireworksParticle.Stop();

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

    IEnumerator InforBeforeGameStart() 
    {
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
        InGameGameObject.SetActive(false);

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
        friendlyCart.SetActive(true);
        friendlyCart.transform.SetPositionAndRotation(examplePosition, exampleRotation);
        // 플레이어 카트 이동 및 사격 제한
        friendlyCart.GetComponent<FriendlyCarMove>().enabled = false;
        friendlyCart.GetComponent<FriendlyShootingCar>().enabled = false;
        // 적 카트 생성
        Vector3 enemyPosition = gateTwoPosition.position + (gateOnePosition.position - gateTwoPosition.position).normalized / 2f;
        enemyPosition.y = 0f;
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
        flagManager.dropDistance = Vector3.Distance(gateOnePosition.position, gateTwoPosition.position) / 8f;
        flag.SetActive(true);

        // InGame GameObject도 골대 사이에 위치
        Vector3 inGameUIPosition = flagPosition;
        inGameUIPosition.y = 1f;
        InGameGameObject.transform.position = inGameUIPosition;

        // 방향은 두 골대를 잇는 직선에 직교하도록, 그리고 XR Origin쪽 방향 선택
        Vector3 direction = gateTwoPosition.position - gateOnePosition.position;
        Vector3 perpendicularDirection = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 xrOriginDirection = (transform.position - inGameUIPosition).normalized;

        // 방향을 XR Origin 쪽으로 설정
        if (Vector3.Dot(perpendicularDirection, xrOriginDirection) >= 0)
        {
            perpendicularDirection = -perpendicularDirection;
        }

        // InGameGameObject의 회전 설정
        InGameGameObject.transform.rotation = Quaternion.LookRotation(perpendicularDirection);

        // Score 각 골대 위에 두기
        Transform friendlyScore = InGameGameObject.transform.Find("Friendly Score");
        Transform enemyScore = InGameGameObject.transform.Find("Enemy Score");

        if (friendlyScore != null && enemyScore != null)
        {   
            // Friendly Score를 Friendly Gate 위에 배치 (왼쪽으로 이동)
            Vector3 friendlyScorePosition = gateOnePosition.position;
            friendlyScorePosition.y += 1f;
            friendlyScore.position = friendlyScorePosition;

            // Enemy Score를 Enemy Gate 위에 배치 (왼쪽으로 이동)
            Vector3 enemyScorePosition = gateTwoPosition.position;
            enemyScorePosition.y += 1f;
            enemyScore.position = enemyScorePosition;

            friendlyScore.LookAt(transform);
            enemyScore.LookAt(transform);
            friendlyScore.rotation *= Quaternion.Euler(0, 180, 0);
            enemyScore.rotation *= Quaternion.Euler(0, 180, 0);

            // y축 회전을 0으로 설정
            Vector3 friendlyEulerAngles = friendlyScore.rotation.eulerAngles;
            friendlyEulerAngles.y = 0;
            friendlyScore.rotation = Quaternion.Euler(friendlyEulerAngles);

            Vector3 enemyEulerAngles = enemyScore.rotation.eulerAngles;
            enemyEulerAngles.y = 0;
            enemyScore.rotation = Quaternion.Euler(enemyEulerAngles);

            // 왼쪽으로 이동
            friendlyScore.position -= friendlyScore.right;
            enemyScore.position -= enemyScore.right;
        }

        flag.transform.SetPositionAndRotation(flagPosition, Quaternion.identity);
        yield return new WaitForSecondsRealtime(2.0f);
        ActivateUI(readyGameObject);
        gameReadyText.text = "잠시후 게임을 시작합니다.";
        yield return new WaitForSecondsRealtime(1.0f);
        StartGame();
    }

    // 게임 시작
    void StartGame()
    {
        // 카운트 다운 시작
        StartCoroutine(StartCountdown());
        // 게임 설정 초기화
        InitializationGameSetting();
        ActivateUI(InGameGameObject);
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
        gameTimeInSeconds = 10f;
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

        // 게임 시작
        gameState = GameState.InProgress;
        // 플레이어 카트 이동 및 사격 허용
        friendlyCart.gameObject.GetComponent<FriendlyCarMove>().enabled = true;
        friendlyCart.gameObject.GetComponent<FriendlyShootingCar>().enabled = true;
        // 적 카트 이동 및 사격
        StartCoroutine(timerManager.GameTimer(gameTimeInSeconds));
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
            // 플레이어 골인시
            case 0:
                friendlyScoreManager.UpdateScore();
                PlayFireworks(0);
                break;
                
            // 적군 골인시
            case 1:
                enemyScoreManager.UpdateScore();
                PlayFireworks(1);
                break;
            
            default:
                break;
        }
    }

    // 골인시 폭죽 효과 실행
    void PlayFireworks(int role) {
        switch (role)
        {
            // 플레이어 골인시
            case 0:
                lineFireworksParticle.transform.position = gateOne.transform.position;
                lineFireworksParticle.transform.rotation = gateOne.transform.rotation;
                fanFireworksParticle.transform.position = gateOne.transform.position;
                fanFireworksParticle.transform.rotation = gateOne.transform.rotation;
                if (!lineFireworksParticle.isPlaying) lineFireworksParticle.Play();
                if (!fanFireworksParticle.isPlaying) fanFireworksParticle.Play();
                break;
            
            // 적군 골인시
            case 1:
                lineFireworksParticle.transform.position = gateTwo.transform.position;
                lineFireworksParticle.transform.rotation = gateTwo.transform.rotation;
                fanFireworksParticle.transform.position = gateTwo.transform.position;
                fanFireworksParticle.transform.rotation = gateTwo.transform.rotation;
                if (!lineFireworksParticle.isPlaying) lineFireworksParticle.Play();
                if (!fanFireworksParticle.isPlaying) fanFireworksParticle.Play();
                break;
            
            default:
                break;
        }
    }
}

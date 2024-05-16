using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public float gameTimeInSeconds = 10f; // 게임 시간(5분)
    public GameState gameState = GameState.BeforeStart;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI lapTimeText;
    public Camera mainCamera;

    // 게임 스코어
    public int playerScore;
    public int enemyScore;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;

    void Start()
    {
        // 랩타임 초기화
        UpdateLapTime(gameTimeInSeconds);
        StartCoroutine(StartCountdown());
        InitializationGameSetting();
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
        string lapTimeString = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds / 10);

        lapTimeText.text = "Lap Time: " + lapTimeString;
    }

    IEnumerator StartCountdown()
    {
        // 3초 카운트 다운
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

        // 게임 시작
        gameState = GameState.InProgress;
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        float timer = gameTimeInSeconds;

        while (timer > 0 && gameState == GameState.InProgress)
        {
            timer -= Time.deltaTime;
            UpdateLapTime(timer);
            yield return null;
        }

        // 게임 종료
        EndGame();
    }

    void EndGame()
    {
        // 게임 종료 처리 구현
        gameState = GameState.End;
        Debug.Log("게임 종료");
    }

    public void IncreaseScore(String player)
    {
        if (player == "player")
        {
            playerScore++;
            playerScoreText.text = playerScore.ToString();
        }
        if (player == "enemy")
        {
            enemyScore++;
            Debug.Log("enemyScore : " + enemyScore);
            enemyScoreText.text = enemyScore.ToString();
        }
    }

}

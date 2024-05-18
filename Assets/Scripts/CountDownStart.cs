using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownStart : MonoBehaviour
{

    private int Timer = 0;
    public GameObject One;   //1번
    public GameObject Two;   //2번
    public GameObject Three;   //3번
    public GameObject START;   //시작 이미지
    
    void Start()
    {
        //시작시 카운트 다운 초기화, 게임 시작 false 설정
        Timer = 0;
        // 튜토리얼, 나머지 (카운트다운 이미지) 안보이기
        Three.SetActive(false);
        Two.SetActive(false);
        One.SetActive(false);
        START.SetActive(false);
    }
    
    void Update()
    {
        //게임 시작시 정지
        if (Timer == 0)
        {
            Time.timeScale = 0.0f;
        }
        //Timer 가 90보다 작거나 같을경우 Timer 계속증가
        if (Timer <= 360)
        {
            Timer++;
 
            // Timer가 60보다 작을경우 3 켜기
            if (Timer < 72)
            {
                Three.SetActive(true);
            }
            // Timer가 60보다 클경우 3 끄고 2켜기
            if (Timer > 144)
            {
                Three.SetActive(false);
                Two.SetActive(true);
            }
            // Timer가 90보다 작을경우 2끄고 1켜기
            if (Timer > 216)
            {
                Two.SetActive(false);
                One.SetActive(true);
            }
            // Timer 가 120보다 클경우 1끄고 START 켜기
            if (Timer > 360)
            {
                One.SetActive(false);
                START.SetActive(true);
                StartCoroutine(this.LoadingEnd());
                Time.timeScale = 1.0f; //게임시작
            }
        }
    }
    
    IEnumerator LoadingEnd()
    {
        yield return new WaitForSeconds(1.0f);
        START.SetActive(false);
    }
    
}

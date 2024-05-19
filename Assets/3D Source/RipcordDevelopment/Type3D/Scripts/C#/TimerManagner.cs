using UnityEngine;
using System.Collections;
using System;

namespace Type3D
{
    public class TimerManager : MonoBehaviour
    {
        [SerializeField]
        private Mesh[] numbers;						// 숫자를 표현할 메쉬 객체 배열 (0-9까지)
        [SerializeField]
        private GameObject[] timerDigits;			// 타이머의 각 자리 숫자를 표시할 게임 오브젝트 배열
        private float timer;                        // 타이머가 마지막으로 리셋된 이후 경과한 시간 (초 단위)

        void UpdateLapTime(float _time)
        {
            int minutes = (int)Mathf.Floor(_time / 60);
            string seconds = Mathf.Floor(_time % 60).ToString("00");
            string hundredths = Mathf.Floor((_time * 100) % 100).ToString("00");

            timerDigits[4].GetComponent<MeshFilter>().mesh = numbers[minutes];
            timerDigits[3].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(seconds.Substring(0, 1))];
            timerDigits[2].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(seconds.Substring(1, 1))];
            timerDigits[1].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(hundredths.Substring(0, 1))];
            timerDigits[0].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(hundredths.Substring(1, 1))];
        }

        public IEnumerator GameTimer(float gameTimeInSeconds)
        {
            timer = gameTimeInSeconds;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                UpdateLapTime(timer);
                yield return null;
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Type3D
{
    public class ScoreManager : MonoBehaviour
    {
        public Mesh[] numbers;						// 숫자를 나타내는 메쉬 객체 배열 (0-9까지)
        public GameObject[] scoreDigits;			// 점수의 각 자리 숫자를 표시할 게임 오브젝트 배열
        public bool animateOnUpdate;				// 점수 업데이트 시 애니메이션 효과를 적용할지 여부

        [HideInInspector] public int score;		// 타입3D 문자로 변환될 점수 값

        public void UpdateScore ()
        {
            score++;								// 현재 점수에 1을 더함

            string scoreString = score.ToString();	// 점수를 문자열로 변환

            if (scoreString.Length >= scoreDigits.Length) // 점수 문자열의 길이가 점수 자릿수 배열보다 크거나 같으면...
            {
                for (int x = 0; x < scoreDigits.Length; x++) // 각 자리 숫자를 순회하면서...
                {
                    scoreDigits[x].GetComponent<MeshFilter>().mesh = numbers[9]; // 각 자리 숫자를 9로 설정 (최대 점수 표시)
                }
            }
            else // 그렇지 않으면...
            {
                for (int x = 0; x < scoreString.Length; x++) // 점수 문자열의 각 문자를 순회하면서...
                {
                    int digitValue = System.Convert.ToInt32(scoreString.Substring(scoreString.Length - 1 - x, 1)); // 각 자리의 숫자 값을 추출
                    scoreDigits[x].GetComponent<MeshFilter>().mesh = numbers[digitValue]; // 추출한 숫자 값에 해당하는 메쉬를 설정
                }
            }

            if (animateOnUpdate && this.GetComponent<TypeEffects>()) // 점수 업데이트 시 애니메이션 효과가 활성화되어 있고, TypeEffects 컴포넌트가 있다면...
            {
                this.GetComponent<TypeEffects>().PlayEffect(); // 점수 표시에서 타입 효과를 재생
            }
        }
    }
}

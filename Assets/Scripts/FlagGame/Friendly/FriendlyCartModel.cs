using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyCartModel : MonoBehaviour
{
    public int maxHealth;         // 최대 체력
    public int curHealth;         // 현재 체력
    public bool hasFlag;            // 플래그 소유 여부

    // 적의 경우
    public float curSpeed;
    public float maxSpeed = 15.0f;

    public enum CartState { Normal, Stunned }
    public CartState currentState = CartState.Normal;   // 상태(정상, 기절)

    public FriendlyCartModel()
    {
        maxHealth = 100;
        curHealth = 100;
        hasFlag = false;

    }

    // 풀피 채우기
    public void Heal()
    {
        curHealth = maxHealth;
    }

    // 피격
    public void Hit()
    {
        curHealth -= 1;
    }

     void OnCollisionEnter(Collision collision) {
        Debug.Log("앗따가! 아군 차가 " + collision.collider.name + "에 맞았어요!");
        Debug.Log(collision.gameObject.layer + "라는 layer를 가지고 있네요?!");
     }

    // 피가 다 닳으면
    // 1. 깃발 drop 메서드 호출 (들고 있다면)
    // 2. stun 만들기
    // 3. hasFlag false 만들기
}

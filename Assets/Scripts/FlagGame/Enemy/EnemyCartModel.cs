using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCartModel
{
    public int maxHealth;         // 최대 체력
    public int curHealth;         // 현재 체력
    public bool hasFlag;            // 플래그 소유 여부

    // 적의 경우
    public float curSpeed;
    public float maxSpeed = 15.0f;

    public enum CartState { Normal, Stunned }
    public CartState currentState = CartState.Normal;   // 상태(정상, 기절)

    public EnemyCartModel()
    {
        maxHealth = 10;
        curHealth = 10;
        hasFlag = false;

        // 적
        curSpeed = 0f;
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

}

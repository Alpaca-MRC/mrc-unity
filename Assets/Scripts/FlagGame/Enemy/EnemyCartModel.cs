using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCartModel
{
    public int maxHealth;         // 최대 체력
    public int curHealth;         // 현재 체력
    public bool hasFlag;            // 플래그 소유 여부

    public float curSpeed;
    public float maxSpeed = 4.0f;

    public enum CartState { Normal, Stunned }
    public CartState currentState;   // 상태(정상, 기절)

    public EnemyCartModel()
    {
        maxHealth = 50;
        curHealth = 50;
        hasFlag = false;

        // 적
        curSpeed = 0f;
        currentState = CartState.Normal;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberManager : MonoBehaviour
{
    public static MemberManager instance = null;

    // 사용자 정보
    public Member currentUser;
    public CartData cartData;


    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 이 객체를 파괴하지 않도록 설정
        DontDestroyOnLoad(gameObject);

        // 유저 정보 초기화
        InitializeMemberInfo();
    }

    // 유저 정보 초기화 메서드
    private void InitializeMemberInfo()
    {

        // MemberAPI 인스턴스 생성
        MemberAPI memberAPI = new();

        // 유저 정보 조회 요청
        memberAPI.GetMemberInfoRequest(currentUser.username, (response) =>
        {
        // Member 클래스의 인스턴스 변수에 할당
        currentUser.username = response.username;
        currentUser.nickname = response.nickname;
        currentUser.selectedCart = cartData.FindCartByName(response.selectedCartName);

        currentUser.carts = new List<Cart>();
        foreach (string cartName in response.carts)
        {
            currentUser.carts.Add(cartData.FindCartByName(response.selectedCartName));
        }

        currentUser.accountType = response.accountType;
        currentUser.iconUrl = response.iconUrl;
        currentUser.coin = response.coin;

        });
        
    }
}

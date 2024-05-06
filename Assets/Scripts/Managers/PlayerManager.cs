using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance = null;

    // 사용자 정보
    public string userId;
    public string nickname;
    public List<Cart> cartList;
    public List<Avatar> avatarList;
    public Cart selectedCart;
    public Avatar selectedAvatar;
    public List<Record> records;

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

        // 사용자 정보 초기화
        InitializeUserInfo();
    }

    // 사용자 정보 초기화 메서드
    private void InitializeUserInfo()
    {
        // 사용자 정보 초기화 코드 작성
        userId = "User123";
        nickname = "Player1";
        cartList = new List<Cart>();
        avatarList = new List<Avatar>();
        selectedCart = null;
        selectedAvatar = null;
        records = new List<Record>();
    }
}

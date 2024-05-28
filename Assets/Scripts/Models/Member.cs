using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member
{
    public string username; // 사용자 ID
    public string nickname; // 사용자 닉네임
    public Cart selectedCart; // 선택된 카트
    public List<Cart> carts; // 사용자의 카트 목록
    public List<Record> records; // 사용자의 기록 목록
    public AccountType accountType;
    public string iconUrl;
    public int coin;

    // 초기화
    public void InitializeUser(string _username, string _nickname, List<Cart> _carts, Cart _selectedCart, List<Record> _records, AccountType _accountType, string _iconUrl, int _coin)
    {
        username = _username;
        nickname = _nickname;
        selectedCart = _selectedCart;
        carts = _carts;
        records = _records;
        accountType = _accountType;
        iconUrl = _iconUrl;
        coin = _coin;
    }

    // 카트 추가
    public void AddCart(Cart cart)
    {
        carts.Add(cart);
    }

    // 기록 업데이트
    public void UpdateRecord(List<Record> _records)
    {
        records = _records;
    }
    
    // 닉네임 변경
    public void updateNickname(String _nickname)
    {
        nickname = _nickname;
    }
}

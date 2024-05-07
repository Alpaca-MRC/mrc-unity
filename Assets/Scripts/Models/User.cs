using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{

    // userId, nickname, carts, characters, selectedCart, selectedCharacter, records
    public int userId; // 사용자 ID
    public string nickname; // 사용자 닉네임
    public List<Cart> carts; // 사용자의 카트 목록
    public List<Character> characters; // 사용자의 캐릭터 목록
    public Cart selectedCart; // 선택된 카트
    public Character selectedCharacter; // 선택된 캐릭터
    public List<Record> records; // 사용자의 기록 목록

    // 초기화
    public void InitializeUser(int id, string name)
    {
        userId = id;
        nickname = name;
        carts = new List<Cart>();
        characters = new List<Character>();
        records = new List<Record>();
    }

    // 카트 추가
    public void AddCart(Cart cart)
    {
        carts.Add(cart);
    }

    // 캐릭터 추가
    public void AddCharacter(Character character)
    {
        characters.Add(character);
    }

    // 기록 추가
    public void AddRecord(Record record)
    {
        records.Add(record);
    }

    // 카트 변경
    public void changeCart(Cart cart)
    {
        selectedCart = cart;
    }

    // 캐릭터 변경
    public void changeCharacter(Character character)
    {
        selectedCharacter = character;
    }

    // 닉네임 변경
    public void updateNickname(String _nickname)
    {
        nickname = _nickname;
    }
}

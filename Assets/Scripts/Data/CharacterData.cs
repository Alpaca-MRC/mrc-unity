using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public List<Character> characters; // 캐릭터 정보 리스트

    void Start()
    {
        // 캐릭터 정보 초기화
        InitializeCharacters();
    }

    // 캐릭터 정보 초기화 메서드
    private void InitializeCharacters()
    {
        characters = new List<Character>
        {
            new(1, "Basic Character"),
            new(2, "Speedy Character")
        };
        // 기타 캐릭터 정보 추가...
    }

    // 특정 캐릭터 정보 가져오기 메서드
    public Character GetCharacterInfo(int characterId)
    {
        foreach (Character character in characters)
        {
            if (character.id == characterId)
            {
                return character;
            }
        }
        return null;
    }
}

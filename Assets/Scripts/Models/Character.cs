using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int id; // 캐릭터의 고유 ID
    public new string name; // 캐릭터의 이름
    // 기타 카트 모델의 속성들
    
    // 생성자
    public Character(int _id, string _name)
    {
        id = _id;
        name = _name;
        // 기타 속성 초기화 코드
    }
}

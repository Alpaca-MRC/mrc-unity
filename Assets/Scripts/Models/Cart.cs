using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Grade {
    Normal,
    Rare,
    Unique,
    Legendary
}

public class Cart
{
    public int id; // 카트의 고유 ID
    public string name; // 카트의 이름
    public Grade grade; // 등급

    // 생성자
    public Cart(int _id, string _name, Grade _grade)
    {
        id = _id;
        name = _name;
        grade = _grade;
    }

}

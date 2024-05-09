using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartData : MonoBehaviour
{
    public List<Cart> cartList; // 카트 정보 리스트

    void Start()
    {
        InitializeCartList();
    }

    // 카트 정보 초기화 메서드
    private void InitializeCartList()
    {
        cartList = new List<Cart>
        {
            new(1, "노멀 카트 1", Grade.NORMAL),
            new(2, "노멀 카트 2", Grade.NORMAL),
            new(3, "레어 카트 1", Grade.RARE),
            new(4, "레어 카트 2", Grade.RARE),
            new(5, "유니크 카트 1", Grade.UNIQUE),
            new(6, "유니트 카트 2", Grade.UNIQUE),
            new(7, "레전더리 카트1", Grade.LEGENDARY),
            new(8, "레전더리 카트2", Grade.LEGENDARY),
            new(9, "노멀 카트 3", Grade.NORMAL),
            new(10, "노멀 카트 4", Grade.NORMAL),
            new(11, "레어 카트 3", Grade.RARE),
            new(12, "레어 카트 4", Grade.RARE),
            new(13, "유니크 카트 3", Grade.UNIQUE)
        };
        
    }

    // 특정 카트 정보 가져오기 메서드
    public Cart FindCartById(int cartId)
    {
        foreach (Cart cart in cartList)
        {
            if (cart.id == cartId)
            {
                return cart;
            }
        }
        return null;
    }

    // 이름으로 가져오기
    public Cart FindCartByName(string cartName)
    {
        foreach (Cart cart in cartList)
        {
            if (cart.name == cartName)
            {
                return cart;
            }
        }
        return null;
    }
}

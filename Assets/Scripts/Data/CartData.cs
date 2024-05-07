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
            new(1, "Basic Cart", Grade.Normal),
            new(2, "Speedy Cart", Grade.Normal),
            new(3, "Turbo Cart", Grade.Rare),
            new(4, "Super Cart", Grade.Rare),
            new(5, "Mega Cart", Grade.Unique),
            new(6, "Ultra Cart", Grade.Unique),
            new(7, "Hyper Cart", Grade.Legendary),
            new(8, "Epic Cart", Grade.Legendary),
            new(9, "Sonic Cart", Grade.Normal),
            new(10, "Blaze Cart", Grade.Normal),
            new(11, "Thunder Cart", Grade.Rare),
            new(12, "Fury Cart", Grade.Rare),
            new(13, "Inferno Cart", Grade.Unique)
        };
        
    }

    // 특정 카트 정보 가져오기 메서드
    public Cart GetCartInfo(int cartId)
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
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartUIManager : MonoBehaviour
{
    public TextMeshProUGUI cartNameText;
    public TextMeshProUGUI cartGradeText;
    public CartData cartData;
    
    public void GetCartInfo(int index)
    {
        Debug.Log("카드 정보 조회");

        
        // 카트 db에서 이름, 등급 가져오기
        Cart cart = cartData.GetCartInfo(index + 1);

        // 이름 변경
        cartNameText = GameObject.Find("NameContent_Txt").GetComponent<TextMeshProUGUI>();
        cartNameText.text = cart.name;

        // 등급 변경
        cartGradeText = GameObject.Find("GradeContent_Txt").GetComponent<TextMeshProUGUI>();
        cartGradeText.text = cart.grade.ToString();
    }
}

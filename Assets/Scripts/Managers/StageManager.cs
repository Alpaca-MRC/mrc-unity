using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject stage; // 단상 게임 오브젝트
    public CartData cartData;
    public KartSelectManager kartSelectManager;


    void Start()
    {
        // 사용자 카트 활성화 나머지 비활성화
        SelectMemberCart();
    }

    private void SelectMemberCart()
    {
        // 유저의 카트
        Cart selectedCart = MemberManager.instance.currentUser.selectedCart;
        
        // 모든 카트 가져오기
        GameObject[] cartObjects = GameObject.FindGameObjectsWithTag("Cart");

        // 각 카트를 순회하며 사용자의 선택된 카트인 경우 활성화하고, 그렇지 않은 경우 비활성화
        foreach (GameObject cartObject in cartObjects)
        {
            // 카트 컴포넌트 가져오기
            Cart cartComponent = cartObject.GetComponent<Cart>();

            if (cartComponent != null)
            {
                // 현재 순회 중인 카트가 사용자의 선택된 카트와 일치하는 경우
                if (cartComponent.id == selectedCart.id)
                {
                    // 카트 활성화
                    cartObject.SetActive(true);
                }
                else
                {
                    // 카트 비활성화
                    cartObject.SetActive(false);
                }
            }
        }
    }

}

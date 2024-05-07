using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject stage; // 단상 게임 오브젝트
    public User user; // 사용자 정보
    public CartData cartData;
    public CharacterData characterData;
    public ResourceLoader resourceLoader;

    public GameObject MyCart;
    public GameObject MyCharacter;


    void Start()
    {

        // 리소스 로더 초기화
        resourceLoader = GetComponent<ResourceLoader>();

        // 사용자 정보 초기화
        // InitializeUser();
        
        // 단상에 카트와 캐릭터 올리기
        PlaceCartAndCharacterOnStage();
    }

    // 사용자 정보 초기화 메서드
    private void InitializeUser()
    {
        // 사용자 정보 생성
        // user = new User();
        // user.InitializeUser(1, "Player1");

        // 사용자의 카트와 캐릭터 추가
        // user.AddCart(cartData.GetCartInfo(1)); // 예시로 카트 ID가 1인 카트 추가
        // user.AddCharacter(characterData.GetCharacterInfo(1)); // 예시로 캐릭터 ID가 1인 캐릭터 추가
    }

    private void PlaceCartAndCharacterOnStage()
    {
        // 카트 로드 및 배치
        GameObject cartPrefab = resourceLoader.LoadCartAsset("Cart_1");
        if (cartPrefab != null)
        {
            MyCart = Instantiate(cartPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("카트 프리팹 로드 실패 ㅠㅠ");
        }

        // 캐릭터 로드 및 배치
        GameObject characterPrefab = resourceLoader.LoadCharacterAsset("Character_1");
        if (characterPrefab != null)
        {
            MyCharacter = Instantiate(characterPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("캐릭터 프리팹 로드 실패 ㅠㅠ");
        }
    }

}

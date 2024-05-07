using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartSelectManager : MonoBehaviour
{
    public GameObject[] kartPrefabs;
    public CartUIManager cartUIManager;
    private readonly int size = 13;
    private int curIndex = 0;

    void Start()
    {
        ActivateCharacter(kartPrefabs[curIndex]);
    }


    // 오른쪽 버튼 클릭 시 발생하는 이벤트 메서드(다음 캐릭터로 변경)
    public void OnRightButtonClicked()
    {
        // 다음 캐릭터 인덱스로 변경
        curIndex = (curIndex + 1) % size;
        Debug.Log("현재 인덱스 : " + curIndex);

        // 변경된 캐릭터로 단상 위의 캐릭터 설정
        ActivateCharacter(kartPrefabs[curIndex]);
        cartUIManager.GetCartInfo(curIndex);
    }

    // 왼쪽 버튼 클릭 시 발생하는 이벤트 메서드(이전 캐릭터로 변경)
    public void OnLeftButtonClicked()
    {
        // 이전 캐릭터 인덱스로 변경
        curIndex = (curIndex - 1 + size) % size;
        Debug.Log("현재 인덱스 : " + curIndex);

        // 변경된 캐릭터로 단상 위의 캐릭터 설정
        ActivateCharacter(kartPrefabs[curIndex]);
        cartUIManager.GetCartInfo(curIndex);
    }

    
    // 캐릭터 활성화 및 비활성화 메서드
    private void ActivateCharacter(GameObject cartObject)
    {
        // 모든 캐릭터를 비활성화
        foreach (GameObject cart in kartPrefabs)
        {
            cart.SetActive(false);
        }

        // 전달받은 캐릭터를 활성화
        cartObject.SetActive(true);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    // 카트 프리팹 경로
    private string cartPrefabPath = "Assets/Resources/Prefabs/Carts";
    private string characterPrefabPath = "Assets/Resources/Prefabs/Characters";

    // 특정한 카트 에셋을 로드하는 함수
    public GameObject LoadCartAsset(string cartName)
    {
        string path = $"{cartPrefabPath}/{cartName}";
        Debug.Log("카트 프리팹 로드하기 : " + path);
        return Resources.Load<GameObject>(path);
    }

    // 특정한 캐릭터 에셋을 로드하는 함수
    public GameObject LoadCharacterAsset(string characterName)
    {
        string path = $"{characterPrefabPath}/{characterName}";
        Debug.Log("캐릭터 프리팹 로드하기 : " + path);
        return Resources.Load<GameObject>(path);
    }

    // 카트 에셋을 로드하는 함수
    public void LoadCartAssets()
    {
        // 카트 에셋 로드
        GameObject cartPrefab = Resources.Load<GameObject>(cartPrefabPath);

        // 로드한 카트 에셋을 사용
        Instantiate(cartPrefab, transform.position, Quaternion.identity);
    }

    // 캐릭터 에셋을 로드하는 함수
    public void LoadCharacterAssets()
    {
        // 캐릭터 에셋 로드
        GameObject characterPrefab = Resources.Load<GameObject>(characterPrefabPath);

        // 로드한 캐릭터 에셋을 사용
        Instantiate(characterPrefab, transform.position, Quaternion.identity);
    }

    // Start 함수에서 카트와 캐릭터 에셋을 로드
    private void Start()
    {
        // LoadCartAssets();
        // LoadCharacterAssets();
    }
}

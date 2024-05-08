using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCart : MonoBehaviour
{
    public GameObject player;
    public float distanceBehind = -0.8f; // 카메라가 플레이어 뒤에 있어야 하는 거리
    public float heightAbove = 0f;     // 카메라가 플레이어 대비 얼마나 높이 위치해야 하는지
    public float lookAtForwardOffset = 2f; // 카메라가 플레이어의 어느 정도 앞을 바라보게 할지
    public float rightOffset = 0.5f;     // 카메라를 오른쪽으로 이동시키는 거리

    void Start() {}

    // 1프레임마다 실행
    void LateUpdate()
    {
        // 플레이어의 위치에서 카메라 위치 계산
        Vector3 desiredPosition = player.transform.position - player.transform.forward * distanceBehind + Vector3.up * heightAbove;
        transform.position = desiredPosition;

        // 카메라가 플레이어를 바라보게 설정
        transform.LookAt(player.transform.position + player.transform.forward * lookAtForwardOffset);
        transform.rotation = player.transform.rotation;
    }
}

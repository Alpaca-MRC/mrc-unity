using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberAPI : APIManager
{
    private readonly string memberUrl = "member";

    // 회원 인증 요청
    public IEnumerator AuthRequest(MemberRequest.AuthRequestData request, Action<MemberResponse.GetMemberInfoData> callback)
    {
        string authRequestJson = JsonUtility.ToJson(request);
        yield return StartCoroutine(PostRequest(memberUrl + "/auth", authRequestJson, (response) =>
        {
            // 응답 처리
            if (!string.IsNullOrEmpty(response))
            {
                // 응답 데이터를 Deserialize
                MemberResponse.Root root = JsonUtility.FromJson<MemberResponse.Root>(response);
                
                if (root != null && root.status == 200)
                {
                    Debug.Log("회원 인증에 성공했습니다.");
                    // 응답 데이터를 콜백으로 전달
                    callback?.Invoke((MemberResponse.GetMemberInfoData)root.data);
                }
                else
                {
                    Debug.LogError("회원 인증에 실패했습니다.");
                }
            }
            else
            {
                Debug.LogError("회원 인증 응답이 비어 있습니다.");
            }
        }));
    }

    // 유저 정보 조회
    public IEnumerator GetMemberInfoRequest(string _username, Action<MemberResponse.GetMemberInfoData> callback)
    {
        // 유저 정보 조회 엔드포인트
        string endpoint = $"{memberUrl}/{_username}";

        // GET 요청 보내기
        yield return StartCoroutine(GetRequest(endpoint, (response) =>
        {
            // 응답 처리
            if (!string.IsNullOrEmpty(response))
            {
                // 응답 데이터를 Deserialize
                MemberResponse.Root root = JsonUtility.FromJson<MemberResponse.Root>(response);

                // 응답이 성공적으로 처리되었는지 확인
                if (root != null && root.status == 200)
                {
                    Debug.Log("유저 정보 조회에 성공했습니다.");

                    // 응답 데이터를 콜백으로 전달
                    callback?.Invoke((MemberResponse.GetMemberInfoData)root.data);
                }
                else
                {
                    Debug.LogError("유저 정보 조회에 실패했습니다.");
                    // TODO: 실패 시 처리 구현
                }
            }
            else
            {
                Debug.LogError("유저 정보 조회 응답이 비어 있습니다.");
                // TODO: 응답이 비어 있을 때 처리 구현
            }
        }));
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberResponse : ResponseData
{
    // 회원 인증



    // 유저 조회
    [Serializable]
    public class GetMemberInfoData : Data
    {
        public string username;
        public string nickname;
        public string selectedCartName;
        public List<string> carts;
        public AccountType accountType;
        public string iconUrl;
        public int coin;

    }
}

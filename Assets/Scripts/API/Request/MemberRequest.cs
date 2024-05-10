using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberRequest
{
    // 회원 인증
    [Serializable]
    public class AuthRequestData
    {
        public string userName;
        public string nickname;
        public AccountType accountType;
    }
}

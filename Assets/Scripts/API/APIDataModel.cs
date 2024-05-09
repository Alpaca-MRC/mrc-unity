using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIForm : MonoBehaviour
{
    [Serializable]
    public class ChangeCartResponse
    {
        public int status;
        public string code;
        public string message;
        public Data data;
    }

    [Serializable]
    public class Data
    {

    }
}

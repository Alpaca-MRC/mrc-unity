using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResponseData
{
    [Serializable]
    public class Root
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

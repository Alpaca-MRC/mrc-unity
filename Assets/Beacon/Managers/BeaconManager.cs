using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BeaconManager
{
    public string uuid;
    public string major;
    public string minor;
    public string name;
    public string txpower;
    public int rssi;
    public float distance;
    public int battery;
    public bool inRange;
    public bool connetable;
    public string macAddress;
    public string serviceData;
    public long addTime;
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ReceiveVideo : MonoBehaviour
{
    public string serverIP = "192.168.137.197";
    public int port = 8080;
    public GameObject Cube;
    private UdpClient client;
    private IPEndPoint serverEndPoint;
    private Thread receiveThread;
    private Texture2D tex;
    private Queue<Action> mainThreadActions = new Queue<Action>();
    private bool isTryingToReconnect = false;
    private bool isConnected = false;

    void Start()
    {
        InitializeConnection();
    }

    void InitializeConnection()
    {

        ConnectToServer();
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
        receiveThread.Start();
    }

    void Update()
    {
        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }

        if (tex != null)
        {
            Cube.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }

    private void ReceiveData()
    {
        while (true)
        {
            if (isTryingToReconnect)
            {
                Thread.Sleep(1000); // 재연결 시도 중 일시적으로 데이터 수신을 중단
                continue;
            }

            try
            {
                byte[] imageSizeData = client.Receive(ref serverEndPoint);
                int imageSize = BitConverter.ToInt32(imageSizeData, 0);
                if (imageSize == 0) continue;
                byte[] imageData = ReceiveFullImage(imageSize);
                if (imageData != null)
                {
                    lock (mainThreadActions)
                    {
                        mainThreadActions.Enqueue(() => tex.LoadImage(imageData));
                    }
                }
                else
                {
                    Debug.LogWarning("이미지 데이터 수신 실패. 다음 데이터를 기다립니다.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"데이터 수신 중 예외 발생: {e.Message}");
                AttemptReconnect();
            }
        }
    }

    private byte[] ReceiveFullImage(int imageSize)
    {
        int received = 0;
        List<byte> imageData = new List<byte>();

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        
        while (received < imageSize)
        {
            if (stopwatch.ElapsedMilliseconds > 300)
            {
                Debug.LogError("시간 초과");
                return null;
            }
            try
            {
                byte[] packetData = client.Receive(ref serverEndPoint);
                received += packetData.Length;
                imageData.AddRange(packetData);
            }
            catch (SocketException e)
            {
                Debug.LogError($"데이터 수신 중 예외 발생: {e.Message}");
                return null; 
            }
        }

        return imageData.ToArray();
    }

    private void AttemptReconnect()
    {
        if (!isTryingToReconnect)
        {
            isTryingToReconnect = true;
            Debug.LogWarning("Connection lost. Attempting to reconnect...");

            try
            {
                CloseClient();
                ConnectToServer();
                Debug.Log("Reconnected to server successfully.");
                isTryingToReconnect = false;
                return;
            }
            catch (Exception e)
            {
                Debug.LogError("Reconnection attemptfailed" + e);
                Thread.Sleep(2000); 
            }
        
            Debug.LogError("Failed to reconnect after multiple attempts.");
            isTryingToReconnect = false;
        }
    }

    private void ConnectToServer()
    {
        tex = new Texture2D(1920, 1080);
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
        client = new UdpClient();
        client.Connect(serverEndPoint);
        isConnected = true;
        byte[] initialData = System.Text.Encoding.UTF8.GetBytes("Unity connected!");
        client.Send(initialData, initialData.Length);
    }

    void OnDestroy()
    {
        Debug.Log("destroying");
        if (isConnected)
        {
            byte[] disconnectMessage = System.Text.Encoding.UTF8.GetBytes("disconnect");
            client.Send(disconnectMessage, disconnectMessage.Length);
            isConnected = false;
            Debug.Log("send close message");
        }
        receiveThread?.Abort();
        client?.Close();
    }

    void CloseClient() 
    {
        Debug.Log("closing");
        if (isConnected)
        {
            byte[] disconnectMessage = System.Text.Encoding.UTF8.GetBytes("disconnect");
            client.Send(disconnectMessage, disconnectMessage.Length);
            isConnected = false;
            Debug.Log("send close message");
        }
        client?.Close();
    }
}

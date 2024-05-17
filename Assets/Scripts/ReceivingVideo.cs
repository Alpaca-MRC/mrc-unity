using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ReceivingVideo : MonoBehaviour
{
    public string serverIP = "192.168.137.197";
    public int port = 9090;
    public GameObject Cube;
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private Texture2D tex;
    private Queue<Action> mainThreadActions = new Queue<Action>();

    void Start()
    {
        tex = new Texture2D(1920, 1080);
        ConnectToServer();
    }

    void Update()
    {
        // 메인 스레드에서 큐에 있는 작업을 실행
        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }

        // 메인 스레드에서 텍스처 업데이트
        if (tex != null)
        {
            Cube.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, port);
            stream = client.GetStream();
            receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 연결 실패: {e.Message}");
        }
    }

    private void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        MemoryStream ms = new MemoryStream();

        while (client.Connected)
        {
            try
            {
                // 이미지 크기 읽기
                int bytesRead = stream.Read(buffer, 0, 4);
                if (bytesRead <= 0) break;

                int imageLength = BitConverter.ToInt32(buffer, 0);
                if (imageLength == 0) break;

                // 이미지 데이터 읽기
                ms.SetLength(0);
                while (imageLength > 0)
                {
                    bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, imageLength));
                    ms.Write(buffer, 0, bytesRead);
                    imageLength -= bytesRead;
                }

                byte[] imageData = ms.ToArray();

                // 메인 스레드에서 텍스처 업데이트 작업을 큐에 추가
                lock (mainThreadActions)
                {
                    mainThreadActions.Enqueue(() => tex.LoadImage(imageData));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"데이터 수신 오류: {e.Message}");
                break;
            }
        }

        client.Close();
    }

    void OnDestroy()
    {
        receiveThread?.Abort();
        client?.Close();
    }
}
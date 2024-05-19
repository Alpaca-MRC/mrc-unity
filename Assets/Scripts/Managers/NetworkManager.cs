using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading;
using System.IO;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private UdpClient controlUdpClient, cameraUdpClient;
    private IPEndPoint controlServerEndPoint, cameraServerEndPoint;
    private GameObject Cube; // 비디오 스트리밍 오브젝트 
    private string serverIp = "192.168.137.193"; // 라즈베리파이 서버의 IP 주소
    private int controlPort = 25001; // 라즈베리파이 제어 서버의 포트 번호
    private int cameraPort = 8080; // 라즈베리파이 카메라 서버의 포트 번호

    // 컨트롤러 값 관리
    public InputActionAsset inputActionsAsset;
    private float horizontalInput, isAPressed, isBPressed;
    private float updatedHorizontalInput, updatedIsAPressed, updatedIsBPressed;
    string commandMessage;
    private bool isControllerInputEnabled;
    private bool isTryingToReconnect = false;

    private bool isConnected = false;

    // 비디오 값 관리
    public Thread receiveThread;
    private Texture2D tex;
    private Queue<Action> mainThreadActions = new Queue<Action>();

    // 소켓 인스턴스 생성
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (isControllerInputEnabled)
        {
            CheckControllerInput();
            CheckCameraData();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (isConnected)
        {
            isConnected = false;
            receiveThread?.Abort();
            tcpClient?.Close();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isControllerInputEnabled = scene.name == "ARScene"; // MR 씬에서만 컨트롤러 값 전송
        if (scene.name == "ARScene")
        {
            Cube = GameObject.Find("Cube");
            StartToReceiveData();
            if (Cube == null)
            {
                Debug.LogError("Cube not found in the scene.");
            }
        }
    }

    public void ConnectToControlUDPServer()
    {
        controlUdpClient = new UdpClient();
        controlServerEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), controlPort);
        Debug.Log("Connected to UDP Server");
    }

    public void ConnectToCameraUDPServer()
    {
        tex = new Texture2D(1280, 720);
        cameraServerEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), cameraPort);
        cameraUdpClient = new UdpClient();
        cameraUdpClient.Connect(cameraServerEndPoint);
    }

    public void StartToReceiveData()
    {
        // isConnected = true;
        // byte[] initialData = System.Text.Encoding.UTF8.GetBytes("Unity connected!");
        // cameraUdpClient.Send(initialData, initialData.Length);
        // receiveThread = new Thread(ReceiveData) { IsBackground = true };
        // receiveThread.Start();
        receiveThread = new Thread(ReceiveTcpData);
        receiveThread.Start();
    }

    public void SendMessageToUDPServer(string message)
    {
        if (controlUdpClient != null)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            controlUdpClient.Send(data, data.Length, controlServerEndPoint);
            Debug.Log("UDP message sent: " + message);
        }
    }

    public void ReceiveData()
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
                byte[] imageSizeData = cameraUdpClient.Receive(ref cameraServerEndPoint);
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

    private void AttemptReconnect()
    {
        if (!isTryingToReconnect)
        {
            isTryingToReconnect = true;
            Debug.LogWarning("Connection lost. Attempting to reconnect...");

            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    CloseClient();
                    ConnectToCameraUDPServer();
                    StartToReceiveData();
                    isTryingToReconnect = false;
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Reconnection attempt {attempt + 1} failed: {e.Message}");
                    Thread.Sleep(2000); // 재연결 시도 간 대기
                }
            }

            Debug.LogError("Failed to reconnect after multiple attempts.");
            isTryingToReconnect = false;
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
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                Debug.LogError("1초가 초과되었습니다.");
                return null;
            }
            try
            {
                byte[] packetData = cameraUdpClient.Receive(ref cameraServerEndPoint);
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

    private void CheckControllerInput()
    {
        // 현재 컨트롤러 값 확인
        updatedIsAPressed = inputActionsAsset.actionMaps[10].actions[0].ReadValue<float>();
        updatedIsBPressed = inputActionsAsset.actionMaps[10].actions[1].ReadValue<float>();
        updatedHorizontalInput = (float) Math.Truncate(Input.GetAxis("Horizontal") * 100 / 20); 
        
        // 값 변경 확인
        if ((updatedHorizontalInput != horizontalInput) || (updatedIsAPressed != isAPressed) || (updatedIsBPressed != isBPressed)) 
        {
            commandMessage = $"{updatedHorizontalInput}/{updatedIsAPressed}/{updatedIsBPressed}_";
            SendMessageToUDPServer(commandMessage);

            // 값 갱신
            horizontalInput = updatedHorizontalInput;
            isAPressed = updatedIsAPressed;
            isBPressed = updatedIsBPressed;
        }
    }

    private void CheckCameraData()
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

    private void CloseClient() 
    {
        if (isConnected)
        {
            byte[] disconnectMessage = System.Text.Encoding.UTF8.GetBytes("disconnect");
            cameraUdpClient.Send(disconnectMessage, disconnectMessage.Length);
            isConnected = false;
            Debug.Log("send close message");
        }
        cameraUdpClient?.Close();
    }

    public void CloseConnections()
    {
        CloseClient();
        SendMessageToUDPServer("close_");
        controlUdpClient?.Close();
        Application.Quit();
    }

    public async Task ConnectToTcpServer()
    {
        tex = new Texture2D(1280, 720);
        try
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(serverIp, cameraPort);
            tcpStream = tcpClient.GetStream();
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 연결 실패: {e}");
            throw;
        }
    }

    // public async Task ConnectToTCPServer()
    // {
    //     try
    //     {
    //         tcpClient = new TcpClient();
    //         await tcpClient.ConnectAsync(serverIp, controlPort);
    //         tcpStream = tcpClient.GetStream();
    //         Debug.Log("Connected to TCP Server");
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError("TCP connection error: " + e);
    //         throw;
    //     }
    // }

    private void ReceiveTcpData()
    {
        byte[] buffer = new byte[4096];
        MemoryStream ms = new MemoryStream();

        while (tcpClient.Connected)
        {
            Debug.Log("아직 TCP 서버 연결됐어!");
            try
            {
                // 이미지 크기 읽기
                int bytesRead = tcpStream.Read(buffer, 0, 4);
                if (bytesRead <= 0) break;

                int imageLength = BitConverter.ToInt32(buffer, 0);
                if (imageLength == 0) break;

                // 이미지 데이터 읽기
                ms.SetLength(0);
                while (imageLength > 0)
                {
                    bytesRead = tcpStream.Read(buffer, 0, Math.Min(buffer.Length, imageLength));
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

        tcpClient.Close();
    }
}


    // public void SendMessageToTCPServer(string message)
    // {
    //     if (tcpStream != null)
    //     {
    //         byte[] data = Encoding.ASCII.GetBytes(message);
    //         tcpStream.Write(data, 0, data.Length);
    //         Debug.Log("TCP message sent: " + message);
    //     }
    //     // 서버로부터 응답 수신
    //     // byte[] responseData = new byte[1024];
    //     // int bytes = tcpStream.Read(responseData, 0, responseData.Length);
    //     // string response = Encoding.ASCII.GetString(responseData, 0, bytes);

    //     // if (response != null && response != "")
    //     // {
    //     //     Debug.Log(response);
    //     // }
    // }

    // public async Task ConnectToTCPServer()
    // {
    //     try
    //     {
    //         tcpClient = new TcpClient();
    //         await tcpClient.ConnectAsync(serverIp, controlPort);
    //         tcpStream = tcpClient.GetStream();
    //         Debug.Log("Connected to TCP Server");
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError("TCP connection error: " + e);
    //         throw;
    //     }
    // }

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Net.Sockets;
// using System.Threading;
// using UnityEngine;

// public class streaming : MonoBehaviour
// {
//     public string serverIP = "192.168.137.197";
//     public int port = 9090;
//     public GameObject Cube;
//     private TcpClient client;
//     private NetworkStream stream;
//     private Thread receiveThread;
//     private Texture2D tex;
//     private Queue<Action> mainThreadActions = new Queue<Action>();

//     void Start()
//     {
//         tex = new Texture2D(1920, 1080);
//         ConnectToServer();
//     }

//     void Update()
//     {
//         // 메인 스레드에서 큐에 있는 작업을 실행
//         lock (mainThreadActions)
//         {
//             while (mainThreadActions.Count > 0)
//             {
//                 mainThreadActions.Dequeue().Invoke();
//             }
//         }

//         // 메인 스레드에서 텍스처 업데이트
//         if (tex != null)
//         {
//             Cube.GetComponent<Renderer>().material.mainTexture = tex;
//         }
//     }

//     private void ConnectToServer()
//     {
//         try
//         {
//             client = new TcpClient(serverIP, port);
//             stream = client.GetStream();
//             receiveThread = new Thread(ReceiveData);
//             receiveThread.Start();
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"서버 연결 실패: {e.Message}");
//         }
//     }

//     private void ReceiveData()
//     {
//         byte[] buffer = new byte[4096];
//         MemoryStream ms = new MemoryStream();

//         while (client.Connected)
//         {
//             try
//             {
//                 // 이미지 크기 읽기
//                 int bytesRead = stream.Read(buffer, 0, 4);
//                 if (bytesRead <= 0) break;

//                 int imageLength = BitConverter.ToInt32(buffer, 0);
//                 if (imageLength == 0) break;

//                 // 이미지 데이터 읽기
//                 ms.SetLength(0);
//                 while (imageLength > 0)
//                 {
//                     bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, imageLength));
//                     ms.Write(buffer, 0, bytesRead);
//                     imageLength -= bytesRead;
//                 }

//                 byte[] imageData = ms.ToArray();

//                 // 메인 스레드에서 텍스처 업데이트 작업을 큐에 추가
//                 lock (mainThreadActions)
//                 {
//                     mainThreadActions.Enqueue(() => tex.LoadImage(imageData));
//                 }
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"데이터 수신 오류: {e.Message}");
//                 break;
//             }
//         }

//         client.Close();
//     }

//     void OnDestroy()
//     {
//         receiveThread?.Abort();
//         client?.Close();
//     }
// }
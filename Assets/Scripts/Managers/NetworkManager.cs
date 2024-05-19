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

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private UdpClient controlUdpClient, cameraUdpClient;
    private IPEndPoint controlServerEndPoint, cameraServerEndPoint;
    public GameObject Cube; // 비디오 스트리밍 오브젝트 
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
            // CheckControllerInput();
            // CheckCameraData();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (isConnected)
        {
            byte[] disconnectMessage = System.Text.Encoding.UTF8.GetBytes("disconnect");
            cameraUdpClient.Send(disconnectMessage, disconnectMessage.Length);
            isConnected = false;
        }
        receiveThread?.Abort();
        cameraUdpClient?.Close();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isControllerInputEnabled = scene.name == "ARScene"; // MR 씬에서만 컨트롤러 값 전송
        if (scene.name == "ARScene")
        {
            // Cube = GameObject.Find("Cube");
            // StartToReceiveData();
            // if (Cube == null)
            // {
            //     Debug.LogError("Cube not found in the scene.");
            // }
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
        isConnected = true;
        byte[] initialData = System.Text.Encoding.UTF8.GetBytes("Unity connected!");
        cameraUdpClient.Send(initialData, initialData.Length);
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
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

    // public void CloseConnections()
    // {
    //     SendMessageToUDPServer("close_");
    //     controlUdpClient?.Close();
    //     // Application.Quit();
    // }
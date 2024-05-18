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

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private UdpClient udpClient;
    private IPEndPoint serverEndPoint;
    private string serverIp = "192.168.137.193"; // 라즈베리파이 서버의 IP 주소
    private int serverPort = 25001; // 라즈베리파이 서버의 포트 번호

    // 컨트롤러 값 관리
    public InputActionAsset inputActionsAsset;
    private float horizontalInput, isAPressed, isBPressed;
    private float updatedHorizontalInput, updatedIsAPressed, updatedIsBPressed;
    string commandMessage;
    private bool isControllerInputEnabled;

    void Start()
    {
        isControllerInputEnabled = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (isControllerInputEnabled)
        {
            CheckControllerInput();
        }
    }

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isControllerInputEnabled = true;
        // isControllerInputEnabled = scene.name == "MainScene"; // MR 씬에서만 컨트롤러 값 전송
    }

    public void ConnectToUDPServer()
    {
        udpClient = new UdpClient();
        serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        Debug.Log("Connected to UDP Server");
    }

    public async Task ConnectToTCPServer()
    {
        try
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(serverIp, serverPort);
            tcpStream = tcpClient.GetStream();
            Debug.Log("Connected to TCP Server");
        }
        catch (Exception e)
        {
            Debug.LogError("TCP connection error: " + e);
            throw;
        }
    }

    public void SendMessageToUDPServer(string message)
    {
        if (udpClient != null)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            udpClient.Send(data, data.Length, serverEndPoint);
            Debug.Log("UDP message sent: " + message);
        }
    }

    public void SendMessageToTCPServer(string message)
    {
        if (tcpStream != null)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            tcpStream.Write(data, 0, data.Length);
            Debug.Log("TCP message sent: " + message);
        }
        // 서버로부터 응답 수신
        // byte[] responseData = new byte[1024];
        // int bytes = tcpStream.Read(responseData, 0, responseData.Length);
        // string response = Encoding.ASCII.GetString(responseData, 0, bytes);

        // if (response != null && response != "")
        // {
        //     Debug.Log(response);
        // }
    }

    public void CloseConnections()
    {
        SendMessageToUDPServer("close_");
        SendMessageToTCPServer("close");
        udpClient?.Close();
        tcpStream?.Close();
        tcpClient?.Close();
        Debug.Log("Connections closed");
// #if UNITY_EDITOR
//     UnityEditor.EditorApplication.isPlaying = false;
// #else
//         // Application.Quit();
// #endif
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
}


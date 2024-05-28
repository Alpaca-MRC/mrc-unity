using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using System.IO;

public class ConnectManager : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream tcpStream;
    private UdpClient controlUdpClient;
    private IPEndPoint controlServerEndPoint;
    public GameObject Cube; // 비디오 스트리밍 오브젝트 
    private string serverIp = "192.168.137.193"; // 라즈베리파이 서버의 IP 주소
    private int controlPort = 25001; // 라즈베리파이 제어 서버의 포트 번호
    private int cameraPort = 8080; // 라즈베리파이 카메라 서버의 포트 번호

    // 컨트롤러 값 관리
    public InputActionAsset inputActionsAsset;
    private float horizontalInput, isAPressed, isBPressed, isXPressed;
    private float updatedHorizontalInput, updatedIsAPressed, updatedIsBPressed;
    string commandMessage;

    // 비디오 값 관리
    public Thread receiveThread;
    private Texture2D tex;
    private Queue<Action> mainThreadActions = new Queue<Action>();

    void Start()
    {
        // 객체의 렌더러 컴포넌트를 찾습니다
        Renderer renderer = Cube.GetComponent<Renderer>();
        renderer.enabled = false;
        ConnectToControlUDPServer();
        ConnectToTcpServer();
    }

    void Update()
    {
        CheckTriggerInput();
        CheckControllerInput();
        CheckCameraData();
    }

    void OnDestroy()
    {
        SendMessageToUDPServer("close_");
        controlUdpClient?.Close();
        receiveThread?.Abort();
        tcpClient?.Close();
    }
    private void ConnectToControlUDPServer()
    {
        controlUdpClient = new UdpClient();
        controlServerEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), controlPort);
    }

    private void ConnectToTcpServer()
    {
        tex = new Texture2D(1280, 720);
        try
        {
            tcpClient = new TcpClient(serverIp, cameraPort);
            tcpStream = tcpClient.GetStream();
            receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"서버 연결 실패: {e.Message}");
        }
    }

    public void SendMessageToUDPServer(string message)
    {
        if (controlUdpClient != null)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            controlUdpClient.Send(data, data.Length, controlServerEndPoint);
        }
    }

    private void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        MemoryStream ms = new MemoryStream();

        while (tcpClient.Connected)
        {
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
    private float cooldownTime = 0.5f;
    private void CheckTriggerInput()
    {   
        cooldownTime -= Time.deltaTime;
        if (cooldownTime > 0) return;
    
        isXPressed = inputActionsAsset.actionMaps[9].actions[0].ReadValue<float>();
        if (isXPressed == 1)
        {
            // 객체의 렌더러 컴포넌트를 찾습니다
            Renderer renderer = Cube.GetComponent<Renderer>();
            // 렌더러가 있으면, 현재 활성화 상태를 반전시킵니다
            if (renderer != null)
            {
                renderer.enabled = !renderer.enabled;
                cooldownTime = 0.5f;
            }
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
}

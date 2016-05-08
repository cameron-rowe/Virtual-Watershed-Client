using UnityEngine;
using System.Collections;

public class ServerNetwork : MonoBehaviour {
    public GameObject player;
    private int port = 25000;
    private int playerCount = 0;
    private string _messageLog = "";

    public void Awake()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
            Network.InitializeServer(10, port, false);
    }

    public void Update()
    {

    }
    public void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Server)
        {
            //GUI.Label(new Rect(100, 100, 100, 25), "Server");
            //GUI.Label(new Rect(100, 125, 100, 25), "Clients attached: " + Network.connections.Length);

            if (GUI.Button(new Rect(80, 75, 100, 25), "Quit server"))
            {
                Network.Disconnect();
                Application.Quit();
            }
            if (GUI.Button(new Rect(80, 100, 100, 25), "Jump"))
            {
                SendToClientJump();
                player.GetComponent<FPSInputController>().Jump();
            }
            if (GUI.Button(new Rect(80, 125, 100, 25), "Fly"))
            {
                SendToClientFly();
                player.GetComponent<toggleScripts>().toggleFlight();
            }

        }
        //GUI.TextArea(new Rect(275, 100, 300, 300), _messageLog);
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        AskClientForInfo(player);
    }

    void AskClientForInfo(NetworkPlayer player)
    {
        GetComponent<NetworkView>().RPC("SetPlayerInfo", player, player);
    }

    [RPC]
    void ReceiveFromClientJump(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        player.GetComponent<FPSInputController>().Jump();
    }
    [RPC]
    void ReceiveFromClientFly(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        player.GetComponent<toggleScripts>().toggleFlight();
    }

    [RPC]
    void ReceiveFromClientForward(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        //player.GetComponent<FPSInputController>().MoveForward();
    }

    [RPC]
    void ReceiveFromClientBackward(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
       // player.GetComponent<FPSInputController>().MoveBackward();
    }

    [RPC]
    void ReceiveFromClientForwardBackwardStop(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
       // player.GetComponent<FPSInputController>().ResetForwardBackwardMovement();
    }

    [RPC]
    void ReceiveFromClientLeft(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        //player.GetComponent<FPSInputController>().MoveLeft();
    }

    [RPC]
    void ReceiveFromClientRight(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        //player.GetComponent<FPSInputController>().MoveRight();
    }

    [RPC]
    void ReceiveFromClientLeftRightStop(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        //player.GetComponent<FPSInputController>().ResetLeftRightMovement();
    }

    [RPC]
    void ReceiveFromClientCamera(string someInfo, Vector3 cameraPos, Quaternion cameraRot)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        player.transform.position = cameraPos;
        player.transform.rotation = cameraRot;
        player.transform.Rotate(new Vector3(0, 90, 0));
    }

    string someInfo = "Server: hello client";
    [RPC]
    void SendToClientJump()
    {
        GetComponent<NetworkView>().RPC("ReceiveFromServerJump", RPCMode.Others, someInfo);
    }
    [RPC]
    void SendToClientFly()
    {
        GetComponent<NetworkView>().RPC("ReceiveFromServerFly", RPCMode.Others, someInfo);
    }

    // fix RPC errors
    [RPC]
    public void ClientToServerJump() { }
    [RPC]
    public void ClientToServerFly() { }
    [RPC]
    void SetPlayerInfo(NetworkPlayer player) { }
    [RPC]
    void ReceiveFromServerJump(string someInfo) { }
    [RPC]
    void ReceiveFromServerFly(string someInfo) { }
    [RPC]
    public void ClientToServerForward() { }
    [RPC]
    public void ClientToServerBackward() { }
    [RPC]
    public void ClientToServerForwardBackwardStop() { }
    [RPC]
    public void ClientToServerLeft() { }
    [RPC]
    public void ClientToServerRight() { }
    [RPC]
    public void ClientToServerLeftRightStop() { }
    [RPC]
    public void ClientToServerCamera() { }
}

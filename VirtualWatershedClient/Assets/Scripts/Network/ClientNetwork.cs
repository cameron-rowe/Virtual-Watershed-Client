using UnityEngine;
using System.Collections;

public class ClientNetwork : MonoBehaviour
{
    public GameObject player;
    public string serverIP = "134.197.66.31";
    public int port = 25000;
    private string _messageLog = "";
    string someInfo = "";
    private NetworkPlayer _myNetworkPlayer;

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (GUI.Button(new Rect(80, 75, 100, 25), "Connect"))
            {
                Network.Connect(serverIP, port);
            }
        }
        else
        {
            if (Network.peerType == NetworkPeerType.Client)
            {
                if (GUI.Button(new Rect(80, 90, 100, 25), "Logut"))
                    Network.Disconnect();
/*
                if (GUI.Button(new Rect(80, 115, 100, 25), "Jump"))
                {
                    player.GetComponent<FPSInputController>().Jump();
                    ClientToServerJump();
                }
                if (GUI.Button(new Rect(80, 140, 100, 25), "Fly"))
                {
                    player.GetComponent<toggleScripts>().toggleFlight();
                    ClientToServerFly();
                }
*/
            }
        }

        //GUI.TextArea(new Rect(250, 100, 300, 300), _messageLog);
    }

    private float updateDT = 0f;
    void Update()
    {
        updateDT += Time.deltaTime;
        if (Network.peerType == NetworkPeerType.Client  && updateDT > 0.05f )
        {
            ClientToServerCamera();
            updateDT = 0f;
        }
    }

    [RPC]
    public void ClientToServerJump()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Jump";
        GetComponent<NetworkView>().RPC("ReceiveFromClientJump", RPCMode.Server, someInfo);

    }
    [RPC]
    public void ClientToServerFly()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Fly";
        GetComponent<NetworkView>().RPC("ReceiveFromClientFly", RPCMode.Server, someInfo);

    }
    [RPC]
    void SetPlayerInfo(NetworkPlayer player)
    {
        _myNetworkPlayer = player;
        someInfo = "Player setted";
        GetComponent<NetworkView>().RPC("ReceiveFromClientJump", RPCMode.Server, someInfo);
    }

    [RPC]
    void ReceiveFromServerJump(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        player.GetComponent<FPSInputController>().Jump();
    }
    [RPC]
    void ReceiveFromServerFly(string someInfo)
    {
        _messageLog += someInfo + "\n";
        Debug.Log(_messageLog);
        player.GetComponent<toggleScripts>().toggleFlight();
    }

    void OnConnectedToServer()
    {
        _messageLog += "Connected to server" + "\n";
        Debug.Log(_messageLog);
    }
    void OnDisconnectedToServer()
    {
        _messageLog += "Disco from server" + "\n";
        Debug.Log(_messageLog);
    }

    [RPC]
    public void ClientToServerForward()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Forward";
        GetComponent<NetworkView>().RPC("ReceiveFromClientForward", RPCMode.Server, someInfo);
    }

    [RPC]
    public void ClientToServerBackward()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Backward";
        GetComponent<NetworkView>().RPC("ReceiveFromClientBackward", RPCMode.Server, someInfo);
    }

    [RPC]
    public void ClientToServerForwardBackwardStop()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": ForwardBackwardStop";
        GetComponent<NetworkView>().RPC("ReceiveFromClientForwardBackwardStop", RPCMode.Server, someInfo);
    }

    [RPC]
    public void ClientToServerLeft()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Left";
        GetComponent<NetworkView>().RPC("ReceiveFromClientLeft", RPCMode.Server, someInfo);
    }

    [RPC]
    public void ClientToServerRight()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Right";
        GetComponent<NetworkView>().RPC("ReceiveFromClientRight", RPCMode.Server, someInfo);
    }

    [RPC]
    public void ClientToServerLeftRightStop()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": LeftRightStop";
        GetComponent<NetworkView>().RPC("ReceiveFromClientLeftRightStop", RPCMode.Server, someInfo);
    }

    [RPC]
    public void ClientToServerCamera()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Camera";
        GetComponent<NetworkView>().RPC("ReceiveFromClientCamera", RPCMode.Server, someInfo, player.transform.position, player.transform.rotation);
    }
    // fix RPC errors
    [RPC]
    void ReceiveFromClientJump(string someInfo) { }
    [RPC]
    void ReceiveFromClientFly(string someInfo) { }
    [RPC]
    void SendToClientJump(string someInfo) { }
    [RPC]
    void ReceiveFromClientForward(string someInfo) { }
    [RPC]
    void ReceiveFromClientBackward(string someInfo) { }
    [RPC]
    void ReceiveFromClientForwardBackwardStop(string someInfo) { }
    [RPC]
    void ReceiveFromClientLeft(string someInfo) { }
    [RPC]
    void ReceiveFromClientRight(string someInfo) { }
    [RPC]
    void ReceiveFromClientLeftRightStop(string someInfo) { }
    [RPC]
    void ReceiveFromClientCamera(string someInfo, Vector3 cameraPos, Quaternion cameraRot) { }
}
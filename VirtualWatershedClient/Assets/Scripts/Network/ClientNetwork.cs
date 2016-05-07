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
            }
        }

        //GUI.TextArea(new Rect(250, 100, 300, 300), _messageLog);
    }

    [RPC]
    void ClientToServerJump()
    {
        someInfo = "Client " + _myNetworkPlayer.guid + ": Jump";
        GetComponent<NetworkView>().RPC("ReceiveFromClientJump", RPCMode.Server, someInfo);

    }
    [RPC]
    void ClientToServerFly()
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

    // fix RPC errors
    [RPC]
    void ReceiveFromClientJump(string someInfo) { }
    [RPC]
    void ReceiveFromClientFly(string someInfo) { }
    [RPC]
    void SendToClientJump(string someInfo) { }
}
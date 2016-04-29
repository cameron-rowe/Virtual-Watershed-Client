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
            GUI.Label(new Rect(100, 100, 150, 25), "Server");
            GUI.Label(new Rect(100, 125, 150, 25), "Clients attached: " + Network.connections.Length);

            if (GUI.Button(new Rect(100, 150, 150, 25), "Quit server"))
            {
                Network.Disconnect();
                Application.Quit();
            }
            if (GUI.Button(new Rect(100, 175, 150, 25), "Send hi to client"))
            {
                SendInfoToClient();
                player.GetComponent<FPSInputController>().Jump();
            }
                
        }
        GUI.TextArea(new Rect(275, 100, 300, 300), _messageLog);
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
    void ReceiveInfoFromClient(string someInfo)
    {
        _messageLog += someInfo + "\n";

        player.GetComponent<FPSInputController>().Jump();
    }

    string someInfo = "Server: hello client";
    [RPC]
    void SendInfoToClient()
    {
        GetComponent<NetworkView>().RPC("ReceiveInfoFromServer", RPCMode.Others, someInfo);
    }

    // fix RPC errors
    [RPC]
    void SendInfoToServer() { }
    [RPC]
    void SetPlayerInfo(NetworkPlayer player) { }
    [RPC]
    void ReceiveInfoFromServer(string someInfo) { }
}

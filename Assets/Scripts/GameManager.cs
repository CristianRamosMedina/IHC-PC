using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public string playerPrefabName = "Cube";

    void Start()
    {
        Debug.Log("START: Conectando a Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED TO MASTER. Uniéndose/creando sala...");
        PhotonNetwork.JoinOrCreateRoom("SalaPrincipal", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("JOINED ROOM. Instanciando jugador...");
        Vector3 spawnPos = new Vector3(Random.Range(80, 100), 300, Random.Range(-4, 4));
        PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("DISCONNECTED: " + cause);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("JOIN ROOM FAILED: " + message);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("CREATED ROOM.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("PLAYER JOINED: " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("PLAYER LEFT: " + otherPlayer.NickName);
    }
}
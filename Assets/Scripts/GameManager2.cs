using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

public class GameManager2 : MonoBehaviourPunCallbacks
{
    public static GameManager2 Instance;

    void Awake() { Instance = this; }
    
    public string playerPrefabName = "Cube2";
    public Camera playerCameraPrefab;

    public static Dictionary<int, GameObject> playerCubes = new Dictionary<int, GameObject>();

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
        Vector3 spawnPos = new Vector3(4, 30, -4);
        GameObject myCube = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
        playerCubes[PhotonNetwork.LocalPlayer.ActorNumber] = myCube;

        // Creamos la cámara para este jugador
        Camera cam = Instantiate(playerCameraPrefab);

        // Espera un frame para que todos estén registrados antes de asignar roles y cámaras
        StartCoroutine(SetupRolesAndCamera(myCube, cam));
    }

    private IEnumerator SetupRolesAndCamera(GameObject myCube, Camera cam)
    {
        yield return null;

        // Espera que al menos 2 jugadores estén (para ver si hay piloto y armero)
        while (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            cam.GetComponent<FollowPlayer>().target = myCube.transform;
            myCube.GetComponent<NetworkPlayer2>().SetRole(true);
            Debug.Log("Esperando a otro jugador para definir roles...");
            yield return null;
        }

        // Hay al menos 2 jugadores: define piloto y armero por ActorNumber más bajo
        int pilotActorNumber = int.MaxValue;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber < pilotActorNumber)
                pilotActorNumber = player.ActorNumber;
        }

        bool soyPiloto = (PhotonNetwork.LocalPlayer.ActorNumber == pilotActorNumber);
        myCube.GetComponent<NetworkPlayer2>().SetRole(soyPiloto);

        if (soyPiloto)
        {
            // Piloto: su cámara sigue el cubo normal
            cam.GetComponent<FollowPlayer>().target = myCube.transform;
            Debug.Log("Soy el PILOTO.");
        }
        else
        {
            // ARMERO: busca el piloto por la red y se monta como hijo
            NetworkPlayer2 piloto = null;
            float timeout = 5f;
            while (piloto == null && timeout > 0)
            {
                foreach (var candidate in GameObject.FindObjectsOfType<NetworkPlayer2>())
                {
                    var view = candidate.GetComponent<PhotonView>();
                    if (view != null && view.Owner != null && view.Owner.ActorNumber == pilotActorNumber)
                    {
                        piloto = candidate;
                        break;
                    }
                }
                yield return null;
                timeout -= Time.deltaTime;
            }
            if (piloto != null)
            {
                myCube.transform.SetParent(piloto.transform, true);
                Debug.Log("Soy el ARMERO, me monto dentro del PILOTO.");
            }
            else
            {
                Debug.LogWarning("No encontré el cubo del piloto a tiempo.");
            }

            // Configura la cámara para el armero
            cam.GetComponent<FollowPlayer>().target = myCube.transform; // Puede ser un offset específico si quieres
            // Añade el FreeLookCamera SOLO a la cámara del armero
            cam.gameObject.AddComponent<FreeLookCamera>();
        }
    }

    // El resto de los métodos de GameManager2 quedan igual
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

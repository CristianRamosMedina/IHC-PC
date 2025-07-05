using UnityEngine;
using Photon.Pun;

public class NetworkPlayer2 : MonoBehaviourPun, IPunObservable
{   
    public GameObject pilotoHUDPrefab;
    public GameObject armeroHUDPrefab;
    private GameObject myHUD; // referencia al HUD instanciado
    public float moveSpeed = 5f;
    public bool isPilot = false; // NUEVO: define si eres piloto o armero

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        networkPosition = transform.position;
        networkRotation = transform.rotation;
        Debug.Log($"NetworkPlayer Start: {photonView.Owner.ActorNumber} ({(photonView.IsMine ? "MI CUBO" : "otro jugador")})");
    }

    void Update()
    {
        if (photonView.IsMine && isPilot)
        {
            if (Application.isFocused)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector3 direction = new Vector3(h, 0, v).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
        else if (!photonView.IsMine)
        {
            // Suaviza el movimiento de los otros jugadores
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }
    }

    public void SetRole(bool pilot)
    {
        isPilot = pilot;
        Debug.Log($"Mi rol es: {(pilot ? "PILOTO" : "ARMERO")}");
        if (photonView.IsMine)
        {
            if (myHUD != null) Destroy(myHUD); // evita duplicar HUD si el rol cambia
            if (isPilot && pilotoHUDPrefab != null)
            {
                myHUD = Instantiate(pilotoHUDPrefab);
            }
            else if (!isPilot && armeroHUDPrefab != null)
            {
                myHUD = Instantiate(armeroHUDPrefab);
           }
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}

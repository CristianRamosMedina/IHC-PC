using UnityEngine;
using Photon.Pun;

public class NetworkPiloto : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        networkPosition = transform.position;
        networkRotation = transform.rotation;

        if (photonView.IsMine)
        {
            Camera oldCam = Camera.main;
            if (oldCam != null) Destroy(oldCam.gameObject);

            Camera cam = Instantiate(GameManager2.Instance.playerCameraPrefab);
            cam.GetComponent<FollowPlayer>().target = this.transform;
            cam.tag = "MainCamera";
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(h, 0, v).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
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
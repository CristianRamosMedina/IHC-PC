using UnityEngine;
using Photon.Pun;
using System.Collections;

public class NetworkArmero : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine)
        {
            Camera oldCam = Camera.main;
            if (oldCam != null) Destroy(oldCam.gameObject);

            Camera cam = Instantiate(GameManager2.Instance.playerCameraPrefab);
            cam.GetComponent<FollowPlayer>().target = this.transform;
            cam.tag = "MainCamera";
            cam.gameObject.AddComponent<FreeLookCamera>();

            StartCoroutine(MountToPiloto());
        }
        else
        {
            StartCoroutine(MountToPiloto());
        }
    }

    IEnumerator MountToPiloto()
    {
        NetworkPiloto piloto = null;
        while (piloto == null)
        {
            piloto = FindObjectOfType<NetworkPiloto>();
            yield return null;
        }
        this.transform.SetParent(piloto.transform, true);
        this.transform.localPosition = new Vector3(0, 1.5f, 0);
    }
}

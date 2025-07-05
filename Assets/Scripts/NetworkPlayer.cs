using UnityEngine;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5f;

    public float mouseSensitivity = 2f;
    public Transform cameraHolder;
    private float verticalRotation = 0f;

    private Camera playerCamera;
    public float normalFOV = 60f;
    public float[] zoomLevels = { 30f, 20f, 10f };
    private int currentZoomIndex = -1;
    public float zoomSpeed = 10f;

    public PlayerUI playerUI;  // Referencia al script PlayerUI

    //public KeyCode zoomKey = KeyCode.LeftShift;

    // Variables para sincronización
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void Start()
    {
        networkPosition = transform.position;
        networkRotation = transform.rotation;

        if (!photonView.IsMine)
        {
            cameraHolder.gameObject.SetActive(false);
            return;
        }

        // Asignar la referencia al HUD
        playerUI = FindObjectOfType<PlayerUI>();  // Encuentra el PlayerUI en la escena
        playerUI.UpdateZoomText(currentZoomIndex);  // Actualiza el texto inicial con el zoom

        // Bloquear cursor para vista de jugador
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera = cameraHolder.GetComponentInChildren<Camera>();
        playerCamera.fieldOfView = normalFOV;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Application.isFocused)
            {
                // Movimiento del jugador con el teclado
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Vector3 direction = new Vector3(h, 0, v).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);

                // Rotación con el mouse
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                verticalRotation -= mouseY;
                verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

                cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
                transform.Rotate(Vector3.up * mouseX);

                // Cambiar nivel de zoom con teclas 1, 2, 3 y desactivar con 0
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    currentZoomIndex = 0;
                    playerUI.UpdateZoomText(currentZoomIndex);  // Actualiza el texto y la mira
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    currentZoomIndex = 1;
                    playerUI.UpdateZoomText(currentZoomIndex);  // Actualiza el texto y la mira
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    currentZoomIndex = 2;
                    playerUI.UpdateZoomText(currentZoomIndex);  // Actualiza el texto y la mira
                }
                else if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    currentZoomIndex = -1; // Desactivar zoom
                    playerUI.UpdateZoomText(currentZoomIndex);  // Actualiza el texto y la mira
                }

                // Cambiar visibilidad de la segunda mira con la barra espaciadora
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerUI.ToggleAltCrosshair();  // Alterna la visibilidad de la segunda mira
                }

                // Aplicar zoom dinámicamente
                float targetFOV = currentZoomIndex >= 0 ? zoomLevels[currentZoomIndex] : normalFOV;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            }
        }
        else
        {
            // Suaviza el movimiento de los otros jugadores
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }

    }

    // Llamado automáticamente por Photon para sincronizar datos
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviar datos a otros
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Recibir datos de red
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}

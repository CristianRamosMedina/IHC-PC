using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, 0); // ← Solo arriba, nada atrás

    void LateUpdate()
    {
        if (target)
        {
            // Coloca la cámara encima del jugador, alineada con su rotación
            transform.position = target.position + target.rotation * offset;
            transform.rotation = target.rotation; // O usa Quaternion.identity si la quieres mirar siempre hacia abajo
        }
    }
}

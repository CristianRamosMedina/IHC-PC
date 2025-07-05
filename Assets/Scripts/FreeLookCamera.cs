using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    public float sensitivity = 3f;
    float rotationY = 0f;
    float rotationX = 0f;

    void Start()
    {
        // Bloquea el cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -60f, 60f);

        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
    }
}

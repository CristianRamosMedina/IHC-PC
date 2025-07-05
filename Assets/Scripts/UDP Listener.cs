using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class HandtrackingTCP : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float giroFactor = 1f;
    volatile int ultimoAngulo = 0;
    Thread tcpThread;

    void Start()
    {
        tcpThread = new Thread(EscucharTCP);
        tcpThread.IsBackground = true;
        tcpThread.Start();
    }

    void EscucharTCP()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5050);
        listener.Start();
        Debug.Log("TCP Listener iniciado en puerto 5050");

        while (true)
        {
            using (var client = listener.AcceptTcpClient())
            using (var stream = client.GetStream())
            {
                Debug.Log("Cliente Python conectado.");
                byte[] buffer = new byte[32];
                int bytesRead = 0;
                StringBuilder sb = new StringBuilder();

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    sb.Append(mensaje);

                    string[] lines = sb.ToString().Split('\n');
                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        string line = lines[i].Trim();
                        if (int.TryParse(line, out int angulo))
                        {
                            ultimoAngulo = angulo;
                            Debug.Log("Ángulo recibido por TCP: " + angulo);
                        }
                    }
                    sb = new StringBuilder(lines[lines.Length - 1]);
                }
                Debug.Log("Cliente Python desconectado.");
            }
        }
    }

    void Update()
    {
        // Avanza siempre hacia adelante
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Gira con el ángulo recibido
        float yRotation = ultimoAngulo * giroFactor * Time.deltaTime;
        transform.Rotate(0, yRotation, 0);

        // Debug de movimiento/giro
        if (Mathf.Abs(ultimoAngulo) > 1)
            Debug.Log("Ángulo aplicado: " + ultimoAngulo + " | Rotación frame: " + yRotation);
    }

    void OnApplicationQuit()
    {
        if (tcpThread != null && tcpThread.IsAlive) tcpThread.Abort();
    }
}

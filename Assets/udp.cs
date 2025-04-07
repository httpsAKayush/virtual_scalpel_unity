using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPRotation : MonoBehaviour
{
    [Header("Target to Rotate (Drag your bone here)")]
    public Transform target;

    private UdpClient udpClient;
    private Thread receiveThread;

    private float pitch, roll, yaw;
    private readonly object rotationLock = new object();

    void Start()
    {
        udpClient = new UdpClient(25665); // Port must match ESP sender
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("UDP Listener started...");
    }

    void LateUpdate()
    {
        if (target == null) return;

        float p, r, y;
        lock (rotationLock)
        {
            p = pitch;
            r = roll;
            y = yaw;
        }

        // Apply rotation to the target object
        target.rotation = Quaternion.Euler(p, y, r); // Unity uses Yaw around Y, Pitch around X, Roll around Z
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                Debug.Log("Received: " + message);

                string[] parts = message.Split(',');
                if (parts.Length == 3)
                {
                    float p = float.Parse(parts[0]);
                    float r = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);

                    lock (rotationLock)
                    {
                        pitch = p;
                        roll = r;
                        yaw = y;
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket closed: " + ex.Message);
                break;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort(); // Safe since we're quitting
        }

        udpClient?.Close();
    }
}

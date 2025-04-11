using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class IndexFingerUDPControl : MonoBehaviour
{
    [Header("Index Finger Joints")]
    public Transform jointA; // IndexA_L
    public Transform jointB; // IndexB_L
    public Transform jointC; // IndexC_L

    [Header("Max Curl Rotation")]
    public Vector3 jointA_CurlRotation = new Vector3(45f, 0f, 0f);
    public Vector3 jointB_CurlRotation = new Vector3(60f, 0f, 0f);
    public Vector3 jointC_CurlRotation = new Vector3(40f, 0f, 0f);

    [Header("Smoothing")]
    public float smoothSpeed = 10f; // Adjust for smoother/faster transitions

    private float curl = 0f;        // Current curl value
    private float targetCurl = 0f;  // Target curl received from UDP

    private UdpClient udpClient;
    private Thread receiveThread;

    void Start()
    {
        udpClient = new UdpClient(25668); // Port used in Arduino
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("UDP Finger Listener started...");
    }

    void Update()
    {
        // Smoothly interpolate current curl to targetCurl
        curl = Mathf.Lerp(curl, targetCurl, Time.deltaTime * smoothSpeed);

        // Apply rotation based on curl (0–1)
        if (jointA) jointA.localRotation = Quaternion.Euler(jointA_CurlRotation * curl);
        if (jointB) jointB.localRotation = Quaternion.Euler(jointB_CurlRotation * curl);
        if (jointC) jointC.localRotation = Quaternion.Euler(jointC_CurlRotation * curl);
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
                int bend = int.Parse(message); // Expecting 0–100
                targetCurl = Mathf.Clamp01(bend / 100f); // Set target only
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
            receiveThread.Abort();
        udpClient?.Close();
    }
}

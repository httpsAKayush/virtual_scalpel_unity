using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class FingerUDPController : MonoBehaviour
{
    [Header("Index Finger Joints")]
    public Transform indexA;
    public Transform indexB;
    public Transform indexC;

    [Header("Middle Finger Joints")]
    public Transform middleA;
    public Transform middleB;
    public Transform middleC;

    [Header("Ring Finger Joints")]
    public Transform ringA;
    public Transform ringB;
    public Transform ringC;

    [Header("Thumb Finger Joints")]
    public Transform thumbA;
    public Transform thumbB;
    public Transform thumbC;

    [Header("Pinky Finger Joints (Simulated)")]
    public Transform pinkyA;
    public Transform pinkyB;
    public Transform pinkyC;

    [Header("Max Curl Rotations - Index")]
    public Vector3 indexCurlA = new Vector3(45f, 0f, 0f);
    public Vector3 indexCurlB = new Vector3(60f, 0f, 0f);
    public Vector3 indexCurlC = new Vector3(40f, 0f, 0f);

    [Header("Max Curl Rotations - Middle")]
    public Vector3 middleCurlA = new Vector3(50f, 0f, 0f);
    public Vector3 middleCurlB = new Vector3(65f, 0f, 0f);
    public Vector3 middleCurlC = new Vector3(45f, 0f, 0f);

    [Header("Max Curl Rotations - Ring")]
    public Vector3 ringCurlA = new Vector3(50f, 0f, 0f);
    public Vector3 ringCurlB = new Vector3(60f, 0f, 0f);
    public Vector3 ringCurlC = new Vector3(40f, 0f, 0f);

    [Header("Max Curl Rotations - Thumb")]
    public Vector3 thumbCurlA = new Vector3(40f, 0f, 0f);
    public Vector3 thumbCurlB = new Vector3(50f, 0f, 0f);
    public Vector3 thumbCurlC = new Vector3(35f, 0f, 0f);

    [Header("Max Curl Rotations - Pinky (Simulated)")]
    public Vector3 pinkyCurlA = new Vector3(40f, 0f, 0f);
    public Vector3 pinkyCurlB = new Vector3(55f, 0f, 0f);
    public Vector3 pinkyCurlC = new Vector3(35f, 0f, 0f);

    private float[] curls = new float[4]; // Index, Middle, Ring, Thumb
    private UdpClient udpClient;
    private Thread receiveThread;

    void Start()
    {
        udpClient = new UdpClient(25668);
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("UDP Finger Listener started...");
    }

    void Update()
    {
        ApplyCurl(indexA, indexB, indexC, indexCurlA, indexCurlB, indexCurlC, curls[0]);
        ApplyCurl(middleA, middleB, middleC, middleCurlA, middleCurlB, middleCurlC, curls[1]);
        ApplyCurl(ringA, ringB, ringC, ringCurlA, ringCurlB, ringCurlC, curls[2]);
        ApplyCurl(thumbA, thumbB, thumbC, thumbCurlA, thumbCurlB, thumbCurlC, curls[3]);

        // 👇 Simulate Pinky using Ring finger reading
        float pinkyCurl = curls[2] * 0.8f; // 80% of ring finger
        ApplyCurl(pinkyA, pinkyB, pinkyC, pinkyCurlA, pinkyCurlB, pinkyCurlC, pinkyCurl);
    }

    private void ApplyCurl(Transform a, Transform b, Transform c, Vector3 rotA, Vector3 rotB, Vector3 rotC, float curl)
    {
        if (a) a.localRotation = Quaternion.Euler(rotA * curl);
        if (b) b.localRotation = Quaternion.Euler(rotB * curl);
        if (c) c.localRotation = Quaternion.Euler(rotC * curl);
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
                string[] tokens = message.Split(',');

                for (int i = 0; i < Mathf.Min(tokens.Length, 4); i++)
                {
                    if (int.TryParse(tokens[i], out int bendVal))
                        curls[i] = Mathf.Clamp01(bendVal / 100f);
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
            receiveThread.Abort();
        udpClient?.Close();
    }
}

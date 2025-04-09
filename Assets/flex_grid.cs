using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    UdpClient client;
    int port = 25668;

    //string[] leftFingers = { "L_Thumb", "L_Index", "L_Middle", "L_Ring", "L_Pinky" };
    //string[] rightFingers = { "R_Thumb", "R_Index", "R_Middle", "R_Ring", "R_Pinky" };

    void Start()
    {
        client = new UdpClient(port);
        client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        Debug.Log("UDP Receiver started on port " + port);
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
        byte[] received = client.EndReceive(ar, ref ep);
        string data = Encoding.ASCII.GetString(received);

        Debug.Log($"Received: {data}");

        string[] tokens = data.Split(',');
        if (tokens.Length == 10)
        {
            Debug.Log("Left Hand:");
            for (int i = 0; i < 5; i++)
            {
                Debug.Log($" {tokens[i]}%");
            }

            Debug.Log("Right Hand:");
            for (int i = 5; i < 10; i++)
            {
                Debug.Log($" {tokens[i]}%");
            }
        }
        else
        {
            Debug.LogWarning("Invalid data length received.");
        }

        // Keep listening
        client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    void OnApplicationQuit()
    {
        client?.Close();
    }
}

// void ReceiveCallback(IAsyncResult ar)
// {
//     IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
//     byte[] received = client.EndReceive(ar, ref ep);
//     string data = Encoding.ASCII.GetString(received);

//     Debug.Log($"Received: {data}");

//     string[] tokens = data.Split(',');
//     if (tokens.Length == 10)
//     {
//         StringBuilder sb = new StringBuilder();
//         sb.AppendLine("Left Hand:");
//         for (int i = 0; i < 5; i++)
//             sb.AppendLine($"  Finger {i + 1}: {tokens[i]}%");

//         sb.AppendLine("Right Hand:");
//         for (int i = 5; i < 10; i++)
//             sb.AppendLine($"  Finger {i - 4}: {tokens[i]}%");

//         Debug.Log(sb.ToString());
//     }
//     else
//     {
//         Debug.LogWarning("Invalid data length received.");
//     }

//     // Keep listening
//     client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
// }
// }

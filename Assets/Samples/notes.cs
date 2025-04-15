// Import necessary namespaces for networking and threading
using System.Net;               // Provides IPAddress and IPEndPoint classes
using System.Net.Sockets;       // Provides UdpClient class for UDP communication
using System.Text;              // For converting byte arrays to strings
using System.Threading;         // Supports threading for non-blocking network receives
using UnityEngine;             // Unity Engine functions, including transforms for applying data

// Declare class-level variables for UDP communications
public class HandWithOrientationController : MonoBehaviour
{
    private UdpClient udpClient;           // Will be used to receive (or send) UDP packets
    private Thread receiveThread;          // Runs the receive function on a separate thread
    private int listenPort = 25667;        // Port must match the sender; here we use the same as on the ESP32
    private readonly object dataLock = new object();  // For thread-safety when sharing data

    // In the Start() method, initialize UDP and start the receiving thread
    void Start()
    {
        // Create a UdpClient listening on 'listenPort'
        udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, listenPort));
        udpClient.EnableBroadcast = true;  // Accept broadcast packets

        // Create and start the background thread that handles incoming UDP packets
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;  // Ensures thread will exit when Unity stops
        receiveThread.Start();

        Debug.Log("Hand With Orientation UDP Receiver started...");
        
        // ... [Additional initialization code such as caching transforms]
    }

    // The ReceiveData method continuously waits for UDP messages
    private void ReceiveData()
    {
        // 'remoteEP' is used to capture the sender's network details (can be ignored or used)
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
        while (true)
        {
            try
            {
                // Block here until a UDP packet is received; data is read into a byte array
                byte[] data = udpClient.Receive(ref remoteEP);
                // Convert the byte array to a string using ASCII encoding (must match sender's encoding)
                string message = Encoding.ASCII.GetString(data);
                // Example message: "15.23,3.67,-45.12,80,50,30,90"

                // Split the incoming string into tokens using comma as delimiter and parse as needed
                string[] tokens = message.Split(',');
                // Lock shared data before updating the internal state to prevent concurrency issues
                lock (dataLock)
                {
                    // Example: Parse tokens and update pitch, roll, yaw, and flex sensor values
                    float newPitch = float.Parse(tokens[0]);
                    float newRoll = float.Parse(tokens[1]);
                    float newYaw = float.Parse(tokens[2]);
                    // Further tokens for finger sensor values follow...
                }
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket exception: " + ex.Message);
                break; // Break out if any socket error occurs
            }
        }
    }

    // In the OnApplicationQuit() method, properly close the UDP client and thread
    void OnApplicationQuit()
    {
        // Signal the thread to stop and close UDP client to free network resources
        udpClient?.Close();
        receiveThread?.Join();  // Wait for the thread to finish cleanly
    }

    // In the Update or LateUpdate method, you would retrieve the updated sensor values (protected by locks)
    // and apply them to modify the hand model’s orientation or finger curls accordingly.
}

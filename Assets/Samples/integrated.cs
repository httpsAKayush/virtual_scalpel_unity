// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;

// public class UnifiedHandController : MonoBehaviour
// {
//     [Header("Hand Orientation Root")]
//     public Transform handRoot;

//     [Header("Finger Transforms")]
//     public Transform indexA, indexB, indexC;
//     public Transform middleA, middleB, middleC;
//     public Transform ringA, ringB, ringC;
//     public Transform thumbA, thumbB, thumbC;
//     public Transform pinkyA, pinkyB, pinkyC;

//     [Header("Max Curl Angles (z-axis only)")]
//     public float indexCurlA = 45f, indexCurlB = 60f, indexCurlC = 40f;
//     public float middleCurlA = 50f, middleCurlB = 65f, middleCurlC = 45f;
//     public float ringCurlA = 50f, ringCurlB = 60f, ringCurlC = 40f;
//     public float thumbCurlA = 40f, thumbCurlB = 50f, thumbCurlC = 35f;
//     public float pinkyCurlA = 40f, pinkyCurlB = 55f, pinkyCurlC = 35f;

//     private float pitch, roll, yaw;
//     private float[] curls = new float[4]; // index, middle, ring, thumb

//     private readonly object dataLock = new object();
//     private UdpClient udpClient;
//     private Thread receiveThread;

//     private Quaternion initialRotation;

//     void Start()
//     {
//         udpClient = new UdpClient(25666); // Port for UDP
//         receiveThread = new Thread(ReceiveData);
//         receiveThread.IsBackground = true;
//         receiveThread.Start();
//         Debug.Log("Unified Hand UDP Receiver started...");

//         initialRotation = Quaternion.Euler(-5.79f, 21.537f, -6.135f);
//         if (handRoot != null)
//             handRoot.rotation = initialRotation;
//     }

//     void LateUpdate()
//     {
//         float p, r, y;
//         float[] fingers = new float[4];

//         lock (dataLock)
//         {
//             p = -pitch;
//             r = -roll;
//             y = yaw;
//             curls.CopyTo(fingers, 0);
//         }

//         if (handRoot)
//         {
//             // Full 3D rotation with pitch & yaw swapped
//             Quaternion rotationDelta = Quaternion.Euler(-p, r, y);
//             handRoot.rotation = initialRotation * rotationDelta;
//         }

//         // Apply finger curls (Z-axis only)
//         ApplyCurlZOnly(indexA, indexB, indexC, indexCurlA, indexCurlB, indexCurlC, fingers[0]);
//         ApplyCurlZOnly(middleA, middleB, middleC, middleCurlA, middleCurlB, middleCurlC, fingers[1]);
//         ApplyCurlZOnly(ringA, ringB, ringC, ringCurlA, ringCurlB, ringCurlC, fingers[2]);
//         ApplyCurlZOnly(thumbA, thumbB, thumbC, thumbCurlA, thumbCurlB, thumbCurlC, fingers[3]);

//         float pinkyCurl = fingers[2] * 0.8f;
//         ApplyCurlZOnly(pinkyA, pinkyB, pinkyC, pinkyCurlA, pinkyCurlB, pinkyCurlC, pinkyCurl);
//     }

//     private void ApplyCurlZOnly(Transform a, Transform b, Transform c, float angleA, float angleB, float angleC, float curl)
//     {
//         if (a) a.localRotation = Quaternion.Euler(0f, 0f, angleA * curl);
//         if (b) b.localRotation = Quaternion.Euler(0f, 0f, angleB * curl);
//         if (c) c.localRotation = Quaternion.Euler(0f, 90f, angleC * curl);
//     }

//     private void ReceiveData()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

//         while (true)
//         {
//             try
//             {
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 string message = Encoding.ASCII.GetString(data);
//                 string[] tokens = message.Split(',');

//                 if (tokens.Length >= 7)
//                 {
//                     float p = float.Parse(tokens[0]);
//                     float r = float.Parse(tokens[1]);
//                     float y = float.Parse(tokens[2]);

//                     float[] newCurls = new float[4];
//                     for (int i = 0; i < 4; i++)
//                     {
//                         if (int.TryParse(tokens[3 + i], out int val))
//                             newCurls[i] = Mathf.Clamp01(val / 100f);
//                     }

//                     lock (dataLock)
//                     {
//                         // SWAP pitch and yaw
//                         pitch = y;
//                         yaw = p;
//                         roll = r;
//                         newCurls.CopyTo(curls, 0);
//                     }
//                 }
//             }
//             catch (SocketException ex)
//             {
//                 Debug.Log("Socket closed: " + ex.Message);
//                 break;
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         if (receiveThread != null && receiveThread.IsAlive)
//             receiveThread.Abort();
//         udpClient?.Close();
//     }
// }

///////////////////////////////////////////////////////////////////
/// 
/// 
// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Collections.Generic;

// public class UnifiedHandController : MonoBehaviour
// {
//     [Header("Hand Orientation Root")]
//     public Transform handRoot;

//     [Header("Finger Transforms")]
//     public Transform indexA, indexB, indexC;
//     public Transform middleA, middleB, middleC;
//     public Transform ringA, ringB, ringC;
//     public Transform thumbA, thumbB, thumbC;
//     public Transform pinkyA, pinkyB, pinkyC;

//     [Header("Max Curl Angles (Z-axis only)")]
//     public float indexCurlA = 45f, indexCurlB = 60f, indexCurlC = 40f;
//     public float middleCurlA = 50f, middleCurlB = 65f, middleCurlC = 45f;
//     public float ringCurlA = 50f, ringCurlB = 60f, ringCurlC = 40f;
//     public float thumbCurlA = 40f, thumbCurlB = 50f, thumbCurlC = 35f;
//     public float pinkyCurlA = 40f, pinkyCurlB = 55f, pinkyCurlC = 35f;

//     private float pitch, roll, yaw;
//     private float[] curls = new float[4]; // index, middle, ring, thumb

//     private readonly object dataLock = new object();
//     private UdpClient udpClient;
//     private Thread receiveThread;
//     private bool isRunning = true;

//     private Quaternion initialRotation;

//     // Store original local rotations for fingers
//     private Dictionary<Transform, Quaternion> baseRotations = new Dictionary<Transform, Quaternion>();

//     void Start()
//     {
//         udpClient = new UdpClient(25666); // Port for UDP
//         receiveThread = new Thread(ReceiveData);
//         receiveThread.IsBackground = true;
//         receiveThread.Start();
//         Debug.Log("Unified Hand UDP Receiver started...");

//         initialRotation = Quaternion.Euler(-5.79f, 21.537f, -6.135f);
//         if (handRoot != null)
//             handRoot.rotation = initialRotation;

//         // Save base local rotations of all finger joints
//         CacheBaseRotation(indexA);
//         CacheBaseRotation(indexB);
//         CacheBaseRotation(indexC);
//         CacheBaseRotation(middleA);
//         CacheBaseRotation(middleB);
//         CacheBaseRotation(middleC);
//         CacheBaseRotation(ringA);
//         CacheBaseRotation(ringB);
//         CacheBaseRotation(ringC);
//         CacheBaseRotation(thumbA);
//         CacheBaseRotation(thumbB);
//         CacheBaseRotation(thumbC);
//         CacheBaseRotation(pinkyA);
//         CacheBaseRotation(pinkyB);
//         CacheBaseRotation(pinkyC);
//     }

//     void LateUpdate()
//     {
//         float p, r, y;
//         float[] fingers = new float[4];

//         lock (dataLock)
//         {
//             p = -pitch;
//             r = -roll;
//             y = yaw;
//             curls.CopyTo(fingers, 0);
//         }

//         if (handRoot)
//         {
//             Quaternion rotationDelta = Quaternion.Euler(-p, r, y);
//             handRoot.rotation = initialRotation * rotationDelta;
//         }

//         // Apply finger curls
//         ApplyCurlZOnly(indexA, indexCurlA, fingers[0]);
//         ApplyCurlZOnly(indexB, indexCurlB, fingers[0]);
//         ApplyCurlZOnly(indexC, indexCurlC, fingers[0]);

//         ApplyCurlZOnly(middleA, middleCurlA, fingers[1]);
//         ApplyCurlZOnly(middleB, middleCurlB, fingers[1]);
//         ApplyCurlZOnly(middleC, middleCurlC, fingers[1]);

//         ApplyCurlZOnly(ringA, ringCurlA, fingers[2]);
//         ApplyCurlZOnly(ringB, ringCurlB, fingers[2]);
//         ApplyCurlZOnly(ringC, ringCurlC, fingers[2]);

//         ApplyCurlZOnly(thumbA, thumbCurlA, fingers[3]);
//         ApplyCurlZOnly(thumbB, thumbCurlB, fingers[3]);
//         ApplyCurlZOnly(thumbC, thumbCurlC, fingers[3]);

//         float pinkyCurl = fingers[2] * 0.8f;
//         ApplyCurlZOnly(pinkyA, pinkyCurlA, pinkyCurl);
//         ApplyCurlZOnly(pinkyB, pinkyCurlB, pinkyCurl);
//         ApplyCurlZOnly(pinkyC, pinkyCurlC, pinkyCurl);
//     }

//     private void CacheBaseRotation(Transform joint)
//     {
//         if (joint != null)
//             baseRotations[joint] = joint.localRotation;
//     }

//     /// <summary>
//     /// Applies Z-axis curl relative to the joint's initial local rotation.
//     /// </summary>
//     private void ApplyCurlZOnly(Transform joint, float maxAngle, float curlAmount)
//     {
//         if (joint == null || !baseRotations.ContainsKey(joint)) return;

//         Quaternion baseRot = baseRotations[joint];
//         Quaternion zRotation = Quaternion.Euler(0f, 0f, maxAngle * curlAmount);
//         joint.localRotation = baseRot * zRotation;
//     }

//     private void ReceiveData()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

//         while (isRunning)
//         {
//             try
//             {
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 string message = Encoding.ASCII.GetString(data);
//                 string[] tokens = message.Split(',');

//                 if (tokens.Length >= 7)
//                 {
//                     float p = float.Parse(tokens[0]);
//                     float r = float.Parse(tokens[1]);
//                     float y = float.Parse(tokens[2]);

//                     float[] newCurls = new float[4];
//                     for (int i = 0; i < 4; i++)
//                     {
//                         if (int.TryParse(tokens[3 + i], out int val))
//                             newCurls[i] = Mathf.Clamp01(val / 100f);
//                     }

//                     lock (dataLock)
//                     {
//                         pitch = y;
//                         yaw = p;
//                         roll = r;
//                         newCurls.CopyTo(curls, 0);
//                     }
//                 }
//             }
//             catch (SocketException ex)
//             {
//                 Debug.Log("Socket exception: " + ex.Message);
//                 break;
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         isRunning = false;
//         udpClient?.Close();
//         receiveThread?.Join();
//     }
// }

// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Collections.Generic;

// public class FlexOnlyHandController : MonoBehaviour
// {
//     [Header("Finger Transforms")]
//     public Transform indexA, indexB, indexC;
//     public Transform middleA, middleB, middleC;
//     public Transform ringA, ringB, ringC;
//     public Transform thumbA, thumbB, thumbC;
//     public Transform pinkyA, pinkyB, pinkyC;

//     [Header("Max Curl Angles (X-axis only)")]
//     public float indexCurlA = 45f, indexCurlB = 60f, indexCurlC = 40f;
//     public float middleCurlA = 50f, middleCurlB = 65f, middleCurlC = 45f;
//     public float ringCurlA = 50f, ringCurlB = 60f, ringCurlC = 40f;
//     public float thumbCurlA = 40f, thumbCurlB = 50f, thumbCurlC = 35f;
//     public float pinkyCurlA = 40f, pinkyCurlB = 55f, pinkyCurlC = 35f;

//     private float[] curls = new float[4]; // thumb, index, middle, ring

//     private readonly object dataLock = new object();
//     private UdpClient udpClient;
//     private Thread receiveThread;
//     private bool isRunning = true;

//     private Dictionary<Transform, Quaternion> baseRotations = new Dictionary<Transform, Quaternion>();

//     void Start()
//     {
//         udpClient = new UdpClient(25666); // Port for UDP
//         receiveThread = new Thread(ReceiveData);
//         receiveThread.IsBackground = true;
//         receiveThread.Start();
//         Debug.Log("Flex-only Hand UDP Receiver started...");

//         // Cache base local rotations
//         CacheBaseRotation(indexA); CacheBaseRotation(indexB); CacheBaseRotation(indexC);
//         CacheBaseRotation(middleA); CacheBaseRotation(middleB); CacheBaseRotation(middleC);
//         CacheBaseRotation(ringA); CacheBaseRotation(ringB); CacheBaseRotation(ringC);
//         CacheBaseRotation(thumbA); CacheBaseRotation(thumbB); CacheBaseRotation(thumbC);
//         CacheBaseRotation(pinkyA); CacheBaseRotation(pinkyB); CacheBaseRotation(pinkyC);
//     }

//     void LateUpdate()
//     {
//         float[] fingers = new float[4];
//         lock (dataLock) curls.CopyTo(fingers, 0);

//         ApplyCurlXOnly(indexA, indexCurlA, fingers[1]);
//         ApplyCurlXOnly(indexB, indexCurlB, fingers[1]);
//         ApplyCurlXOnly(indexC, indexCurlC, fingers[1]);

//         ApplyCurlXOnly(middleA, middleCurlA, fingers[2]);
//         ApplyCurlXOnly(middleB, middleCurlB, fingers[2]);
//         ApplyCurlXOnly(middleC, middleCurlC, fingers[2]);

//         ApplyCurlXOnly(ringA, ringCurlA, fingers[3]);
//         ApplyCurlXOnly(ringB, ringCurlB, fingers[3]);
//         ApplyCurlXOnly(ringC, ringCurlC, fingers[3]);

//         ApplyCurlXOnly(thumbA, thumbCurlA, fingers[0]);
//         ApplyCurlXOnly(thumbB, thumbCurlB, fingers[0]);
//         ApplyCurlXOnly(thumbC, thumbCurlC, fingers[0]);

//         float pinkyCurl = fingers[3] * 0.8f;
//         ApplyCurlXOnly(pinkyA, pinkyCurlA, pinkyCurl);
//         ApplyCurlXOnly(pinkyB, pinkyCurlB, pinkyCurl);
//         ApplyCurlXOnly(pinkyC, pinkyCurlC, pinkyCurl);
//     }

//     private void CacheBaseRotation(Transform joint)
//     {
//         if (joint != null)
//             baseRotations[joint] = joint.localRotation;
//     }

//     private void ApplyCurlXOnly(Transform joint, float maxAngle, float curlAmount)
//     {
//         if (joint == null || !baseRotations.ContainsKey(joint)) return;

//         Quaternion baseRot = baseRotations[joint];
//         Quaternion xRotation = Quaternion.Euler(maxAngle * curlAmount, 0f, 0f);
//         joint.localRotation = baseRot * xRotation;
//     }

//     private void ReceiveData()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
//         while (isRunning)
//         {
//             try
//             {
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 string message = Encoding.ASCII.GetString(data);
//                 string[] tokens = message.Split(',');

//                 if (tokens.Length >= 4)
//                 {
//                     float[] newCurls = new float[4];
//                     for (int i = 0; i < 4; i++)
//                     {
//                         if (int.TryParse(tokens[i], out int val))
//                             newCurls[i] = Mathf.Clamp01(val / 100f);
//                     }

//                     lock (dataLock)
//                     {
//                         newCurls.CopyTo(curls, 0);
//                     }
//                 }
//             }
//             catch (SocketException ex)
//             {
//                 Debug.Log("Socket exception: " + ex.Message);
//                 break;
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         isRunning = false;
//         udpClient?.Close();
//         receiveThread?.Join();
//     }
// }
////////////////////////////////////////////////////////////////////////
/// flex done
// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Collections.Generic;

// public class FlexOnlyHandController : MonoBehaviour
// {
//     [Header("Finger Transforms")]
//     public Transform indexA, indexB, indexC;
//     public Transform middleA, middleB, middleC;
//     public Transform ringA, ringB, ringC;
//     public Transform thumbA, thumbB, thumbC;
//     public Transform pinkyA, pinkyB, pinkyC;

//     [Header("Max Curl Angles (X-axis, inward curl)")]
//     public float indexCurlA = 45f, indexCurlB = 60f, indexCurlC = 40f;
//     public float middleCurlA = 50f, middleCurlB = 65f, middleCurlC = 45f;
//     public float ringCurlA = 50f, ringCurlB = 60f, ringCurlC = 40f;
//     public float thumbCurlA = 40f, thumbCurlB = 50f, thumbCurlC = 35f;
//     public float pinkyCurlA = 30f, pinkyCurlB = 55f, pinkyCurlC = 35f;

//     private float[] curls = new float[4]; // thumb, index, middle, ring

//     private readonly object dataLock = new object();
//     private UdpClient udpClient;
//     private Thread receiveThread;
//     private bool isRunning = true;

//     private Dictionary<Transform, Quaternion> baseRotations = new Dictionary<Transform, Quaternion>();

//     void Start()
// {
//     // udpClient = new UdpClient(25666); // Port for UDP
//     // udpClient.EnableBroadcast = true;

// udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 25666));
// udpClient.EnableBroadcast = true;

//     receiveThread = new Thread(ReceiveData);
//     receiveThread.IsBackground = true;
//     receiveThread.Start();
//     Debug.Log("Flex-only Hand UDP Receiver started...");

//     // Cache base local rotations
//     CacheBaseRotation(indexA); CacheBaseRotation(indexB); CacheBaseRotation(indexC);
//     CacheBaseRotation(middleA); CacheBaseRotation(middleB); CacheBaseRotation(middleC);
//     CacheBaseRotation(ringA); CacheBaseRotation(ringB); CacheBaseRotation(ringC);
//     CacheBaseRotation(thumbA); CacheBaseRotation(thumbB); CacheBaseRotation(thumbC);
//     CacheBaseRotation(pinkyA); CacheBaseRotation(pinkyB); CacheBaseRotation(pinkyC);
// }


//     void LateUpdate()
//     {
//         float[] fingers = new float[4];
//         lock (dataLock) curls.CopyTo(fingers, 0);

//         ApplyCurlXOnly(indexA, indexCurlA, fingers[3]);
//         ApplyCurlXOnly(indexB, indexCurlB, fingers[3]);
//         ApplyCurlXOnly(indexC, indexCurlC, fingers[3]);

//         ApplyCurlXOnly(middleA, middleCurlA, fingers[2]);
//         ApplyCurlXOnly(middleB, middleCurlB, fingers[2]);
//         ApplyCurlXOnly(middleC, middleCurlC, fingers[2]);

//         ApplyCurlXOnly(ringA, ringCurlA, fingers[1]);
//         ApplyCurlXOnly(ringB, ringCurlB, fingers[1]);
//         ApplyCurlXOnly(ringC, ringCurlC, fingers[1]);

//         ApplyCurlXOnly(thumbA, thumbCurlA, fingers[0]);
//         ApplyCurlXOnly(thumbB, thumbCurlB, fingers[0]);
//         ApplyCurlXOnly(thumbC, thumbCurlC, fingers[0]);

//         float pinkyCurl = fingers[1] * 0.7f;
//         ApplyCurlXOnly(pinkyA, pinkyCurlA, pinkyCurl);
//         ApplyCurlXOnly(pinkyB, pinkyCurlB, pinkyCurl);
//         ApplyCurlXOnly(pinkyC, pinkyCurlC, pinkyCurl);
//     }

//     private void CacheBaseRotation(Transform joint)
//     {
//         if (joint != null)
//             baseRotations[joint] = joint.localRotation;
//     }

//     private void ApplyCurlXOnly(Transform joint, float maxAngle, float curlAmount)
//     {
//         if (joint == null || !baseRotations.ContainsKey(joint)) return;

//         Quaternion baseRot = baseRotations[joint];
//         Quaternion xRotation = Quaternion.Euler(-maxAngle * curlAmount, 0f, 0f); // NEGATE ANGLE for inward curl
//         joint.localRotation = baseRot * xRotation;
//     }

//     private void ReceiveData()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
//         while (isRunning)
//         {
//             try
//             {
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 string message = Encoding.ASCII.GetString(data);
//                 string[] tokens = message.Split(',');

//                 if (tokens.Length >= 4)
//                 {
//                     float[] newCurls = new float[4];
//                     for (int i = 0; i < 4; i++)
//                     {
//                         if (int.TryParse(tokens[i], out int val))
//                             newCurls[i] = Mathf.Clamp01(val / 100f);
//                     }

//                     lock (dataLock)
//                     {
//                         newCurls.CopyTo(curls, 0);
//                     }
//                 }
//             }
//             catch (SocketException ex)
//             {
//                 Debug.Log("Socket exception: " + ex.Message);
//                 break;
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         isRunning = false;
//         udpClient?.Close();
//         receiveThread?.Join();
//     }
// }
// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Collections.Generic;

// public class HandWithOrientationController : MonoBehaviour
// {
//     [Header("Finger Transforms")]
//     public Transform indexA, indexB, indexC;
//     public Transform middleA, middleB, middleC;
//     public Transform ringA, ringB, ringC;
//     public Transform thumbA, thumbB, thumbC;
//     public Transform pinkyA, pinkyB, pinkyC;

//     [Header("Target Object for Orientation")]
//     public Transform targetObject;

//     [Header("Max Curl Angles (X-axis, inward curl)")]
//     public float indexCurlA = 45f, indexCurlB = 60f, indexCurlC = 40f;
//     public float middleCurlA = 50f, middleCurlB = 65f, middleCurlC = 45f;
//     public float ringCurlA = 50f, ringCurlB = 60f, ringCurlC = 40f;
//     public float thumbCurlA = 40f, thumbCurlB = 50f, thumbCurlC = 35f;
//     public float pinkyCurlA = 30f, pinkyCurlB = 55f, pinkyCurlC = 35f;

//     private float[] curls = new float[4]; // thumb, index, middle, ring
//     private float pitch, roll, yaw;

//     private readonly object dataLock = new object();
//     private UdpClient udpClient;
//     private Thread receiveThread;
//     private bool isRunning = true;

//     private Dictionary<Transform, Quaternion> baseRotations = new Dictionary<Transform, Quaternion>();
//     private Quaternion baseOrientation;

//     void Start()
//     {
//         udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 25666));
//         udpClient.EnableBroadcast = true;

//         receiveThread = new Thread(ReceiveData);
//         receiveThread.IsBackground = true;
//         receiveThread.Start();
//         Debug.Log("Hand With Orientation UDP Receiver started...");

//         // Cache base local rotations for all finger joints
//         CacheBaseRotation(indexA); CacheBaseRotation(indexB); CacheBaseRotation(indexC);
//         CacheBaseRotation(middleA); CacheBaseRotation(middleB); CacheBaseRotation(middleC);
//         CacheBaseRotation(ringA); CacheBaseRotation(ringB); CacheBaseRotation(ringC);
//         CacheBaseRotation(thumbA); CacheBaseRotation(thumbB); CacheBaseRotation(thumbC);
//         CacheBaseRotation(pinkyA); CacheBaseRotation(pinkyB); CacheBaseRotation(pinkyC);

//         if (targetObject != null)
//             baseOrientation = targetObject.localRotation;
//     }

//     void LateUpdate()
//     {
//         float[] fingers = new float[4];
//         float localPitch, localRoll, localYaw;

//         lock (dataLock)
//         {
//             curls.CopyTo(fingers, 0);
//             localPitch = pitch;
//             localRoll = roll;
//             localYaw = yaw;
//         }

//         // Apply finger curls
//         ApplyCurlXOnly(indexA, indexCurlA, fingers[1]);
//         ApplyCurlXOnly(indexB, indexCurlB, fingers[1]);
//         ApplyCurlXOnly(indexC, indexCurlC, fingers[1]);

//         ApplyCurlXOnly(middleA, middleCurlA, fingers[2]);
//         ApplyCurlXOnly(middleB, middleCurlB, fingers[2]);
//         ApplyCurlXOnly(middleC, middleCurlC, fingers[2]);

//         ApplyCurlXOnly(ringA, ringCurlA, fingers[3]);
//         ApplyCurlXOnly(ringB, ringCurlB, fingers[3]);
//         ApplyCurlXOnly(ringC, ringCurlC, fingers[3]);

//         ApplyCurlXOnly(thumbA, thumbCurlA, fingers[0]);
//         ApplyCurlXOnly(thumbB, thumbCurlB, fingers[0]);
//         ApplyCurlXOnly(thumbC, thumbCurlC, fingers[0]);

//         float pinkyCurl = fingers[3] * 0.7f;
//         ApplyCurlXOnly(pinkyA, pinkyCurlA, pinkyCurl);
//         ApplyCurlXOnly(pinkyB, pinkyCurlB, pinkyCurl);
//         ApplyCurlXOnly(pinkyC, pinkyCurlC, pinkyCurl);

//         // Apply orientation to hand object
//         if (targetObject != null)
//         {
//             // Adjust for coordinate mapping: ESP32 to Unity
//             Quaternion rotation = Quaternion.Euler(
//                 localYaw,   // pitch becomes X, negate for correct tilt
//                 -localRoll,      // yaw becomes Y
//                 localPitch  // roll becomes Z, also negated
//             );
//             targetObject.localRotation = baseOrientation * rotation;
//         }
//     }

//     private void CacheBaseRotation(Transform joint)
//     {
//         if (joint != null)
//             baseRotations[joint] = joint.localRotation;
//     }

//     private void ApplyCurlXOnly(Transform joint, float maxAngle, float curlAmount)
//     {
//         if (joint == null || !baseRotations.ContainsKey(joint)) return;

//         Quaternion baseRot = baseRotations[joint];
//         Quaternion xRotation = Quaternion.Euler(-maxAngle * curlAmount, 0f, 0f);
//         joint.localRotation = baseRot * xRotation;
//     }

//     private void ReceiveData()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
//         while (isRunning)
//         {
//             try
//             {
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 string message = Encoding.ASCII.GetString(data);
//                 string[] tokens = message.Split(',');

//                 if (tokens.Length >= 7)
//                 {
//                     float newPitch = float.Parse(tokens[0]);
//                     float newRoll = float.Parse(tokens[1]);
//                     float newYaw = float.Parse(tokens[2]);

//                     float[] newCurls = new float[4];
//                     for (int i = 0; i < 4; i++)
//                     {
//                         if (int.TryParse(tokens[3 + i], out int val))
//                             newCurls[i] = Mathf.Clamp01(val / 100f);
//                     }

//                     lock (dataLock)
//                     {
//                         pitch = newPitch;
//                         roll = newRoll;
//                         yaw = newYaw;
//                         newCurls.CopyTo(curls, 0);
//                     }
//                 }
//             }
//             catch (SocketException ex)
//             {
//                 Debug.Log("Socket exception: " + ex.Message);
//                 break;
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         isRunning = false;
//         udpClient?.Close();
//         receiveThread?.Join();
//     }
// }
/////////////////////////////////////////////////////////
/// without joystick
// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Collections.Generic;

// public class HandWithOrientationController : MonoBehaviour
// {
//     [Header("Finger Transforms")]
//     public Transform indexA, indexB, indexC;
//     public Transform middleA, middleB, middleC;
//     public Transform ringA, ringB, ringC;
//     public Transform thumbA, thumbB, thumbC;
//     public Transform pinkyA, pinkyB, pinkyC;

//     [Header("Target Object for Orientation")]
//     public Transform targetObject;

//     [Header("Max Curl Angles (X-axis, inward curl)")]
//     public float indexCurlA = 45f, indexCurlB = 60f, indexCurlC = 40f;
//     public float middleCurlA = 50f, middleCurlB = 65f, middleCurlC = 45f;
//     public float ringCurlA = 50f, ringCurlB = 60f, ringCurlC = 40f;
//     public float thumbCurlA = 40f, thumbCurlB = 50f, thumbCurlC = 35f;
//     public float pinkyCurlA = 30f, pinkyCurlB = 55f, pinkyCurlC = 35f;

//     private float[] curls = new float[4]; // thumb, index, middle, ring
//     private float pitch, roll, yaw;

//     private readonly object dataLock = new object();
//     private UdpClient udpClient;
//     private Thread receiveThread;
//     private bool isRunning = true;

//     private Dictionary<Transform, Quaternion> baseRotations = new Dictionary<Transform, Quaternion>();
//     private Quaternion baseOrientation;

//     // Orientation reference (for relative movement)
//     private bool orientationReferenceSet = false;
//     private float referencePitch, referenceRoll, referenceYaw;

//     void Start()
//     {
//         udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 25667));
//         udpClient.EnableBroadcast = true;

//         receiveThread = new Thread(ReceiveData);
//         receiveThread.IsBackground = true;
//         receiveThread.Start();
//         Debug.Log("Hand With Orientation UDP Receiver started...");

//         // Cache base local rotations for all finger joints
//         CacheBaseRotation(indexA); CacheBaseRotation(indexB); CacheBaseRotation(indexC);
//         CacheBaseRotation(middleA); CacheBaseRotation(middleB); CacheBaseRotation(middleC);
//         CacheBaseRotation(ringA); CacheBaseRotation(ringB); CacheBaseRotation(ringC);
//         CacheBaseRotation(thumbA); CacheBaseRotation(thumbB); CacheBaseRotation(thumbC);
//         CacheBaseRotation(pinkyA); CacheBaseRotation(pinkyB); CacheBaseRotation(pinkyC);

//         if (targetObject != null)
//             baseOrientation = targetObject.localRotation;
//     }

//     void LateUpdate()
//     {
//         float[] fingers = new float[4];
//         float localPitch, localRoll, localYaw;
//         bool refSet;

//         lock (dataLock)
//         {
//             curls.CopyTo(fingers, 0);
//             localPitch = pitch;
//             localRoll = roll;
//             localYaw = yaw;
//             refSet = orientationReferenceSet;
//         }

//         // Apply finger curls
//         ApplyCurlXOnly(indexA, indexCurlA, fingers[1]);
//         ApplyCurlXOnly(indexB, indexCurlB, fingers[1]);
//         ApplyCurlXOnly(indexC, indexCurlC, fingers[1]);

//         ApplyCurlXOnly(middleA, middleCurlA, fingers[2]);
//         ApplyCurlXOnly(middleB, middleCurlB, fingers[2]);
//         ApplyCurlXOnly(middleC, middleCurlC, fingers[2]);

//         ApplyCurlXOnly(ringA, ringCurlA, fingers[3]);
//         ApplyCurlXOnly(ringB, ringCurlB, fingers[3]);
//         ApplyCurlXOnly(ringC, ringCurlC, fingers[3]);

//         ApplyCurlXOnly(thumbA, thumbCurlA, fingers[0]);
//         ApplyCurlXOnly(thumbB, thumbCurlB, fingers[0]);
//         ApplyCurlXOnly(thumbC, thumbCurlC, fingers[0]);

//         float pinkyCurl = fingers[3] * 0.7f;
//         ApplyCurlXOnly(pinkyA, pinkyCurlA, pinkyCurl);
//         ApplyCurlXOnly(pinkyB, pinkyCurlB, pinkyCurl);
//         ApplyCurlXOnly(pinkyC, pinkyCurlC, pinkyCurl);

//         // Apply relative orientation
//         if (targetObject != null && refSet)
//         {
//             float deltaPitch = localPitch - referencePitch;
//             float deltaRoll = localRoll - referenceRoll;
//             float deltaYaw = localYaw - referenceYaw;

//             Quaternion deltaRotation = Quaternion.Euler(
//                 -deltaYaw,        // pitch from ESP = X-axis
//                 -deltaRoll,      // yaw from ESP = Y-axis, inverted
//                 deltaPitch       // roll from ESP = Z-axis
//             );

//             targetObject.localRotation = baseOrientation * deltaRotation;
//         }
//     }

//     private void CacheBaseRotation(Transform joint)
//     {
//         if (joint != null)
//             baseRotations[joint] = joint.localRotation;
//     }

//     private void ApplyCurlXOnly(Transform joint, float maxAngle, float curlAmount)
//     {
//         if (joint == null || !baseRotations.ContainsKey(joint)) return;

//         Quaternion baseRot = baseRotations[joint];
//         Quaternion xRotation = Quaternion.Euler(-maxAngle * curlAmount, 0f, 0f);
//         joint.localRotation = baseRot * xRotation;
//     }

//     private void ReceiveData()
//     {
//         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
//         while (isRunning)
//         {
//             try
//             {
//                 byte[] data = udpClient.Receive(ref remoteEP);
//                 string message = Encoding.ASCII.GetString(data);
//                 string[] tokens = message.Split(',');

//                 if (tokens.Length >= 7)
//                 {
//                     float newPitch = float.Parse(tokens[0]);
//                     float newRoll = float.Parse(tokens[1]);
//                     float newYaw = float.Parse(tokens[2]);

//                     float[] newCurls = new float[4];
//                     for (int i = 0; i < 4; i++)
//                     {
//                         if (int.TryParse(tokens[3 + i], out int val))
//                             newCurls[i] = Mathf.Clamp01(val / 100f);
//                     }

//                     lock (dataLock)
//                     {
//                         if (!orientationReferenceSet)
//                         {
//                             referencePitch = newPitch;
//                             referenceRoll = newRoll;
//                             referenceYaw = newYaw;
//                             orientationReferenceSet = true;
//                             Debug.Log("Orientation reference set: " +
//                                       $"Pitch={referencePitch}, Roll={referenceRoll}, Yaw={referenceYaw}");
//                         }

//                         pitch = newPitch;
//                         roll = newRoll;
//                         yaw = newYaw;
//                         newCurls.CopyTo(curls, 0);
//                     }
//                 }
//             }
//             catch (SocketException ex)
//             {
//                 Debug.Log("Socket exception: " + ex.Message);
//                 break;
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         isRunning = false;
//         udpClient?.Close();
//         receiveThread?.Join();
//     }
// }
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class HandWithOrientationController : MonoBehaviour
{
    [Header("Finger Transforms")]
    public Transform indexA, indexB, indexC;
    public Transform middleA, middleB, middleC;
    public Transform ringA, ringB, ringC;
    public Transform thumbA, thumbB, thumbC;
    public Transform pinkyA, pinkyB, pinkyC;

    [Header("Target Object for Orientation")]
    public Transform targetObject;

    [Header("Max Curl Angles (X-axis, inward curl)")]
    public float indexCurlA = 45f, indexCurlB = 60f, indexCurlC = 40f;
    public float middleCurlA = 50f, middleCurlB = 65f, middleCurlC = 45f;
    public float ringCurlA = 50f, ringCurlB = 60f, ringCurlC = 40f;
    public float thumbCurlA = 40f, thumbCurlB = 50f, thumbCurlC = 35f;
    public float pinkyCurlA = 30f, pinkyCurlB = 55f, pinkyCurlC = 35f;

    [Header("Joystick Settings")]
    public float joystickSensitivity = 0.002f;  // public variable for tuning movement
    public int joystickCenter = 2047;           // midpoint of joystick range

    // Parsed values
    private float[] curls = new float[4]; // thumb, index, middle, ring
    private float pitch, roll, yaw;
    public int joystickX = 0;
    public int joystickY = 0;

    private readonly object dataLock = new object();
    private UdpClient udpClient;
    private Thread receiveThread;
    private bool isRunning = true;

    private Dictionary<Transform, Quaternion> baseRotations = new Dictionary<Transform, Quaternion>();
    private Quaternion baseOrientation;

    private bool orientationReferenceSet = false;
    private float referencePitch, referenceRoll, referenceYaw;

    private Vector3 initialPosition;

    void Start()
    {
        udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 25667));
        udpClient.EnableBroadcast = true;

        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("Hand With Orientation UDP Receiver started...");

        CacheBaseRotation(indexA); CacheBaseRotation(indexB); CacheBaseRotation(indexC);
        CacheBaseRotation(middleA); CacheBaseRotation(middleB); CacheBaseRotation(middleC);
        CacheBaseRotation(ringA); CacheBaseRotation(ringB); CacheBaseRotation(ringC);
        CacheBaseRotation(thumbA); CacheBaseRotation(thumbB); CacheBaseRotation(thumbC);
        CacheBaseRotation(pinkyA); CacheBaseRotation(pinkyB); CacheBaseRotation(pinkyC);

        if (targetObject != null)
        {
            baseOrientation = targetObject.localRotation;
            initialPosition = targetObject.localPosition; // store original position
        }
    }

    void LateUpdate()
    {
        float[] fingers = new float[4];
        float localPitch, localRoll, localYaw;
        int joyX, joyY;
        bool refSet;

        lock (dataLock)
        {
            curls.CopyTo(fingers, 0);
            localPitch = pitch;
            localRoll = roll;
            localYaw = yaw;
            joyX = joystickX;
            joyY = joystickY;
            refSet = orientationReferenceSet;
        }

        // Apply finger curls
        ApplyCurlXOnly(indexA, indexCurlA, fingers[1]);
        ApplyCurlXOnly(indexB, indexCurlB, fingers[1]);
        ApplyCurlXOnly(indexC, indexCurlC, fingers[1]);

        ApplyCurlXOnly(middleA, middleCurlA, fingers[2]);
        ApplyCurlXOnly(middleB, middleCurlB, fingers[2]);
        ApplyCurlXOnly(middleC, middleCurlC, fingers[2]);

        ApplyCurlXOnly(ringA, ringCurlA, fingers[3]);
        ApplyCurlXOnly(ringB, ringCurlB, fingers[3]);
        ApplyCurlXOnly(ringC, ringCurlC, fingers[3]);

        ApplyCurlXOnly(thumbA, thumbCurlA, fingers[0]);
        ApplyCurlXOnly(thumbB, thumbCurlB, fingers[0]);
        ApplyCurlXOnly(thumbC, thumbCurlC, fingers[0]);

        float pinkyCurl = fingers[3] * 0.7f;
        ApplyCurlXOnly(pinkyA, pinkyCurlA, pinkyCurl);
        ApplyCurlXOnly(pinkyB, pinkyCurlB, pinkyCurl);
        ApplyCurlXOnly(pinkyC, pinkyCurlC, pinkyCurl);

        // Apply orientation
        if (targetObject != null && refSet)
        {
            float deltaPitch = localPitch - referencePitch;
            float deltaRoll = localRoll - referenceRoll;
            float deltaYaw = localYaw - referenceYaw;

            Quaternion deltaRotation = Quaternion.Euler(
                -deltaYaw,
                -deltaRoll,
                deltaPitch
            );

            targetObject.localRotation = baseOrientation * deltaRotation;

            // Calculate joystick-based movement
            float deltaZ = (joyY - joystickCenter) * joystickSensitivity *0.01f;
            float deltaX = (joyX - joystickCenter) * joystickSensitivity *0.01f;

            targetObject.localPosition = initialPosition + new Vector3(deltaX, 0f, -deltaZ);
        }
    }

    private void CacheBaseRotation(Transform joint)
    {
        if (joint != null)
            baseRotations[joint] = joint.localRotation;
    }

    private void ApplyCurlXOnly(Transform joint, float maxAngle, float curlAmount)
    {
        if (joint == null || !baseRotations.ContainsKey(joint)) return;

        Quaternion baseRot = baseRotations[joint];
        Quaternion xRotation = Quaternion.Euler(-maxAngle * curlAmount, 0f, 0f);
        joint.localRotation = baseRot * xRotation;
    }

    private void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        while (isRunning)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                string[] tokens = message.Split(',');

                if (tokens.Length >= 9)
                {
                    float newPitch = float.Parse(tokens[0]);
                    float newRoll = float.Parse(tokens[1]);
                    float newYaw = float.Parse(tokens[2]);

                    float[] newCurls = new float[4];
                    for (int i = 0; i < 4; i++)
                        newCurls[i] = Mathf.Clamp01(float.Parse(tokens[3 + i]) / 100f);

                    int joyX = int.Parse(tokens[7]);
                    int joyY = int.Parse(tokens[8]);

                    lock (dataLock)
                    {
                        pitch = newPitch;
                        roll = newRoll;
                        yaw = newYaw;
                        curls = newCurls;
                        joystickX = joyX;
                        joystickY = joyY;

                        if (!orientationReferenceSet)
                        {
                            referencePitch = newPitch;
                            referenceRoll = newRoll;
                            referenceYaw = newYaw;
                            orientationReferenceSet = true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("UDP Receive Error: " + ex.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        udpClient?.Close();
        receiveThread?.Abort();
    }
}

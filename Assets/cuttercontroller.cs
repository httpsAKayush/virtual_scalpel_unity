/*using UnityEngine;

public class CutterController : MonoBehaviour
{
    public GameObject objectToCut;
    public Transform cuttingPlaneTransform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("🪓 Cut command issued!");

            if (objectToCut == null || cuttingPlaneTransform == null)
            {
                Debug.LogError("❌ objectToCut or cuttingPlaneTransform is missing!");
                return;
            }

            Plane cutPlane = new Plane(cuttingPlaneTransform.up, cuttingPlaneTransform.position);
            GameObject newRemaining = GetComponent<MeshCutter>().Cut(objectToCut, cutPlane);

            if (newRemaining != null)
            {
                objectToCut = newRemaining;
            }
            else
            {
                Debug.Log("🛑 No remaining mesh to continue cutting.");
            }
        }
    }
}



using UnityEngine;

public class CutterController : MonoBehaviour
{
    public GameObject objectToCut;
    public Transform cuttingPlaneTransform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("🪓 Cut command issued!");

            if (objectToCut == null || cuttingPlaneTransform == null)
            {
                Debug.LogError("❌ objectToCut or cuttingPlaneTransform is missing!");
                return;
            }

            Plane cutPlane = new Plane(cuttingPlaneTransform.up, cuttingPlaneTransform.position);
            GameObject newRemaining = GetComponent<MeshCutter>().Cut(objectToCut, cutPlane);

            if (newRemaining != null)
            {
                objectToCut = newRemaining;
            }
            else
            {
                Debug.Log("🛑 No remaining mesh to continue cutting.");
            }
        }
    }
}





using UnityEngine;

public class CutterController : MonoBehaviour
{
    public GameObject objectToCut;
    public Transform cuttingPlaneTransform;
    public Transform scalpelTipTransform;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("🪓 Cut command issued!");

            if (objectToCut == null || cuttingPlaneTransform == null || scalpelTipTransform == null)
            {
                Debug.LogError("❌ objectToCut, cuttingPlaneTransform, or scalpelTipTransform is missing!");
                return;
            }

            Plane cutPlane = new Plane(cuttingPlaneTransform.up, cuttingPlaneTransform.position);
            Vector3 scalpelTip = scalpelTipTransform.position;

            GameObject newRemaining = GetComponent<MeshCutter>().Cut(objectToCut, cutPlane, scalpelTip);

            if (newRemaining != null)
            {
                objectToCut = newRemaining;
            }
            else
            {
                Debug.Log("🛑 No remaining mesh to continue cutting.");
            }
        }
    }
}

*/

using UnityEngine;

public class CutterController : MonoBehaviour
{
    public GameObject objectToCut;
    public Transform cuttingPlaneTransform;
    public Transform scalpelTipTransform;
    public Camera mainCamera;

    private bool isDragging = false;
    private Vector3 lastHitPoint;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                lastHitPoint = hit.point;
                cuttingPlaneTransform.position = hit.point;
                cuttingPlaneTransform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            if (objectToCut == null || cuttingPlaneTransform == null)
            {
                Debug.LogError("❌ Missing reference!");
                return;
            }

            Plane cutPlane = new Plane(cuttingPlaneTransform.up, cuttingPlaneTransform.position);
            Vector3 scalpelTip = scalpelTipTransform != null ? scalpelTipTransform.position : lastHitPoint;

            GameObject newRemaining = GetComponent<MeshCutter>().Cut(objectToCut, cutPlane, scalpelTip);

            if (newRemaining != null)
                objectToCut = newRemaining;
        }
    }
}



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

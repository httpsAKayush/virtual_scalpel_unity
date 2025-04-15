using UnityEngine;

public class CutterController : MonoBehaviour
{
    public GameObject objectToCut;
    public Transform cuttingPlaneTransform;
    public Transform scalpelTipTransform;
    public float minMoveDistance = 0.001f;

    private Vector3 lastCutPoint;
    private bool hasLastCutPoint = false;

    void Update()
    {
        if (objectToCut == null || cuttingPlaneTransform == null || scalpelTipTransform == null)
        {
            Debug.LogError("❌ Missing reference!");
            return;
        }

        Collider targetCollider = objectToCut.GetComponent<Collider>();
        if (targetCollider == null)
        {
            Debug.LogError("❌ No collider on objectToCut!");
            return;
        }

        Vector3 tipPosition = scalpelTipTransform.position;

        // Check intersection
        bool isIntersecting = targetCollider.bounds.Contains(tipPosition);

        if (isIntersecting)
        {
            float distanceMoved = hasLastCutPoint ? Vector3.Distance(tipPosition, lastCutPoint) : float.MaxValue;

            if (distanceMoved > minMoveDistance)
            {
                Plane cutPlane = new Plane(cuttingPlaneTransform.up, cuttingPlaneTransform.position);

                MeshCutter cutter = GetComponent<MeshCutter>();
                if (cutter == null)
                {
                    Debug.LogError("❌ Missing MeshCutter component!");
                    return;
                }

                GameObject newRemaining = cutter.Cut(objectToCut, cutPlane, tipPosition);

                if (newRemaining != null)
                    objectToCut = newRemaining;

                lastCutPoint = tipPosition;
                hasLastCutPoint = true;
            }
        }
        else
        {
            hasLastCutPoint = false; // Reset once scalpel exits
        }
    }
}
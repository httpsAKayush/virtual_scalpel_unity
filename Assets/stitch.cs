using System.Collections.Generic;
using UnityEngine;

public class StitchOnCollision : MonoBehaviour
{
    public float stitchInterval = 0.05f;
    public float stitchWidth = 0.01f;
    public Material stitchMaterial;

    private Vector3 lastStitchPos;
    private bool firstTouch = true;

    private List<GameObject> stitches = new();

    private void OnTriggerEnter(Collider other)
    {
        TryCreateStitch(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryCreateStitch(other);
    }

    void TryCreateStitch(Collider other)
    {
        Vector3 contactPoint = transform.position;

        if (firstTouch || Vector3.Distance(contactPoint, lastStitchPos) >= stitchInterval)
        {
            firstTouch = false;

            // Get contact surface normal
            Vector3 normal = (contactPoint - other.ClosestPoint(contactPoint)).normalized;
            Vector3 forward = transform.forward;
            Vector3 right = Vector3.Cross(forward, normal).normalized;

            Vector3 p1 = contactPoint + right * stitchWidth;
            Vector3 p2 = contactPoint - right * stitchWidth;

            CreateStitch(p1, p2, normal);
            lastStitchPos = contactPoint;
        }
    }

    void CreateStitch(Vector3 p1, Vector3 p2, Vector3 normal)
    {
        Mesh mesh = new Mesh();

        Vector3 mid = (p1 + p2) / 2f;
        Vector3 offset = Vector3.Cross((p2 - p1).normalized, normal) * stitchWidth;

        Vector3 v0 = p1 + offset;
        Vector3 v1 = p2 + offset;
        Vector3 v2 = mid - offset;

        mesh.vertices = new Vector3[] { v0, v1, v2 };
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.RecalculateNormals();

        GameObject stitch = new GameObject("Stitch", typeof(MeshFilter), typeof(MeshRenderer));
        stitch.GetComponent<MeshFilter>().mesh = mesh;
        stitch.GetComponent<MeshRenderer>().material = stitchMaterial;
        stitches.Add(stitch);
    }
}

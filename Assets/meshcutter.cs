using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    public float cuttingDepth = 0.02f; // How deep the knife cuts

    public GameObject Cut(GameObject obj, Plane cuttingPlane)
    {
        Debug.Log("🔪 Starting mesh cut...");

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        List<Vector3> cutVerts = new();
        List<int> cutTris = new();

        List<Vector3> remainVerts = new();
        List<int> remainTris = new();

        int cutCount = 0;
        int remainCount = 0;

        for (int i = 0; i < tris.Length; i += 3)
        {
            int i0 = tris[i];
            int i1 = tris[i + 1];
            int i2 = tris[i + 2];

            Vector3 v0 = obj.transform.TransformPoint(verts[i0]);
            Vector3 v1 = obj.transform.TransformPoint(verts[i1]);
            Vector3 v2 = obj.transform.TransformPoint(verts[i2]);

            float d0 = Mathf.Abs(cuttingPlane.GetDistanceToPoint(v0));
            float d1 = Mathf.Abs(cuttingPlane.GetDistanceToPoint(v1));
            float d2 = Mathf.Abs(cuttingPlane.GetDistanceToPoint(v2));

            bool allFar = d0 > cuttingDepth && d1 > cuttingDepth && d2 > cuttingDepth;

            if (!allFar)
            {
                AddTriangle(ref cutVerts, ref cutTris, v0, v1, v2);
                cutCount++;
            }
            else
            {
                AddTriangle(ref remainVerts, ref remainTris, v0, v1, v2);
                remainCount++;
            }
        }

        Debug.Log($"✅ Triangles cut: {cutCount}, Remaining: {remainCount}");

        CreatePiece(cutVerts, cutTris, obj, "Cut Part", Color.red);
        GameObject remaining = CreatePiece(remainVerts, remainTris, obj, "Remaining Part", Color.white);

        Destroy(obj);
        return remaining;
    }

    void AddTriangle(ref List<Vector3> verts, ref List<int> tris, Vector3 a, Vector3 b, Vector3 c)
    {
        int index = verts.Count;
        verts.Add(a);
        verts.Add(b);
        verts.Add(c);
        tris.Add(index);
        tris.Add(index + 1);
        tris.Add(index + 2);
    }

    GameObject CreatePiece(List<Vector3> verts, List<int> tris, GameObject source, string name, Color color)
    {
        if (verts.Count == 0 || tris.Count == 0)
        {
            Debug.LogWarning($"⚠️ Skipping creation of {name} — no valid mesh data.");
            return null;
        }

        GameObject go = new GameObject(name);
        Mesh newMesh = new Mesh();

        Vector3[] localVerts = new Vector3[verts.Count];
        for (int i = 0; i < verts.Count; i++)
            localVerts[i] = source.transform.InverseTransformPoint(verts[i]);

        newMesh.vertices = localVerts;
        newMesh.triangles = tris.ToArray();
        newMesh.RecalculateNormals();

        go.transform.position = source.transform.position;
        go.transform.rotation = source.transform.rotation;
        go.transform.localScale = source.transform.localScale;

        var mf = go.AddComponent<MeshFilter>();
        mf.mesh = newMesh;

        var mr = go.AddComponent<MeshRenderer>();
        var mat = new Material(source.GetComponent<MeshRenderer>().material);
        mat.color = color;
        mr.material = mat;

        var mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = newMesh;
        mc.convex = true;

        return go;
    }
}

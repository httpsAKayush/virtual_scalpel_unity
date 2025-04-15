/*using System.Collections.Generic;
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



using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    public GameObject Cut(GameObject obj, Plane cuttingPlane)
    {
        Debug.Log("🔬 Performing surgical mesh cut...");

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        List<Vector3> cutVerts = new();
        List<int> cutTris = new();

        List<Vector3> remainVerts = new();
        List<int> remainTris = new();

        Transform objTransform = obj.transform;

        int closestTriangleIndex = -1;
        float closestHitDistance = float.MaxValue;
        Vector3 bestV0 = Vector3.zero, bestV1 = Vector3.zero, bestV2 = Vector3.zero;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            // Check if triangle intersects plane
            if (!TriangleIntersectsPlane(cuttingPlane, v0, v1, v2))
                continue;

            // Get shortest distance from triangle to plane (via edges)
            float d = GetTriangleToPlaneDistance(cuttingPlane, v0, v1, v2);

            if (d < closestHitDistance)
            {
                closestHitDistance = d;
                closestTriangleIndex = i;
                bestV0 = v0;
                bestV1 = v1;
                bestV2 = v2;
            }
        }

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (i == closestTriangleIndex)
            {
                ClassifyAndSliceTriangle(cuttingPlane, bestV0, bestV1, bestV2, ref cutVerts, ref cutTris, ref remainVerts, ref remainTris);
            }
            else
            {
                AddTriangle(ref remainVerts, ref remainTris, v0, v1, v2);
            }
        }

        Debug.Log($"🩺 Cut 1 precise triangle. Cut part: {cutTris.Count / 3}");

        CreatePiece(cutVerts, cutTris, obj, "Cut Part", Color.red);
        GameObject remaining = CreatePiece(remainVerts, remainTris, obj, "Remaining Part", Color.white);

        Destroy(obj);
        return remaining;
    }

    bool TriangleIntersectsPlane(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);

        return (d0 * d1 < 0) || (d1 * d2 < 0) || (d2 * d0 < 0);
    }

    float GetTriangleToPlaneDistance(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float min = float.MaxValue;

        Vector3[] edges = { v0, v1, v1, v2, v2, v0 };
        for (int i = 0; i < edges.Length; i += 2)
        {
            if (plane.Raycast(new Ray(edges[i], edges[i + 1] - edges[i]), out float distance))
            {
                min = Mathf.Min(min, distance);
            }
        }

        return min;
    }

    void ClassifyAndSliceTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2,
                                   ref List<Vector3> cutVerts, ref List<int> cutTris,
                                   ref List<Vector3> remainVerts, ref List<int> remainTris)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);

        Vector3[] points = new Vector3[] { v0, v1, v2 };
        float[] distances = new float[] { d0, d1, d2 };
        List<Vector3> positive = new();
        List<Vector3> negative = new();

        for (int i = 0; i < 3; i++)
        {
            if (distances[i] >= 0) positive.Add(points[i]);
            else negative.Add(points[i]);
        }

        if (positive.Count == 1)
        {
            Vector3 p = positive[0];
            Vector3 n1 = negative[0];
            Vector3 n2 = negative[1];

            Vector3 i1 = LinePlaneIntersection(plane, p, n1);
            Vector3 i2 = LinePlaneIntersection(plane, p, n2);

            AddTriangle(ref cutVerts, ref cutTris, p, i1, i2);
        }
        else if (positive.Count == 2)
        {
            Vector3 p1 = positive[0];
            Vector3 p2 = positive[1];
            Vector3 n = negative[0];

            Vector3 i1 = LinePlaneIntersection(plane, p1, n);
            Vector3 i2 = LinePlaneIntersection(plane, p2, n);

            AddTriangle(ref cutVerts, ref cutTris, p1, p2, i1);
            AddTriangle(ref cutVerts, ref cutTris, p2, i2, i1);
        }
    }

    Vector3 LinePlaneIntersection(Plane plane, Vector3 a, Vector3 b)
    {
        Vector3 direction = b - a;
        if (plane.Raycast(new Ray(a, direction), out float distance))
        {
            return a + direction.normalized * distance;
        }
        return a;
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
            Debug.LogWarning($"⚠️ Skipping creation of {name} — no mesh.");
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




using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    public GameObject Cut(GameObject obj, Plane cuttingPlane)
    {
        Debug.Log("🔬 Performing surgical mesh cut...");

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        List<Vector3> cutVerts = new();
        List<int> cutTris = new();

        List<Vector3> remainVerts = new();
        List<int> remainTris = new();

        Transform objTransform = obj.transform;

        int closestTriangleIndex = -1;
        float closestPlaneDistance = float.MaxValue;
        Vector3 bestV0 = Vector3.zero, bestV1 = Vector3.zero, bestV2 = Vector3.zero;

        // 🔍 Step 1: Find the single closest triangle intersected by the plane
        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (!TriangleIntersectsPlane(cuttingPlane, v0, v1, v2))
                continue;

            Vector3 centroid = (v0 + v1 + v2) / 3f;
            float distToPlane = Mathf.Abs(cuttingPlane.GetDistanceToPoint(centroid));

            if (distToPlane < closestPlaneDistance)
            {
                closestPlaneDistance = distToPlane;
                closestTriangleIndex = i;
                bestV0 = v0;
                bestV1 = v1;
                bestV2 = v2;
            }
        }

        // 🔧 Step 2: Slice only that triangle
        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (i == closestTriangleIndex)
            {
                ClassifyAndSliceTriangle(cuttingPlane, bestV0, bestV1, bestV2, ref cutVerts, ref cutTris, ref remainVerts, ref remainTris);
            }
            else
            {
                AddTriangle(ref remainVerts, ref remainTris, v0, v1, v2);
            }
        }

        Debug.Log($"✅ Cut 1 precise triangle. Cut part: {cutTris.Count / 3}");

        CreatePiece(cutVerts, cutTris, obj, "Cut Part", Color.red);
        GameObject remaining = CreatePiece(remainVerts, remainTris, obj, "Remaining Part", Color.white);

        Destroy(obj);
        return remaining;
    }

    bool TriangleIntersectsPlane(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);
        return (d0 * d1 < 0) || (d1 * d2 < 0) || (d2 * d0 < 0);
    }

    void ClassifyAndSliceTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2,
                                   ref List<Vector3> cutVerts, ref List<int> cutTris,
                                   ref List<Vector3> remainVerts, ref List<int> remainTris)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);

        Vector3[] points = new Vector3[] { v0, v1, v2 };
        float[] distances = new float[] { d0, d1, d2 };
        List<Vector3> positive = new();
        List<Vector3> negative = new();

        for (int i = 0; i < 3; i++)
        {
            if (distances[i] >= 0) positive.Add(points[i]);
            else negative.Add(points[i]);
        }

        if (positive.Count == 1)
        {
            Vector3 p = positive[0];
            Vector3 n1 = negative[0];
            Vector3 n2 = negative[1];

            Vector3 i1 = LinePlaneIntersection(plane, p, n1);
            Vector3 i2 = LinePlaneIntersection(plane, p, n2);

            AddTriangle(ref cutVerts, ref cutTris, p, i1, i2);
        }
        else if (positive.Count == 2)
        {
            Vector3 p1 = positive[0];
            Vector3 p2 = positive[1];
            Vector3 n = negative[0];

            Vector3 i1 = LinePlaneIntersection(plane, p1, n);
            Vector3 i2 = LinePlaneIntersection(plane, p2, n);

            AddTriangle(ref cutVerts, ref cutTris, p1, p2, i1);
            AddTriangle(ref cutVerts, ref cutTris, p2, i2, i1);
        }
    }

    Vector3 LinePlaneIntersection(Plane plane, Vector3 a, Vector3 b)
    {
        Vector3 direction = b - a;
        if (plane.Raycast(new Ray(a, direction), out float distance))
        {
            return a + direction.normalized * distance;
        }
        return a; // fallback
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
            Debug.LogWarning($"⚠️ Skipping creation of {name} — no mesh.");
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


using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    public GameObject Cut(GameObject obj, Plane cuttingPlane)
    {
        Debug.Log("✂️ Precise mesh cut started...");

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        List<Vector3> cutVerts = new();
        List<int> cutTris = new();

        List<Vector3> remainVerts = new();
        List<int> remainTris = new();

        Transform objTransform = obj.transform;

        int closestIndex = -1;
        float minDist = float.MaxValue;
        Vector3 bestV0 = Vector3.zero, bestV1 = Vector3.zero, bestV2 = Vector3.zero;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (TrianglePlaneIntersection(cuttingPlane, v0, v1, v2))
            {
                float dist = Mathf.Abs(cuttingPlane.GetDistanceToPoint((v0 + v1 + v2) / 3));
                if (dist < minDist)
                {
                    minDist = dist;
                    closestIndex = i;
                    bestV0 = v0;
                    bestV1 = v1;
                    bestV2 = v2;
                }
            }
        }

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (i == closestIndex)
            {
                SliceTriangle(cuttingPlane, bestV0, bestV1, bestV2, ref cutVerts, ref cutTris, ref remainVerts, ref remainTris);
            }
            else
            {
                AddTriangle(ref remainVerts, ref remainTris, v0, v1, v2);
            }
        }

        Debug.Log($"✅ One triangle precisely sliced.");

        CreatePiece(cutVerts, cutTris, obj, "Cut Part", Color.red);
        GameObject remaining = CreatePiece(remainVerts, remainTris, obj, "Remaining Part", Color.white);

        Destroy(obj);
        return remaining;
    }

    bool TrianglePlaneIntersection(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);
        return (d0 * d1 < 0) || (d1 * d2 < 0) || (d2 * d0 < 0);
    }

    void SliceTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2,
                       ref List<Vector3> cutVerts, ref List<int> cutTris,
                       ref List<Vector3> remainVerts, ref List<int> remainTris)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);

        List<Vector3> above = new();
        List<Vector3> below = new();

        if (d0 >= 0) above.Add(v0); else below.Add(v0);
        if (d1 >= 0) above.Add(v1); else below.Add(v1);
        if (d2 >= 0) above.Add(v2); else below.Add(v2);

        if (above.Count == 1)
        {
            Vector3 a = above[0];
            Vector3 b = below[0];
            Vector3 c = below[1];

            Vector3 ab = LinePlaneIntersection(plane, a, b);
            Vector3 ac = LinePlaneIntersection(plane, a, c);

            AddTriangle(ref cutVerts, ref cutTris, a, ab, ac);
        }
        else if (above.Count == 2)
        {
            Vector3 a = above[0];
            Vector3 b = above[1];
            Vector3 c = below[0];

            Vector3 ca = LinePlaneIntersection(plane, c, a);
            Vector3 cb = LinePlaneIntersection(plane, c, b);

            AddTriangle(ref cutVerts, ref cutTris, a, b, cb);
            AddTriangle(ref cutVerts, ref cutTris, a, cb, ca);
        }
    }

    Vector3 LinePlaneIntersection(Plane plane, Vector3 a, Vector3 b)
    {
        Vector3 dir = b - a;
        if (plane.Raycast(new Ray(a, dir), out float dist))
        {
            return a + dir.normalized * dist;
        }
        return a;
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
            Debug.LogWarning($"⚠️ Skipping {name} — no valid mesh.");
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

        go.AddComponent<MeshFilter>().mesh = newMesh;

        var mat = new Material(source.GetComponent<MeshRenderer>().material);
        mat.color = color;

        go.AddComponent<MeshRenderer>().material = mat;

        var mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = newMesh;
        mc.convex = true;

        return go;
    }
}




using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    public GameObject Cut(GameObject obj, Plane cuttingPlane, Vector3 scalpelTip)
    {
        Debug.Log("✂️ Precise mesh cut started...");

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        List<Vector3> cutVerts = new();
        List<int> cutTris = new();

        List<Vector3> remainVerts = new();
        List<int> remainTris = new();

        Transform objTransform = obj.transform;

        int closestIndex = -1;
        float minDist = float.MaxValue;
        Vector3 bestV0 = Vector3.zero, bestV1 = Vector3.zero, bestV2 = Vector3.zero;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (TrianglePlaneIntersection(cuttingPlane, v0, v1, v2))
            {
                Vector3 triangleCenter = (v0 + v1 + v2) / 3f;
                float dist = Vector3.Distance(scalpelTip, triangleCenter);

                if (dist < minDist)
                {
                    minDist = dist;
                    closestIndex = i;
                    bestV0 = v0;
                    bestV1 = v1;
                    bestV2 = v2;
                }
            }
        }

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (i == closestIndex)
            {
                SliceTriangle(cuttingPlane, bestV0, bestV1, bestV2, ref cutVerts, ref cutTris, ref remainVerts, ref remainTris);
            }
            else
            {
                AddTriangle(ref remainVerts, ref remainTris, v0, v1, v2);
            }
        }

        Debug.Log($"✅ One triangle precisely sliced.");

        CreatePiece(cutVerts, cutTris, obj, "Cut Part", Color.red);
        GameObject remaining = CreatePiece(remainVerts, remainTris, obj, "Remaining Part", Color.white);

        Destroy(obj);
        return remaining;
    }

    bool TrianglePlaneIntersection(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);
        return (d0 * d1 < 0) || (d1 * d2 < 0) || (d2 * d0 < 0);
    }

    void SliceTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2,
                       ref List<Vector3> cutVerts, ref List<int> cutTris,
                       ref List<Vector3> remainVerts, ref List<int> remainTris)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);

        List<Vector3> above = new();
        List<Vector3> below = new();

        if (d0 >= 0) above.Add(v0); else below.Add(v0);
        if (d1 >= 0) above.Add(v1); else below.Add(v1);
        if (d2 >= 0) above.Add(v2); else below.Add(v2);

        if (above.Count == 1)
        {
            Vector3 a = above[0];
            Vector3 b = below[0];
            Vector3 c = below[1];

            Vector3 ab = LinePlaneIntersection(plane, a, b);
            Vector3 ac = LinePlaneIntersection(plane, a, c);

            AddTriangle(ref cutVerts, ref cutTris, a, ab, ac);
        }
        else if (above.Count == 2)
        {
            Vector3 a = above[0];
            Vector3 b = above[1];
            Vector3 c = below[0];

            Vector3 ca = LinePlaneIntersection(plane, c, a);
            Vector3 cb = LinePlaneIntersection(plane, c, b);

            AddTriangle(ref cutVerts, ref cutTris, a, b, cb);
            AddTriangle(ref cutVerts, ref cutTris, a, cb, ca);
        }
    }

    Vector3 LinePlaneIntersection(Plane plane, Vector3 a, Vector3 b)
    {
        Vector3 dir = b - a;
        if (plane.Raycast(new Ray(a, dir), out float dist))
        {
            return a + dir.normalized * dist;
        }
        return a;
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
            Debug.LogWarning($"⚠️ Skipping {name} — no valid mesh.");
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

        go.AddComponent<MeshFilter>().mesh = newMesh;

        var mat = new Material(source.GetComponent<MeshRenderer>().material);
        mat.color = color;

        go.AddComponent<MeshRenderer>().material = mat;

        var mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = newMesh;
        mc.convex = true;

        return go;
    }
}




*/

using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    public GameObject Cut(GameObject obj, Plane cuttingPlane, Vector3 scalpelTip)
    {
        Debug.Log("✂️ Precise mesh cut started...");

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        List<Vector3> cutVerts = new();
        List<int> cutTris = new();

        List<Vector3> remainVerts = new();
        List<int> remainTris = new();

        Transform objTransform = obj.transform;

        int closestIndex = -1;
        float minDist = float.MaxValue;
        Vector3 bestV0 = Vector3.zero, bestV1 = Vector3.zero, bestV2 = Vector3.zero;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (TrianglePlaneIntersection(cuttingPlane, v0, v1, v2))
            {
                Vector3 triangleCenter = (v0 + v1 + v2) / 3f;
                float dist = Vector3.Distance(scalpelTip, triangleCenter);

                if (dist < minDist)
                {
                    minDist = dist;
                    closestIndex = i;
                    bestV0 = v0;
                    bestV1 = v1;
                    bestV2 = v2;
                }
            }
        }

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 v0 = objTransform.TransformPoint(verts[tris[i]]);
            Vector3 v1 = objTransform.TransformPoint(verts[tris[i + 1]]);
            Vector3 v2 = objTransform.TransformPoint(verts[tris[i + 2]]);

            if (i == closestIndex)
            {
                SliceTriangle(cuttingPlane, bestV0, bestV1, bestV2, ref cutVerts, ref cutTris, ref remainVerts, ref remainTris);
            }
            else
            {
                AddTriangle(ref remainVerts, ref remainTris, v0, v1, v2);
            }
        }

        Debug.Log($"✅ One triangle precisely sliced.");

        GameObject cutPart = CreatePiece(cutVerts, cutTris, obj, "Cut Part", Color.red);
        GameObject remaining = CreatePiece(remainVerts, remainTris, obj, "Remaining Part", Color.white);

        if (cutPart != null)
            Destroy(cutPart);

        Destroy(obj);
        return remaining;
    }

    bool TrianglePlaneIntersection(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);
        return (d0 * d1 < 0) || (d1 * d2 < 0) || (d2 * d0 < 0);
    }

    void SliceTriangle(Plane plane, Vector3 v0, Vector3 v1, Vector3 v2,
                       ref List<Vector3> cutVerts, ref List<int> cutTris,
                       ref List<Vector3> remainVerts, ref List<int> remainTris)
    {
        float d0 = plane.GetDistanceToPoint(v0);
        float d1 = plane.GetDistanceToPoint(v1);
        float d2 = plane.GetDistanceToPoint(v2);

        List<Vector3> above = new();
        List<Vector3> below = new();

        if (d0 >= 0) above.Add(v0); else below.Add(v0);
        if (d1 >= 0) above.Add(v1); else below.Add(v1);
        if (d2 >= 0) above.Add(v2); else below.Add(v2);

        if (above.Count == 1)
        {
            Vector3 a = above[0];
            Vector3 b = below[0];
            Vector3 c = below[1];

            Vector3 ab = LinePlaneIntersection(plane, a, b);
            Vector3 ac = LinePlaneIntersection(plane, a, c);

            AddTriangle(ref cutVerts, ref cutTris, a, ab, ac);
        }
        else if (above.Count == 2)
        {
            Vector3 a = above[0];
            Vector3 b = above[1];
            Vector3 c = below[0];

            Vector3 ca = LinePlaneIntersection(plane, c, a);
            Vector3 cb = LinePlaneIntersection(plane, c, b);

            AddTriangle(ref cutVerts, ref cutTris, a, b, cb);
            AddTriangle(ref cutVerts, ref cutTris, a, cb, ca);
        }
    }

    Vector3 LinePlaneIntersection(Plane plane, Vector3 a, Vector3 b)
    {
        Vector3 dir = b - a;
        if (plane.Raycast(new Ray(a, dir), out float dist))
        {
            return a + dir.normalized * dist;
        }
        return a;
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
            Debug.LogWarning($"⚠️ Skipping {name} — no valid mesh.");
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

        go.AddComponent<MeshFilter>().mesh = newMesh;

        var mat = new Material(source.GetComponent<MeshRenderer>().material);
        mat.color = color;
        go.AddComponent<MeshRenderer>().material = mat;

        var mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = newMesh;
        mc.convex = true;

        return go;
    }
}

using UnityEngine;
using System.Collections.Generic;

public static class MeshGenerator
{
    public static void GenerateMesh(bool[,,] voxels, float voxelSize, GameObject target)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int size = voxels.GetLength(0);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (voxels[x, y, z])
                    {
                        AddCube(vertices, triangles, new Vector3(x, y, z) * voxelSize, voxelSize);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshFilter filter = target.GetComponent<MeshFilter>();
        if (filter == null) filter = target.AddComponent<MeshFilter>();
        filter.mesh = mesh;
    }

    static void AddCube(List<Vector3> vertices, List<int> triangles, Vector3 position, float size)
    {
        Vector3[] cubeVerts = {
            new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0),
            new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1)
        };

        int[] cubeTris = {
            0, 2, 1, 2, 0, 3,  // Front
            4, 5, 6, 6, 7, 4,  // Back
            0, 1, 5, 5, 4, 0,  // Bottom
            2, 3, 7, 7, 6, 2,  // Top
            0, 4, 7, 7, 3, 0,  // Left
            1, 2, 6, 6, 5, 1   // Right
        };

        int indexStart = vertices.Count;
        for (int i = 0; i < cubeVerts.Length; i++)
        {
            vertices.Add(position + cubeVerts[i] * size);
        }
        for (int i = 0; i < cubeTris.Length; i++)
        {
            triangles.Add(indexStart + cubeTris[i]);
        }
    }
}

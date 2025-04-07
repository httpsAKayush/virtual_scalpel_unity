using UnityEngine;
using System.Collections.Generic;

public class VoxelGrid : MonoBehaviour
{
    public int resolution = 32;  // Voxel grid resolution
    public float voxelSize = 0.1f;
    private bool[,,] voxels;  // Store solid/empty voxels

    private Vector3 minBounds, maxBounds;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Bounds bounds = meshFilter.mesh.bounds;
        minBounds = transform.TransformPoint(bounds.min);
        maxBounds = transform.TransformPoint(bounds.max);

        voxels = new bool[resolution, resolution, resolution];
        VoxelizeMesh(meshFilter.mesh);
    }
void Update()
{
    if (Input.GetKeyDown(KeyCode.Space))  // Press Space to cut
    {
        VoxelGrid voxelGrid = FindObjectOfType<VoxelGrid>();
        if (voxelGrid != null)
        {
            voxelGrid.Cut(transform.position, 2f); // Cut at the GameObject's position with a radius of 0.5
        }
    }
}

    void VoxelizeMesh(Mesh mesh)
    {
        Vector3 size = maxBounds - minBounds;
        float step = Mathf.Max(size.x, size.y, size.z) / resolution;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    Vector3 voxelCenter = minBounds + new Vector3(x, y, z) * step;
                    if (IsPointInsideMesh(voxelCenter, mesh))
                    {
                        voxels[x, y, z] = true;
                    }
                }
            }
        }

        GenerateMesh();
    }

    bool IsPointInsideMesh(Vector3 point, Mesh mesh)
    {
        Ray ray = new Ray(point + Vector3.up * 10f, Vector3.down);
        return Physics.Raycast(ray, out _); // Odd number of hits means inside the object
    }

    void GenerateMesh()
    {
        // Generate new mesh from voxel data
        MeshGenerator.GenerateMesh(voxels, voxelSize, gameObject);
    }
     public void Cut(Vector3 position, float radius)
    {
        Vector3 size = maxBounds - minBounds;
        float step = Mathf.Max(size.x, size.y, size.z) / resolution;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    Vector3 voxelCenter = minBounds + new Vector3(x, y, z) * step;
                    if (Vector3.Distance(voxelCenter, position) < radius)
                    {
                        voxels[x, y, z] = false;  // Mark voxel as removed
                    }
                }
            }
        }

        GenerateMesh(); // Update the mesh after cutting
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Voxelizer : MonoBehaviour
{
    public GameObject anatomyModel; // Assign your model in the Inspector
    public float voxelSize = 0.5f; // Adjust voxel size for performance
    public int maxSize = 100; // Limits voxel grid size to prevent memory overflow

    private Dictionary<Vector3, bool> voxelData = new Dictionary<Vector3, bool>();
    private int sizeX, sizeY, sizeZ;

    void Start()
    {
        StartCoroutine(GenerateVoxels()); // Run voxel generation step-by-step
    }

    IEnumerator GenerateVoxels()
    {
        Bounds combinedBounds = CalculateBounds(anatomyModel);
        sizeX = Mathf.Min(Mathf.CeilToInt(combinedBounds.size.x / voxelSize), maxSize);
        sizeY = Mathf.Min(Mathf.CeilToInt(combinedBounds.size.y / voxelSize), maxSize);
        sizeZ = Mathf.Min(Mathf.CeilToInt(combinedBounds.size.z / voxelSize), maxSize);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    Vector3 worldPos = combinedBounds.min + new Vector3(x, y, z) * voxelSize;
                    if (IsPointInsideModel(worldPos, anatomyModel))
                    {
                        voxelData[new Vector3(x, y, z)] = true; // Store only filled voxels
                    }
                }
            }
            yield return null; // Prevents Unity from freezing
        }

        GenerateMesh(); // Call the mesh generation after voxels are processed
    }

    void GenerateMesh()
    {
        bool[,,] voxelArray = new bool[sizeX, sizeY, sizeZ];
        foreach (var voxel in voxelData)
        {
            Vector3 pos = voxel.Key;
            voxelArray[(int)pos.x, (int)pos.y, (int)pos.z] = true;
        }

        GameObject voxelObject = new GameObject("VoxelizedModel");
        voxelObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = voxelObject.AddComponent<MeshRenderer>();
        
        // Assign a basic material
        renderer.material = new Material(Shader.Find("Standard"));
        
        // Set position to match the anatomy model
        voxelObject.transform.position = anatomyModel.transform.position;
        
        MeshGenerator.GenerateMesh(voxelArray, voxelSize, voxelObject);
    }

    Bounds CalculateBounds(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    bool IsPointInsideModel(Vector3 point, GameObject model)
    {
        RaycastHit hit;
        Vector3 direction = Vector3.up;
        return Physics.Raycast(new Ray(point - direction * 10, direction), out hit, 20f) && hit.collider.gameObject == model;
    }
}


/*


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Voxelizer : MonoBehaviour
{
    public GameObject anatomyModel; // Assign your model in the Inspector
    public float voxelSize = 0.5f; // Adjust voxel size for performance
    public int maxSize = 100; // Limits voxel grid size to prevent memory overflow
    public Material voxelMaterial; // Assign a material in the Inspector

    private Dictionary<Vector3, bool> voxelData = new Dictionary<Vector3, bool>();
    private int sizeX, sizeY, sizeZ;
    private GameObject voxelObject;

    void Start()
    {
        StartCoroutine(GenerateVoxels()); // Run voxel generation step-by-step
    }

    IEnumerator GenerateVoxels()
    {
        Bounds combinedBounds = CalculateBounds(anatomyModel);
        sizeX = Mathf.Min(Mathf.CeilToInt(combinedBounds.size.x / voxelSize), maxSize);
        sizeY = Mathf.Min(Mathf.CeilToInt(combinedBounds.size.y / voxelSize), maxSize);
        sizeZ = Mathf.Min(Mathf.CeilToInt(combinedBounds.size.z / voxelSize), maxSize);

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    Vector3 worldPos = combinedBounds.min + new Vector3(x, y, z) * voxelSize;
                    if (IsPointInsideModel(worldPos, anatomyModel))
                    {
                        voxelData[new Vector3(x, y, z)] = true; // Store only filled voxels
                    }
                }
            }
            yield return null; // Prevents Unity from freezing
        }

        GenerateMesh(); // Call the mesh generation after voxels are processed
    }

    void GenerateMesh()
    {
        if (voxelObject != null) Destroy(voxelObject); // Remove old mesh before updating

        bool[,,] voxelArray = new bool[sizeX, sizeY, sizeZ];
        foreach (var voxel in voxelData)
        {
            Vector3 pos = voxel.Key;
            voxelArray[(int)pos.x, (int)pos.y, (int)pos.z] = true;
        }

        voxelObject = new GameObject("VoxelizedModel");
        voxelObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = voxelObject.AddComponent<MeshRenderer>();

        renderer.material = voxelMaterial ?? new Material(Shader.Find("Standard"));
        voxelObject.transform.position = anatomyModel.transform.position;

        MeshGenerator.GenerateMesh(voxelArray, voxelSize, voxelObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click: Cut
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                CutVoxel(hit.point);
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right Click: Deform
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                DeformVoxel(hit.point, Vector3.up);
            }
        }
    }

    void CutVoxel(Vector3 worldPosition)
    {
        Vector3 localPos = (worldPosition - CalculateBounds(anatomyModel).min) / voxelSize;
        Vector3 roundedPos = new Vector3(Mathf.Round(localPos.x), Mathf.Round(localPos.y), Mathf.Round(localPos.z));

        if (voxelData.ContainsKey(roundedPos))
        {
            voxelData.Remove(roundedPos);
            GenerateMesh();
        }
    }

    void DeformVoxel(Vector3 worldPosition, Vector3 forceDirection)
    {
        Vector3 localPos = (worldPosition - CalculateBounds(anatomyModel).min) / voxelSize;
        Vector3 roundedPos = new Vector3(Mathf.Round(localPos.x), Mathf.Round(localPos.y), Mathf.Round(localPos.z));

        if (voxelData.ContainsKey(roundedPos))
        {
            Vector3 newPos = roundedPos + forceDirection * 0.5f;
            voxelData.Remove(roundedPos);
            voxelData[newPos] = true;
            GenerateMesh();
        }
    }

    Bounds CalculateBounds(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(Vector3.zero, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    bool IsPointInsideModel(Vector3 point, GameObject model)
    {
        RaycastHit hit;
        Vector3 direction = Vector3.up;
        return Physics.Raycast(new Ray(point - direction * 10, direction), out hit, 20f) && hit.collider.gameObject == model;
    }
}





*/
using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private GameObject chunkObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public ushort[] voxelMap = new ushort[VoxelData.chunkSize];

    private bool isDirty;

    int verticesIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector3> normals = new List<Vector3>();
    
    World world;

    public ushort this[int x, int y, int z]
    {
        get { return voxelMap[x * VoxelData.chunkWidth * VoxelData.chunkHeight + y * VoxelData.chunkWidth + z]; }
        set { voxelMap[x * VoxelData.chunkWidth * VoxelData.chunkHeight + y * VoxelData.chunkWidth + z] = value; isDirty = true; }
    }

    public Chunk(World _world, Vector3 pos)
    {
        world = _world;

        //GameObject that will hold chunk
        chunkObject = new GameObject("Chunk " + pos.x + ", " + pos.z);

        position = pos;

        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.chunkMaterial;

        FillVoxelMap();
        CreateMeshData();
        DrawMesh();
    }

    Vector3 position
    {
        get { return chunkObject.transform.position; }
        set { chunkObject.transform.position = value; }
    }

    //setter and getter for active state of chunk
    public bool IsActive
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    //clear mesh data
    private void ClearMeshData()
    {
        verticesIndex = 0;
        vertices.Clear();
        normals.Clear();
        triangles.Clear();
    }

    //check if voxel exists in this chunk
    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.chunkWidth - 1 || y < 0 || y > VoxelData.chunkWidth - 1 || z < 0 || z > VoxelData.chunkWidth - 1)
            return false;
        else
            return true;
    }

    //check if voxel type is solid
    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.voxelTypes[world.GetVoxel(position + pos)].isSolid;

        //return true if voxel is solid
        return world.voxelTypes[this[x, y, z]].isSolid;
    }

    //populate voxel map with data
    private void FillVoxelMap()
    {
        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    this[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }
    }

    //create mesh from voxel map data
    private void CreateMeshData()
    {
        ClearMeshData();

        for (int x = 0; x < VoxelData.chunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.chunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.chunkWidth; z++)
                {
                    //skip blocks that aren't solid
                    if (!world.voxelTypes[this[x, y, z]].isSolid)
                        continue;

                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    //add data for each voxel to chunk
    private void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int i = 0; i < VoxelData.TOTAL_CUBE_FACES; i++)
        {
            Vector3 adjacentVoxel = pos + VoxelData.faceChecks[i];

            //draw voxel face if adjacent voxel is not a solid
            if (!CheckVoxel(adjacentVoxel))
            {
                for (int j = 0; j < VoxelData.TOTAL_INDICES; j++)
                {
                    int triangleIndex = VoxelData.voxelTris[i, j];
                    vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);
                    normals.Add(VoxelData.voxelNormals[triangleIndex]);
                }

                foreach (var tri in VoxelData.voxelTrisOrder)
                    triangles.Add(verticesIndex + tri);

                verticesIndex += VoxelData.TOTAL_INDICES;
            }
        }
    }

    public void DrawMesh()
    {
        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh; 
    }
}

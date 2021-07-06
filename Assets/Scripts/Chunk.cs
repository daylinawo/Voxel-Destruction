using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private MeshFilter meshFilter;

    public ushort[] voxelMap = new ushort[VoxelData.chunkSizeCubed];

    private bool isDirty;

    int verticesIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector3> normals = new List<Vector3>();

    private System.Random random = new System.Random();

    public ushort this[int x, int y, int z]
    {
        get { return voxelMap[x * VoxelData.chunkSize * VoxelData.chunkSize + y * VoxelData.chunkSize + z]; }
        set { voxelMap[x * VoxelData.chunkSize * VoxelData.chunkSize + y * VoxelData.chunkSize + z] = value; isDirty = true; }
    }

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        for (int x = 0; x < VoxelData.chunkSize; x++)
        {
            for (int y = 0; y < VoxelData.chunkSize; y++)
            {
                for (int z = 0; z < VoxelData.chunkSize; z++)
                {
                    //var voxelType = (ushort)random.Next(0, 2);
                    this[x, y, z] = 1;
                }
            }
        }
    }

    private void Update()
    {
        if (isDirty)
        {
            CreateMeshData();
            DrawMesh();

            foreach(var vert in vertices)
                Debug.Log(vert);

            isDirty = false;
        }
    }
    private void CreateMeshData()
    {
        verticesIndex = 0;
        vertices.Clear();
        normals.Clear();
        triangles.Clear();

        for (int x = 0; x < VoxelData.chunkSize; x++)
        {
            for (int y = 0; y < VoxelData.chunkSize; y++)
            {
                for (int z = 0; z < VoxelData.chunkSize; z++)
                {
                    var voxelType = this[x, y, z];

                    //if it is air ignore this block
                    if (voxelType == 0)
                        continue;

                    AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    //check if voxel at specified position exists
    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.chunkSize - 1 || y < 0 || y > VoxelData.chunkSize - 1 || z < 0 || z > VoxelData.chunkSize - 1)
            return false;

        //return true if voxel is solid
        return this[x, y, z] != 0;
    }

    //add data for each voxel to chunk
    private void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int i = 0; i < VoxelData.TOTAL_CUBE_FACES; i++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[i]))
            {
                for (int j = 0; j < VoxelData.TOTAL_INDICES; j++)
                {
                    int triangleIndex = VoxelData.voxelTris[i, j];
                    vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);
                    normals.Add(VoxelData.voxelNormals[triangleIndex]);
                }

                foreach(var tri in VoxelData.voxelTrisOrder)
                    triangles.Add(verticesIndex + tri);

                verticesIndex += VoxelData.TOTAL_INDICES;
            }
        }

        //foreach(var vert in VoxelData.voxelVerts)
        //    vertices.Add(vert + pos);

        //foreach(var tri in VoxelData.voxelTris)
        //    triangles.Add(tri + verticesIndex);

        //foreach (var normal in VoxelData.voxelNormals)
        //    normals.Add(normal);

        //verticesIndex = vertices.Count;

        ////entire faces
        //for(int i = 0; i < 6; i++)
        //{
        //    //optimisation to reduce number of faces/triangles
        //    //only draw face if adjacent voxel is air
        //    if (!CheckVoxel(pos + VoxelData.faceChecks[i]))
        //    {
        //        //each vertex
        //        for (int j = 0; j < 6; j++)
        //        {
        //index referencing a voxel vertex
        //            int triangleIndex = VoxelData.voxelTris[i, j];
        //offset voxel vert by a position
        //            vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);
        //            normals.Add(VoxelData.voxelNormals[triangleIndex]);
        //            triangles.Add(verticesIndex);

        //            verticesIndex++;
        //        }
        //    }
        //}
    }

    public void DrawMesh()
    {
        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        //mesh.SetNormals(normals);
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh; 
    }
}

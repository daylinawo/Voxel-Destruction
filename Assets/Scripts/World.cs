using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material chunkMaterial;
    public VoxelType[] voxelTypes;

    public Dictionary<ChunkID, Chunk> chunks = new Dictionary<ChunkID, Chunk>();
    private System.Random random = new System.Random();


    //get and set voxel in chunk
    public ushort this[int x, int y, int z]
    {
        get 
        {
            var chunk = chunks[ChunkID.FromWorldPos(x, y, z)];
            return chunk[x & 0x0F, y & 0x0F, z & 0x0F];
        }
        
        set 
        {
            var chunk = chunks[ChunkID.FromWorldPos(x, y, z)];
            chunk[x & 0x0F, y & 0x0F, z & 0x0F] = value;
        }
    }

    private void Start()
    {
        CreateWorld();
    }

    void CreateWorld()
    {
        for(int x = 0; x < VoxelData.worldSizeInChunks; x++)
        {
            for (int z = 0; z < VoxelData.worldSizeInChunks; z++)
            {
                CreateChunk(VoxelData.chunkSize * x, VoxelData.chunkSize * z);
            }
        }
    }

    void CreateChunk(int x, int z)
    {
        //Create a new chunk
        Chunk chunk = new Chunk(this, new Vector3(x, 0, z));

        //Add chunk to dictionary for referencing
        chunks.Add(new ChunkID(x, 0, z), chunk);
    }

  
    public bool IsChunkInWorld(Vector3 chunkPos)
    {
        int x = Mathf.FloorToInt(chunkPos.x);
        int y = Mathf.FloorToInt(chunkPos.y);
        int z = Mathf.FloorToInt(chunkPos.z);
        
        Chunk chunk = chunks[ChunkID.FromWorldPos(x, y, z)];

        if (chunk != null)
            return false;
        else
            return true;
    }

    public bool IsVoxelInWorld(Vector3 voxelPos)
    {
        int x = Mathf.FloorToInt(voxelPos.x);
        int y = Mathf.FloorToInt(voxelPos.y);
        int z = Mathf.FloorToInt(voxelPos.z);

        if (x >= 0 && x < VoxelData.worldSizeInVoxel &&
            y >= 0 && y < VoxelData.chunkHeight &&
            z >= 0 && z < VoxelData.worldSizeInVoxel)
            return true;
        else
            return false;
    }

    //returns a voxel type
    public ushort GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos))
            return 0;

        return 1;
    }
}

[System.Serializable]
public struct VoxelType
{
    public string voxelName;
    public bool isSolid;
}
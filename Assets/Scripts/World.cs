using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material chunkMaterial;
    public VoxelType[] voxelTypes;

    public Transform player;
    private Vector3 spawnPoint;

    public Dictionary<ChunkID, Chunk> chunks = new Dictionary<ChunkID, Chunk>();

    //get and set voxel in chunk
    public ushort this[int x, int y, int z]
    {
        get 
        {
            var chunk = chunks[ChunkID.FromWorldPos(x, y, z)];
            Vector3Int voxelPos = GetVoxelChunkPosition(x, y, z);
            return chunk[voxelPos.x, voxelPos.y, voxelPos.z];
        }
        
        set 
        {
            var chunk = chunks[ChunkID.FromWorldPos(x, y, z)];
            Vector3Int voxelPos = GetVoxelChunkPosition(x, y, z);
            chunk[voxelPos.x, voxelPos.y, voxelPos.z] = value;
        }
    }

    private void Start()
    {
        spawnPoint = new Vector3(VoxelData.worldSizeInVoxel / 2f, VoxelData.chunkHeight, VoxelData.worldSizeInVoxel / 2f);
        CreateWorld();
    }

    //returns voxel position from array in 3D coordinates 
    public Vector3Int GetVoxelChunkPosition(int x, int y, int z)
    {
        return new Vector3Int(x & 0xF, y & 0x3F, z & 0xF);
    }

    public void PrintVoxelInfo(int x, int y, int z)
    {
        Debug.Log("Voxel Position: " + x + ", " + y + ", " + z + 
                  "\nVoxel type: " + voxelTypes[this[x, y, z]].voxelName);
    }

    public void CreateWorld()
    {
        for(int x = 0; x < VoxelData.worldSizeInChunks; x++)
        {
            for (int z = 0; z < VoxelData.worldSizeInChunks; z++)
            {
                CreateChunk(VoxelData.chunkWidth * x, VoxelData.chunkWidth * z);
            }
        }

        player.position = spawnPoint;
    }

    public void CreateChunk(int x, int z)
    {
        Chunk chunk = new Chunk(this, new Vector3(x, 0, z));

        //Add chunk to dictionary for referencing
        chunks.Add(new ChunkID(x, 0, z), chunk);
    }

    public bool IsVoxelSolid(float _x, float _y, float _z)
    {

        //position
        int x = Mathf.FloorToInt(_x);
        int y = Mathf.FloorToInt(_y);
        int z = Mathf.FloorToInt(_z);

        ChunkID chunkID = ChunkID.FromWorldPos(x, y, z);

        if (!chunks.ContainsKey(ChunkID.FromWorldPos(x, y, z)))
        {
           // Debug.Log("Chunk \"" + chunkID.x + ", " + chunkID.y + ", " + chunkID.z + "\"  is not in world.");
            return false;
        }

        //Debug.Log("Chunk \"" + id.x + ", " + id.y + ", " + id.z + "\"  is in world.");
        //Vector3Int voxelPos = GetVoxelPositionInChunk(x, y, z);
        //PrintVoxelInfo(voxelPos.x, voxelPos.y, voxelPos.z);
        
        return voxelTypes[this[x, y, z]].isSolid;

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
        int yPos = Mathf.FloorToInt(pos.y);

        //if outside world, voxel is air
        if (!IsVoxelInWorld(pos))
            return 0;
        
        int terrainHeight = Mathf.FloorToInt(VoxelData.chunkHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 1100, 0.4f));

        if (yPos <= terrainHeight)
            return 1;
        else
            return 0;
    }
}

[System.Serializable]
public struct VoxelType
{
    public string voxelName;
    public bool isSolid;
}
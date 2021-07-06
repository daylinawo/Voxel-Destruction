using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material chunkMaterial;

    public Dictionary<ChunkID, Chunk> chunks = new Dictionary<ChunkID, Chunk>();
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
        Chunk chunk = new Chunk(this, new Vector3(0, 0, 0));
    }
}

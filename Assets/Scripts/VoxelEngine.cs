using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelEngine : MonoBehaviour
{
    private World world = new World();
    public Material chunkMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject that will hold chunk
        var chunkGameObject = new GameObject("Chunk: 0, 0, 0");

        chunkGameObject.transform.parent = transform.parent;

        //Add chunk to GameObject
        var chunk = chunkGameObject.AddComponent<Chunk>();

        //Add chunk to world at position 0 0 0
        world.chunks.Add(new ChunkID(0, 0, 0), chunk);

        chunkGameObject.GetComponent<MeshRenderer>().material = chunkMaterial;

        foreach(ChunkID key in world.chunks.Keys)
        {
            Vector3 _key = new Vector3(key.x, key.y, key.z);
            Debug.Log("Chunk ID: " + _key);
        }



    }

    // Update is called once per frame
    void Update()
    {


    }
}

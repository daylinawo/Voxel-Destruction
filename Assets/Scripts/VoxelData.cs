using UnityEngine;

public static class VoxelData
{
    public static readonly int chunkWidth = 16;
    public static readonly int chunkHeight = 64;
    public static readonly int worldSizeInChunks = 4;
    public static readonly byte voxelXShift = 4;
    public static readonly byte voxelYShift = 6;
    public static readonly byte voxelZShift = 4;

    public static int chunkSize
    {
        get { return chunkWidth * chunkHeight * chunkWidth; }
    }

    public static int worldSizeInVoxel
    {
        get { return chunkWidth * worldSizeInChunks; }
    }
    public const int TOTAL_CUBE_FACES = 6;
    public const int TOTAL_INDICES = 4;
    
    public static readonly int TotalVoxelTypes = 2;

    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        //Front
        new Vector3(0, 0, 0), //Bottom Left
        new Vector3(1, 0, 0), //Bottom Right
        new Vector3(1, 1, 0), //Top Right
        new Vector3(0, 1, 0), //Top Left

        //Back
        new Vector3(0, 1, 1), //Top Left
        new Vector3(1, 1, 1), //Top Right
        new Vector3(0, 0, 1), //Bottom Left 
        new Vector3(1, 0, 1)  //Bottom Right 
    };

    public static readonly int[,] voxelTris = new int[TOTAL_CUBE_FACES, TOTAL_INDICES]
    {
        { 0, 3, 1, 2 }, //Front Face
        { 3, 4, 2, 5 }, //Top Face
        { 6, 4, 0, 3 }, //Left Face
        { 1, 2, 7, 5 }, //Right Face
        { 7, 5, 6, 4 }, //Back Face
        { 6, 0, 7, 1 }  //Bottom Face
    };
    
    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3 ( 0, 0, -1 ), //Front
        new Vector3 ( 0, 1, 0 ), //Top
        new Vector3 ( -1, 0, 0 ), //Left
        new Vector3 ( 1, 0, 0 ), //Right
        new Vector3 ( 0, 0, 1 ), //Back
        new Vector3 ( 0, -1, 0 )  //Bottom
    };    
    
    public static readonly Vector3[] chunkSides = new Vector3[6]
    {
        new Vector3 ( 0, 0, -1 ) * chunkSize, //Front
        new Vector3 ( 0, 1, 0 ) * chunkSize, //Top
        new Vector3 ( -1, 0, 0 ) * chunkSize, //Left
        new Vector3 ( 1, 0, 0 ) * chunkSize, //Right
        new Vector3 ( 0, 0, 1 ) * chunkSize, //Back
        new Vector3 ( 0, -1, 0 ) * chunkSize  //Bottom
    };

    public static readonly Vector3[] voxelNormals = new[]
    {
        Vector3.up, Vector3.up,
        Vector3.up, Vector3.up,       
        Vector3.up, Vector3.up,
        Vector3.up, Vector3.up,
    };

    public static readonly int[] voxelTrisOrder = new int[6]
    {
        0, 1, 2, 2, 1, 3
    };
}

using System;
using UnityEngine;

public struct ChunkID : IEquatable<ChunkID>
{
    public readonly int x, y, z;

    public ChunkID(int X, int Y, int Z)
    {
        x = X >> 4;
        y = Y >> 4;
        z = Z >> 4;
    }
    public bool Equals(ChunkID other)
    {
        if (other == null) return false;
        return x == other.x && y == other.y && z == other.z;
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return Equals(obj is ChunkID);
    }

    public static bool operator ==(ChunkID lhs, ChunkID rhs)
    {
        return lhs.Equals(rhs);
    }    
    
    public static bool operator !=(ChunkID lhs, ChunkID rhs)
    {
        return !lhs.Equals(rhs);
    }

    public static ChunkID FromWorldPos(int x, int y, int z)
    {
        return new ChunkID(x, y, z);
    }

    public override int GetHashCode()
    {
        //dont care about overflow or underflow, just want the value
        unchecked
        {
            var hashCode = x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            hashCode = (hashCode * 397) ^ z.GetHashCode();
            return hashCode;
        }
    }

    public static void DisplayID(int x, int y, int z)
    {
        Debug.Log(FromWorldPos(x, y, z));
    }
}

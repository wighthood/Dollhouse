using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GeneratedDoorData : INetworkSerializable
{
    public Vector2 pos;
    public bool[] entriesStates=new bool[4];

    public GeneratedDoorData()
    {
        entriesStates=new bool[4];
    }
    
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref pos);
        serializer.SerializeValue(ref entriesStates);
    }
}

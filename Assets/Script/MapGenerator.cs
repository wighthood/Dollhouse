using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapGenerator : NetworkBehaviour
{
    
    
    public List<Node> nodes = new List<Node>();
    
    // 1 to take account of the root node
    int nbRoom = 1;
    [SerializeField] int maxNbRoom = 60;
    List<Vector2> AllNodesPos = new List<Vector2>();

    [SerializeField]
    RoomDoorsRef NodeCube;


    [Header("SPOOKY STUFF")]
    [SerializeField] private GameObject creeper;
    [SerializeField] private float creeperChance;
    
    private Node root;
    
    private Dictionary<Entries, Vector2> EntriesPosValues = new Dictionary<Entries, Vector2>();
    
    private void Start()
    {
        if (IsServer)
        {
            GenerateMapServerRPC();
        }
    }
    
    void GenerateMap(Node node,Vector3 pos, Entries entrance)
    {
        if (nbRoom >= maxNbRoom) return;

        List<KeyValuePair<Entries, DoorState>> entryBuffer = new List<KeyValuePair<Entries, DoorState>>(node.entryIsOpen);
        
        for (int i = 0; i < entryBuffer.Count; i++)
        {
            if (entryBuffer[i].Key == entrance)
            {
                entryBuffer.Remove(entryBuffer[i]);
                i--;
            }
        }
        
        List<KeyValuePair<Entries, DoorState>> buffer = new List<KeyValuePair<Entries, DoorState>>(entryBuffer);
        
        int doorsToCheck = entryBuffer.Count;

        for (int i = 0; i < doorsToCheck; i++)
        {
            int index = Random.Range(0, buffer.Count);
            KeyValuePair<Entries, DoorState> currentEntry = buffer[index];
            buffer.RemoveAt(index);
            if (currentEntry.Value == DoorState.Open)
            {
                foreach (var child in node.childs)
                {
                    if (child.parentIsFrom==FindOppositeDoor((int)currentEntry.Key))
                    {
                        GenerateMap(child, pos, FindOppositeDoor((int)currentEntry.Key));
                        break;
                    }
                }
                entryBuffer.Remove(currentEntry);
            }
        }
        
        if (Random.Range(0, 2) == 0 && node.parent != null)
        {
            return;
        }
        doorsToCheck=entryBuffer.Count;
        
        for (int i = 0; i < doorsToCheck; i++)
        {
            int index = Random.Range(0, entryBuffer.Count);
            KeyValuePair<Entries, DoorState> currentEntry = entryBuffer[index];
            entryBuffer.RemoveAt(index);
            if (currentEntry.Value == DoorState.Stucked)
            {
                continue;
            }
            int r = Random.Range(0, 2);
            if (r == 0)
            {
                continue;
            }
            if (!NodeCanSpawn(node, currentEntry.Key))
            {
                node.entryIsOpen[currentEntry.Key] = DoorState.Stucked;
                continue;
            }
            node.entryIsOpen[currentEntry.Key] = DoorState.Open;
            nbRoom++;
            //generate room
            Node newChild = new Node();
            newChild.parent = node;
            newChild.entryIsOpen[FindOppositeDoor((int)currentEntry.Key)] = DoorState.Open;
            newChild.parentIsFrom = FindOppositeDoor((int)currentEntry.Key);
            node.childs.Add(newChild);
            newChild.name = "Salle" + nbRoom;
            newChild.nodePos = node.nodePos + EntriesPosValues[currentEntry.Key];
            newChild.lustreBehaviour = (LustreBehaviour)Random.Range(0, 4);
            nodes.Add(newChild);
            AllNodesPos.Add(newChild.nodePos);
            GenerateMap(newChild,pos, FindOppositeDoor(i));
            
        }
        
    }
    

    [Rpc(SendTo.Server)]
    void GenerateMapServerRPC()
    {
        EntriesPosValues.Add(Entries.Up, Vector2.up);
        EntriesPosValues.Add(Entries.Right, Vector2.right);
        EntriesPosValues.Add(Entries.Down, Vector2.down);
        EntriesPosValues.Add(Entries.Left, Vector2.left);

        root = new Node();
        root.nodeObject=NodeCube.gameObject;
        root.name = "root";
        nodes.Add(root);
        root.entryIsOpen[Entries.Down] = DoorState.Stucked;
        AllNodesPos.Add(root.nodePos);
        
        while (nbRoom < maxNbRoom)
        {
            GenerateMap(root, NodeCube.transform.position, Entries.Down);
        }
        GeneratedDoorData[] data = new GeneratedDoorData[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            data[i] = new GeneratedDoorData
            {
                pos = nodes[i].nodePos,
                lustreBehaviour = nodes[i].lustreBehaviour,
            };
            foreach (var entry in nodes[i].entryIsOpen)
            {
                if (entry.Value == DoorState.Open)
                {
                    data[i].entriesStates[(int)entry.Key] = false;
                }
                else
                {
                    data[i].entriesStates[(int)entry.Key] = true;
                }
            }
        }
        CreateClientRoomsRPC(data);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    void CreateClientRoomsRPC(GeneratedDoorData[] data)
    {
            RoomDoorsRef roomInfo;
            for (int i = 0; i < data.Length; i++)
            {
                //always taking into account the root
                if (i > 0)
                {
                    GameObject newRoom=Instantiate(NodeCube.gameObject, Vector3.zero, Quaternion.identity);
                    roomInfo = newRoom.GetComponent<RoomDoorsRef>();
                    newRoom.transform.position=new Vector3(data[i].pos.x*roomInfo.roomSize,0,data[i].pos.y*roomInfo.roomSize);
                    newRoom.name = "Room " +  i;
                }
                else
                {
                    roomInfo = NodeCube;
                }

                switch (data[i].lustreBehaviour)
                {
                    case LustreBehaviour.Moving :
                        continue;
                    break;
                    case LustreBehaviour.None :
                        roomInfo.lightScript.DeactivateAnim(); 
                        break;
                    case LustreBehaviour.Off:
                        roomInfo.lightScript.SetLight(0);
                        break;
                    case LustreBehaviour.OffNotMoving:
                        roomInfo.lightScript.DeactivateAnim();
                        roomInfo.lightScript.SetLight(0);
                        break;
                    default:
                        break;
                    
                }
            
                for (int j = 0; j < 4; j++)
                {
                    roomInfo.entryGOs[j].SetActive(data[i].entriesStates[j]);
                    if (data[i].lustreBehaviour == LustreBehaviour.Off ||
                        data[i].lustreBehaviour == LustreBehaviour.OffNotMoving)
                    {
                        roomInfo.lightScript.SetDoorsLight(j,!data[i].entriesStates[j]);
                    }
                }
            }
    }
    
    bool NodeCanSpawn(Node node, Entries entry)
    {
        return !AllNodesPos.Contains(node.nodePos + EntriesPosValues[entry]);

    }
    Node NodeParkour(Node node)
    {
        Node a = new Node();
        if (node.childs.Count > 0)
        {
            foreach (Node child in node.childs)
            {
                NodeParkour(child);
            }
        }
        return a;
    }

    Entries FindOppositeDoor(int index)
    {
        if (index + 2 >= 4)
        {

            return ((Entries)index - 2);

        }
        return ((Entries)index + 2);
    }
}



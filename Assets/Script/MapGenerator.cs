using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : NetworkBehaviour
{
    
    
    public List<Node> nodes = new List<Node>();
    int nbRoom = 0;
    int maxNbRoom = 60;
    List<Vector2> dunemaniereouduneautre = new List<Vector2>();

    [SerializeField]
    RoomDoorsRef NodeCube;
    
    private Node root;
    
    
    Dictionary<Entries,Vector2> EntriesPosValues = new Dictionary<Entries,Vector2>();


    private void Start()
    {
        EntriesPosValues.Add(Entries.Up, Vector2.up);
        EntriesPosValues.Add(Entries.Right, Vector2.right);
        EntriesPosValues.Add(Entries.Down, Vector2.down);
        EntriesPosValues.Add(Entries.Left, Vector2.left);

        root = new Node(NodeCube.gameObject);
        root.name = "root";
        nodes.Add(root);
        root.entryIsOpen[Entries.Down] = DoorState.Stucked;
        root.nodeObject.GetComponent<RoomDoorsRef>().OpenCloseEntry(Entries.Down, DoorState.Stucked);
        dunemaniereouduneautre.Add(root.nodePos);
        Color b = Color.black;


        while (nbRoom < maxNbRoom)
        {
            GenerateMap(root, NodeCube.transform.position, Entries.Down, b);
            b.r += .2f;

        }

        for (int i = 0; i < nodes.Count; i++)
        {
            foreach (var entry in nodes[i].entryIsOpen)
            {
                nodes[i].nodeObject.GetComponent<RoomDoorsRef>().OpenCloseEntry(entry.Key, entry.Value);
            }
        }
    }



    void GenerateMap(Node node,Vector3 pos, Entries entrance, Color colour)
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
                        GenerateMap(child, pos, FindOppositeDoor((int)currentEntry.Key),new Color(colour.r,0,colour.b+.2f));
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
                //node.nodeObject.GetComponent<RoomDoorsRef>().OpenCloseEntry(currentEntry.Key,DoorState.Stucked);
                continue;
            }
            
            node.entryIsOpen[currentEntry.Key] = DoorState.Open;
            //node.nodeObject.GetComponent<RoomDoorsRef>().OpenCloseEntry(currentEntry.Key,DoorState.Open);
            
            nbRoom++;
            
            //generer salle
            GameObject newRoom=Instantiate(NodeCube.gameObject, pos, Quaternion.identity);
            newRoom.name = "Room " + nbRoom;
            Node newChild = new Node(newRoom);
            newChild.parent = node;
            newChild.entryIsOpen[FindOppositeDoor((int)currentEntry.Key)] = DoorState.Open;
            //newChild.nodeObject.GetComponent<RoomDoorsRef>().OpenCloseEntry(FindOppositeDoor((int)currentEntry.Key),DoorState.Open);
            newChild.parentIsFrom = FindOppositeDoor((int)currentEntry.Key);
            node.childs.Add(newChild);
            newChild.name = "Salle" + nbRoom;
            newChild.nodePos = node.nodePos + EntriesPosValues[currentEntry.Key];
            nodes.Add(newChild);
            dunemaniereouduneautre.Add(newChild.nodePos);
            newChild.nodeObject.transform.position+=new Vector3(newChild.nodePos.x,0,newChild.nodePos.y)*NodeCube.roomSize;
            //newChild.nodeObject.GetComponent<MeshRenderer>().material.color = colour;
            GenerateMap(newChild,pos, FindOppositeDoor(i), colour);
            
            
/*
            Vector2 a = node.nodePos + EntriesPosValues[(Entries)i];
            float rslt=Mathf.Atan2(a.y,a.x);
            rslt *= Mathf.Rad2Deg;
            Debug.Log("angle : "+rslt);
            
            if (Mathf.Abs(rslt - angleTarget) > maxAngle)
            {
                continue;
            }
            
            
            node.entryIsOpen[(Entries)i] = false;
            
            
            if (nbMaxRoom <= nbRoom)
            {
                return;
            }
            

            if (i + 2 >= 4)
            {
                newChild.entryIsOpen[(Entries)i-2]=false;

            }
            else
            {
                newChild.entryIsOpen[(Entries)i+2]=false;
            }

            
            newRoom.name = "Salle" + nbRoom;
            Debug.Log(newChild.name);
            Debug.Log("nbroom : " + nbRoom);
            nodes.Add(newChild);
         //   Debug.Log("total : " + totalDoors);
             
            GenerateMap(newChild,pos, nbMaxRoom);
            if (node.parent == null)
            {
                angleTarget -= 90;
                if (angleTarget < 0)
                {
                    angleTarget = 180;
                }
            }*/
            
        }
        /*
        while (node.parent == null && nbRoom < nbMaxRoom)
        {
            Dictionary<int,Entries> indexBuffer=new Dictionary<int,Entries>();
            angleTarget = 90;


            List<int> indexBuffer2=new List<int>();
            for (int i = nodes.Count-1; i >=0 ; i--)
            {
                if (nodes[i].parent != null)
                {
                    float rslt=Mathf.Atan2(nodes[i].nodePos.y,nodes[i].nodePos.x);
                    rslt *= Mathf.Rad2Deg;
                    Debug.Log(rslt);
                    if (Mathf.Abs(rslt - angleTarget) > maxAngle)
                    {
                        Destroy(nodes[i].nodeObject);
                    }
                }
            }

            for (int i = 0; i < indexBuffer2.Count; i++)
            {
                nodes.RemoveAt(indexBuffer2[i]);
                nbRoom--;
            }
            for (int i = 0; i < nodes.Count; i++)
            {

                
                foreach (var entry1 in nodes[i].entryIsOpen.Where(entry=>entry.Value==true))
                {
                    if (entry1.Value)
                    {
                        for (int j = i; j < nodes.Count; j++)
                        {
                            foreach (var entry2 in nodes[j].entryIsOpen.Where(entry=>entry.Value==true))
                            {
                                if (entry2.Value)
                                {
                                    if (nodes[i].nodePos + EntriesPosValues[entry1.Key] ==
                                        nodes[j].nodePos + EntriesPosValues[entry2.Key] || 
                                        nodes[i].nodePos == 
                                        nodes[j].nodePos + EntriesPosValues[entry2.Key] || 
                                        nodes[j].nodePos == nodes[i].nodePos + EntriesPosValues[entry2.Key])
                                    {
                                        if (!indexBuffer.ContainsKey(i))
                                        {
                                            indexBuffer.Add(i,entry1.Key);
                                        }
                                        if (!indexBuffer.ContainsKey(j))
                                        {
                                            indexBuffer.Add(j,entry2.Key);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }

            foreach (var index in indexBuffer)
            {
                nodes[index.Key].entryIsOpen[index.Value]=false;
            }

            Node nodeTocheck = node;
            angleTarget = 0;

            nodeTocheck = parkour(nodeTocheck);
            
            GenerateMap(nodeTocheck,Vector3.zero, nbMaxRoom);

            
        }
        */
    }

    bool NodeCanSpawn(Node node, Entries entry)
    {
        return !dunemaniereouduneautre.Contains(node.nodePos + EntriesPosValues[entry]);

    }
    Node NodeParkour(Node node)
    {
        Node a = new Node(NodeCube.gameObject);
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



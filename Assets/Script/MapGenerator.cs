using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : NetworkBehaviour
{
    private Node root = new Node();
    private List<Node> nodes = new List<Node>();
    private void Start()
    {
        root.name = "root";
        nodes.Add(root);
        GenerateMap(root);
        Debug.Log(root.name);
        Debug.Log(root.childs.Count);
        Node test = root;
        while (test.childs.Count > 0)
        {
            Debug.Log(test.name);
            test = test.childs[0];
        }
    }
    
/// <summary>
/// generate new doors and update the total number of doors
/// </summary>
/// <param name="totalPortes">
/// total number of doors to update
/// </param>
/// <returns>
/// Returns the number of newly generated doors
/// </returns>
    int GenerateDoors(ref int totalDoors)
    {
        int generatedDoors;
        generatedDoors=Random.Range(0,4);
        totalDoors += generatedDoors;
        
        return generatedDoors;
    }

    void GenerateMap(Node node,int nbDoors=0, int totalDoors=0, int nbMaxRoom=20, int nbRoom=0)
    {
        if (node.parent == null)
        {
            nbDoors=GenerateDoors(ref totalDoors);
        }

        for (int i = 0; i < nbDoors; i++)
        {
            if (nbRoom >= nbMaxRoom)
            {
                return;
            }
            //generer salle
            Node newChild=new Node();
            newChild.parent=node;
            node.childs.Add(newChild);
            nbRoom++;
            newChild.name ="Salle"+nbRoom;
            nodes.Add(newChild);
            totalDoors -= 1;
            if (Random.Range(0, 2)==0)
            {
                return;
            }

            nbDoors = GenerateDoors(ref totalDoors);
            if (nbRoom + totalDoors > nbMaxRoom)
            {
                nbDoors=nbRoom+totalDoors-nbMaxRoom;
            }
            GenerateMap(newChild,nbDoors, totalDoors, nbMaxRoom, nbRoom);
        }
    }
    
}

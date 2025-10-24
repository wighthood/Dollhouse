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
        Node Test = root;
        while (Test.childs.Count > 0)
        {
            Debug.Log(Test.name);
            Test = Test.childs[0];
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
    int GenerateDoors(ref int totalPortes)
    {
        int generatedDoors;
        generatedDoors=Random.Range(0,4);
        totalPortes += generatedDoors;
        
        return generatedDoors;
    }

    void GenerateMap(Node noeud,int nbPortes=0, int totalPortes=0, int nbMaxSalles=20, int nbSalle=0)
    {
        if (noeud.parent == null)
        {
            nbPortes=GenerateDoors(ref totalPortes);
        }

        for (int i = 0; i < nbPortes; i++)
        {
            if (nbSalle >= nbMaxSalles)
            {
                return;
            }
            //generer salle
            Node newChild=new Node();
            newChild.parent=noeud;
            noeud.childs.Add(newChild);
            nbSalle++;
            newChild.name ="Salle"+nbSalle;
            nodes.Add(newChild);
            totalPortes -= 1;
            if (Random.Range(0, 2)==0)
            {
                return;
            }

            nbPortes = GenerateDoors(ref totalPortes);
            if (nbSalle + totalPortes > nbMaxSalles)
            {
                nbPortes=nbSalle+totalPortes-nbMaxSalles;
            }
            GenerateMap(newChild,nbPortes,totalPortes,nbMaxSalles,nbSalle);
        }
    }
    
}

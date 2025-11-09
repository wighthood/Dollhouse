using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Entries
{
    Up=0,
    Right=1,
    Down=2,
    Left=3
}

public enum DoorState
{
    Open,
    Closed,
    Stucked
}

[Serializable]
public class Node
{
    public Node()
    {
        entryIsOpen.Add(Entries.Up,DoorState.Closed);
        entryIsOpen.Add(Entries.Right,DoorState.Closed);
        entryIsOpen.Add(Entries.Down,DoorState.Closed);
        entryIsOpen.Add(Entries.Left,DoorState.Closed);
        lustreBehaviour = LustreBehaviour.Moving;
        colour=Color.black;
    }
    public Node parent;
    [SerializeReference]
    public List<Node> childs=new List<Node>();

    public Vector2 nodePos=Vector2.zero;
    

    
    public Dictionary<Entries, DoorState> entryIsOpen = new Dictionary<Entries, DoorState>();
    public Entries parentIsFrom;
    public GameObject nodeObject;
    public string name;
    public Color colour;
    public LustreBehaviour lustreBehaviour;
}

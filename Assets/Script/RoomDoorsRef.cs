using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class RoomDoorsRef : MonoBehaviour
{
    
    public List<GameObject> entryGOs;
    public float roomSize;
    
    
    public void OpenCloseEntry(Entries entry, DoorState doorState)
    {
        if (doorState == DoorState.Open)
        {
            entryGOs[(int)entry].SetActive(false);
            return;
        }
        entryGOs[(int)entry].SetActive(true);
    }
    
    
}


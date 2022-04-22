using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Blueprint
{
    public string BuildingID;
    public bool unlocked;

    public Blueprint(string ID, bool _unlocked){
        BuildingID = ID;
        unlocked = _unlocked;
    }
}
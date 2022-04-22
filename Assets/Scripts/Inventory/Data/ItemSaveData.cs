using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSaveData 
{
    public string itemName;
   public List<StackInfo> stacks = new List<StackInfo>();
}
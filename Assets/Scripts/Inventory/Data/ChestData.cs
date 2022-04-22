using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestData
{
    public List<ItemSaveData> inventory;

   public ChestData(List<ItemSaveData> _inventory){
       inventory = new List<ItemSaveData>(_inventory);
   }
}
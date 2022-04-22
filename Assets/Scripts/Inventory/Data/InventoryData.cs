using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public List<ItemSaveData> inventory{get;private set;}
    public List<ChestData>[] chestsCreated = new List<ChestData>[3];

    public void GetData(){
       inventory = InventoryManager.Instance.getInventoryToSave();

       for(int x = 0; x <= GameData.current.inventoryData.chestsCreated.Length - 1; x++){
            chestsCreated[x] = GameData.current.inventoryData.chestsCreated[x];
        } 

        chestsCreated[SaveLoad.GetCurrentScene() - 1] = InventoryManager.Instance.SaveChests();
    }
}
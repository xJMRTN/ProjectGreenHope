using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public static GameData current;
    public string GameName;
    public string GameDate;
    public int GameID;
    public PlayerData playerData;
    public LightingData lightingData;
    public BuildingData buildingData;
    public InventoryData inventoryData;

    public GameData(){
        if(current == null) {
            playerData = new PlayerData();
            lightingData = new LightingData();
            buildingData = new BuildingData();
            inventoryData = new InventoryData();
        }
        else { 
            playerData = current.playerData;   
            lightingData = current.lightingData;   
            buildingData = current.buildingData;   
            inventoryData = current.inventoryData;   
        }
    }

    public void GetData(){
       playerData.GetData();
       lightingData.GetData();
       buildingData.GetData();
       inventoryData.GetData();
       //GameName = PlayerPrefs.GetString("WorldName");
       GameName = "TempName";
    }

    public void SaveGame(){
        current = this;
        GetData();
        SaveLoad.Save();     
    }

    public void LoadGame(){
        SaveLoad.Load();
    }
}
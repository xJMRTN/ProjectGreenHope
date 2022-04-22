using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public List<Building>[] buildingsCreated = new List<Building>[3];
    public List<Blueprint> blueprints = new List<Blueprint>();

    public void GetData(){

        for(int x = 0; x <= GameData.current.buildingData.buildingsCreated.Length - 1; x++){
            buildingsCreated[x] = GameData.current.buildingData.buildingsCreated[x];
        } 

        buildingsCreated[SaveLoad.GetCurrentScene() - 1] = BuildManager.Instance.buildingsCreated;      
        blueprints = BuildManager.Instance.SaveBlueprints();
    }
}
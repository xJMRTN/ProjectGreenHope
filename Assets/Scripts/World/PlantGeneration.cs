using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGeneration : MonoBehaviour
{
    [SerializeField] float SpawnRadius;
    [SerializeField] GameObject[] treePrefabs;
    [SerializeField] int treeAmount;
    [SerializeField] Transform spawnPoint;

    public void Start(){
        SpawnItems();
    }

    void SpawnItems(){
        for(int x = 0; x <= treeAmount; x++){
            Vector3 pos = FindPos();
            if(pos != Vector3.zero) Spawn(pos);
        }
    }

    Vector3 FindPos(){
        Vector3 ItemPosition = new Vector3();
        bool ready = false;
        int x = 0;
        while(!ready){
            Vector3 topPosition = (Vector3)Random.insideUnitCircle * SpawnRadius;
             topPosition = new Vector3 (topPosition.x + spawnPoint.position.x, spawnPoint.position.y, topPosition.y+ spawnPoint.position.z);  
            Vector3 placePosition = UtilityManager.Instance.ShootRayCastDown(topPosition);
            if(placePosition != Vector3.zero) {
                ready = true;
                ItemPosition = placePosition;
            }
            x++;
            if( x > 50) return Vector3.zero;
        }
        return ItemPosition;
    }

    void Spawn(Vector3 pos){
        GameObject go = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
    }
}
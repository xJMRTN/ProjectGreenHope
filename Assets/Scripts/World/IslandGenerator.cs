using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class IslandGenerator : MonoBehaviour
{
   [SerializeField] GameObject[] IslandPrefrabs;
   [SerializeField] Transform SpawnPosition;
   [SerializeField] Transform Player;
   [SerializeField] float numberOfIslands;
   [SerializeField] float minDistanceBetweenIslands;
   [SerializeField] float minDistanceToPlayer;
   [SerializeField] float minY;
   [SerializeField] float maxY;
   [SerializeField] float SpawnRadius;
   [SerializeField] float DespawnRange;

   List<GameObject> CurrentIslands = new List<GameObject>();

    private static IslandGenerator instance = new IslandGenerator();

    public void Awake(){
        if(instance == null) {
            instance = this;        
        }
    }

    static IslandGenerator(){
    }

    private IslandGenerator(){

    }

    public static IslandGenerator Instance{
        get{return instance;}   
    }

   public void Start(){
       SpawnIslands();
   }

   void Update(){
       CheckIslandPositions();
   }

   void CheckIslandPositions(){
       GameObject currentIsland = new GameObject();
       bool despawned = false;
       foreach(GameObject go in CurrentIslands){
           currentIsland = go;
           float distance = Vector3.Distance(go.transform.position, Player.position);
           if(distance > DespawnRange) {
               SpawnNewIsland();
               despawned = true;
               break;
           }
        }
        if(despawned){
            CurrentIslands.Remove(currentIsland);
            Destroy(currentIsland);
        }
   }

   void SpawnNewIsland(){
       CalculateNewOrigin();
       SpawnIsland();
   }

//this doesnt work fix it
   void CalculateNewOrigin(){
       Vector3 newPos = new Vector3(
           Player.position.x + (Player.position.x - SpawnPosition.position.x),
           SpawnPosition.position.y,
           Player.position.z + (Player.position.z - SpawnPosition.position.z)
        );

        SpawnPosition.position = newPos;
   }

   void SpawnIsland(){
        Vector3 pos = FindPos();
        if(pos != Vector3.zero) Spawn(pos);
   }

   void SpawnIslands(){
       for(int x = 0; x <= numberOfIslands; x++){
           SpawnIsland();
        }
   }

    void Spawn(Vector3 pos){
        GameObject go = Instantiate(IslandPrefrabs[Random.Range(0, IslandPrefrabs.Length)], pos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
        CurrentIslands.Add(go);
    }

    Vector3 FindPos(){
        Vector3 ItemPosition = new Vector3();
        bool ready = false;
        int x = 0;
        while(!ready){
            Vector3 topPosition = (Vector3)Random.insideUnitCircle * SpawnRadius;
             topPosition = new Vector3 (topPosition.x + SpawnPosition.position.x, Random.Range(minY, maxY), topPosition.y+ SpawnPosition.position.z);  
             ItemPosition = topPosition;
            if(!CheckIfCloseToOtherIsland(topPosition) && !CheckIfCloseToPlayer(topPosition)) ready = true;
            else{
                x++;
                if( x > 50) return Vector3.zero;
            }        
        }

        return ItemPosition;
    }

    bool CheckIfCloseToOtherIsland(Vector3 pos){
        foreach(GameObject go in CurrentIslands){
            float distance = Vector3.Distance(go.transform.position, pos);
            if(distance < minDistanceBetweenIslands) return true;
        }
        return false;
    }

    bool CheckIfCloseToPlayer(Vector3 pos){
        return (Vector3.Distance(pos, Player.position) < minDistanceToPlayer);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class ItemManager : MonoBehaviour
{
    public GameObject Player;
    GameObject currentItem;
    public Item[] items;
    public Dictionary<string, Item> itemList = new Dictionary<string, Item>();
    
    public List<Building> worldCreated = new List<Building>();
    public List<GameObject> worldCreatedObj = new List<GameObject>();


    public List<GameObject> lumberables = new List<GameObject>();
    public List<GameObject> minables = new List<GameObject>();
    public List<GameObject> foragable = new List<GameObject>();

    private static ItemManager instance = new ItemManager();

    void Awake(){
        if(instance == null) {
            instance = this;              
        }
        instance.SetupDictionaries();
    }

    static ItemManager(){
    }

    private ItemManager(){

    }

    public static ItemManager Instance{
        get{return instance;}   
    }

    public void SetupDictionaries(){      
        foreach(Item item in items){
            string key = item.name;
            if(!itemList.ContainsKey(key)) {
                itemList.Add(key, item);
            }
        }
    }
    
    ///<summary>This method spawns one or more objects at a location.</summary>
    public void Spawn(Transform location, float range, int amount, string _key, bool isItem){
        bool ready = true;
        if(currentItem)currentItem = null;
        if(isItem){
            if(!itemList.ContainsKey(_key)){
                ready = false;
            }else currentItem = itemList[_key].ItemObject;
        }
        if(ready){           
            for(int x = 0; x < amount; x++){
                Vector3 position = new Vector3(location.position.x + Random.Range(-range, range),location.position.y,location.position.z + Random.Range(-range, range));
                GameObject itemCreated = Instantiate(currentItem, position, Quaternion.identity, location);
                if(isItem) {
                    AddToList(position, _key);
                    worldCreatedObj.Add(itemCreated);
                    AddToCatList(itemCreated);
                }
            }
        }       
    }


    public void AddToCatList(GameObject item){
        switch(item.GetComponent<Item>().skills){
            case Item.SkillType.Mining:
                minables.Add(item);
                break;
            case Item.SkillType.Lumbering:
                lumberables.Add(item);
                break;
            case Item.SkillType.Foraging:
                foragable.Add(item);
                break;
        }
    }

    public void Spawn(Vector3 location, string _key, bool pool, float respawnTime){
        GameObject itemCreated = Instantiate(currentItem, location, Quaternion.identity);
        worldCreatedObj.Add(itemCreated);
        AddToCatList(itemCreated);
    }

    private void AddToList(Vector3 pos, string id){
        float[] position = new float[3];
        position[0] =  pos.x;
        position[1] =  pos.y;
        position[2] =  pos.z;
        Building newBuilding = new Building(id, position);
        worldCreated.Add(newBuilding);
    }

    public List<Building> SaveWorld(){
        int x = 0;
        foreach(Building b in worldCreated){
            Item thisItem = worldCreatedObj[x].GetComponent<Item>();                  
            x++;
        }

        return worldCreated;
    }

    public void LoadSavedWorld(List<Building> _create){
        foreach(Building b in _create){

            Vector3 buildingPosition = new Vector3(b.position[0],b.position[1],b.position[2]);
            currentItem = itemList[b.id].ItemObject;

            Spawn(buildingPosition, b.id, b.isPooled, b.respawnTime);
            worldCreated.Add(b);            
        }
    }
}
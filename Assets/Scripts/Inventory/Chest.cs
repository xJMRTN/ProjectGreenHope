using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour
{
    public List<ItemData> inventory;
    private Dictionary<InventoryItem, ItemData> itemDictionary;
    public Texture2D cursor;
    public GameObject player;
   

    void Awake(){
        inventory = new List<ItemData>();
        itemDictionary = new Dictionary<InventoryItem, ItemData>();
    }

    void Start(){
        player = GameObject.Find("Player");
    }

    public void SetInventoryAsActive(){
        InventoryManager.Instance.SetChestInventory(this);
    }

    public void LoadChestData(List<ItemSaveData> listOfItems){
        foreach(ItemSaveData data in listOfItems){
            InventoryItem _data = InventoryManager.Instance.SearchDictionary(data.itemName);
            foreach(StackInfo info in data.stacks){
                Add(_data, info.stackSize, info.InventorySlot);
            }           
        }
    }

    public void Add(InventoryItem _data, int amount, int slotNumber){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slotNumber);
            if(_info == null){
                //new Inventory slot, old item
                AddNewChestSlot(value, slotNumber, amount);
            }else{
                //old inventory slot, old item
                _info.AddToStack(amount);
            }       
        }else{
            ItemData newItem = new ItemData(_data, amount);
            inventory.Add(newItem);
            itemDictionary.Add(_data, newItem);
            AddNewChestSlot(newItem, slotNumber);
        }       
    }

    public void AddNewChestSlot(ItemData value, int slotNumber){
        value.stackInfos[0].InventorySlot = slotNumber;
    }

    public void AddNewChestSlot(ItemData value, int slotNumber, int amount){  
        StackInfo newStack = new StackInfo(amount, slotNumber);   
        value.stackInfos.Add(newStack); 
        newStack.InventorySlot = slotNumber;
    }

    public void RemoveFromChestSlot(InventoryItem _data, int slot){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slot);
            if(value.RemoveFromList(_info)){
                inventory.Remove(value);
                itemDictionary.Remove(_data);
            }
        }
    }

    public void Remove(InventoryItem _data, int amount){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            int amountLeft = amount;
            List<StackInfo> ItemsToRemove = new List<StackInfo>();

            foreach(StackInfo _info in value.stackInfos){
                if(amountLeft > 0){                
                    if(_info.stackSize - amountLeft >= 0){
                        //everything can come out of this stack
                        _info.stackSize -= amountLeft;
                        amountLeft = 0;
                        if(_info.stackSize == 0){
                            ItemsToRemove.Add(_info);
                        }   
                    }else{
                        amountLeft -= _info.stackSize;
                        _info.stackSize = 0;
                
                        ItemsToRemove.Add(_info);
                    }
                }else break;
            }

            //if the inventory for this item is all gone, clear from dictionary.
            if(ItemsToRemove.Count > 0){

            
                if(value.RemoveFromList(ItemsToRemove)){
                    inventory.Remove(value);
                    itemDictionary.Remove(_data);
                }
            }
        } 
    }
}
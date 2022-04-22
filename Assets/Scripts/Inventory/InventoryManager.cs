using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public sealed class InventoryManager : MonoBehaviour
{
    private Dictionary<InventoryItem, ItemData> itemDictionary;
    [SerializeField]
    public List<ItemData> inventory {get;private set;}
    private static InventoryManager instance = new InventoryManager();
    public GameObject offLayout;
    public GameObject onLayout;
    bool inUI = false;
    public GameObject inventoryUI;
    public Canvas canvas;
    
    public List<ItemData> chestInventory {get;private set;}
    private Dictionary<InventoryItem, ItemData> chestItemDictionary;
    public ItemSlot activeItemSlot;
    public ItemSlot[] slots;
    public ItemSlot[] chestSlots;
    public GameObject chestUI;
    bool inChest = false;
    public Text itemName;
    public Text itemDesc;
    public Text itemYield;
    public GameObject popup;
    public ItemSlot ItemInHand;
    ItemSlot ItemHovering;
    public Transform holdingItemLayout;

    Chest currentChest;

    public bool ItemPickedUp = false;

    public GameObject player;
    int currentScenePos;

    public int currentSlotSelected;

    public void Awake(){
        if(instance == null) {
            instance = this;              
        }
        inventory = new List<ItemData>();
        chestInventory = new List<ItemData>();
        itemDictionary = new Dictionary<InventoryItem, ItemData>();
        chestItemDictionary = new Dictionary<InventoryItem, ItemData>();
        currentSlotSelected = 1;
    }

    void AssignSlotIDS(){
        int i = 0;
        foreach(ItemSlot slot in slots){
            slot.slotID = i;
            i++;
        }   
    }

    void AssignChestSlotIDS(){
        int i = 0;
        foreach(ItemSlot slot in chestSlots){
            slot.slotID = i;
            i++;
        }   
    }

    public ItemData Get(InventoryItem _data){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            return value;
        }
        return null;
    }

    static InventoryManager(){
        
    }

    private InventoryManager(){
        
    }

    public static InventoryManager Instance{
        get{return instance;}
    }

    void Start(){
        currentScenePos = SaveLoad.GetCurrentScene() - 1;
        if(GameData.current != null) {
            ConvertInventory(GameData.current.inventoryData.inventory);
            }
        AssignSlotIDS();
    }

    public void Update(){
        if(Input.GetKeyDown(KeyCode.I)){
            ToggleUI();
        } 

        if(ItemPickedUp) MoveItem();
    }

    public void ToggleUI(){
        inUI = !inUI;
        if(inUI){
            inventoryUI.transform.parent = onLayout.transform;         
        }
        else {
            if(ItemInHand) 
            ItemInHand.DropItem();
            inventoryUI.transform.parent = offLayout.transform;    
        }
    }

    public void ChangeCurrentItemSlot(int value){
        slots[currentSlotSelected].isActiveSlot = false;
        currentSlotSelected = value;
        activeItemSlot.SetUpActiveSlot(slots[value]);
    }

    public void Add(InventoryItem _data, int amount){
        if(_data == null) return;
        if(itemDictionary.TryGetValue(_data, out ItemData value)){   
            //Item is already in inventory - put items in first stack in inventory
            value.stackInfos[0].AddToStack(amount);
            UpdateInventory(value.stackInfos[0].InventorySlot);
        }else{
            //Item is not in this inventory, add a new slot
            ItemData newItem = new ItemData(_data, amount);
            inventory.Add(newItem);
            itemDictionary.Add(_data, newItem);
            AddNewSlot(newItem);
        }       
    }

    public ItemSlot Add(InventoryItem _data, int amount, int slotNumber){
        if(_data == null) return null;
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slotNumber);
            if(_info == null){
                //new Inventory slot, old item
                return AddNewSlot(value, slotNumber, amount);
            }else{
                //old inventory slot, old item
                _info.AddToStack(amount);
                UpdateInventory(_info.InventorySlot);
            }
        }else{
            //Item is not in inventory
            ItemData newItem = new ItemData(_data, amount);
            inventory.Add(newItem);
            itemDictionary.Add(_data, newItem);
            AddNewSlot(newItem, slotNumber);
        }    
        return null;   
    }

    public bool HasEnoughResources(InventoryItem _data, int amount){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            int _amountInInventory = 0;
            //loop through all stacks of the item and add them together
            foreach(StackInfo _info in value.stackInfos){
                _amountInInventory += _info.stackSize;
            }
            if(_amountInInventory >=  amount) return true;
            else {
                return false;
            }
        }else return false;
    }

    public bool IsItemInInventory(InventoryItem _data){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            return true;
        }
        return false;
    }

    public ItemData GetItemdata(InventoryItem _data){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            return value;
        }
        return null;
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
                            DeleteSlot(_info.InventorySlot);
                            ItemsToRemove.Add(_info);
                        }else UpdateInventory(_info.InventorySlot);     
                    }else{
      
                        amountLeft -= _info.stackSize;
                        _info.stackSize = 0;
                        
                        DeleteSlot(_info.InventorySlot);
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

    public void RemoveFromSlot(InventoryItem _data, int slot){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slot);
            if(value.RemoveFromList(_info)){
                inventory.Remove(value);
                itemDictionary.Remove(_data);
            }
             DeleteSlot(slot);
        }
    }
    
    public void RemoveFromSlot(InventoryItem _data, int slot, int amount){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slot);
             _info.stackSize -= amount;
             UpdateInventory(_info.InventorySlot);
        }
    }

    public int FindFirstOpenSlot(){
        int i = 0;
        foreach(ItemSlot slot in slots){
            if(slot.currentItem == null) {               
                return i;
            }
            i++;
        }   
        return -1; 
    }

    public int FindFirstOpenChestSlot(){
        int i = 0;
        foreach(ItemSlot slot in chestSlots){
            if(slot.currentItem == null) {               
                return i;
            }
            i++;
        }   
        return -1; 
    }

    public int HowManySlotsHaveItems(){
        int i = 0;
        foreach(ItemSlot slot in slots){
            if(slot.currentItem != null) {               
                i++;
            }
        }   
        return i; 
    }

    public void AddNewSlot(ItemData value){
        int i = 0;
        foreach(ItemSlot slot in slots){
            if(slot.currentItem == null) {
                slot.Set(value, value.stackInfos[0].stackSize, i, null);
                return;
            }
            i++;
        }    
        //No room for the item
        inventory.Remove(value);
        itemDictionary.Remove(value.data);
    }

    public bool RoomInInventory(InventoryItem _data){
        if(itemDictionary.TryGetValue(_data, out ItemData value)){
            return true;
        }

        int i = 0;
        foreach(ItemSlot slot in slots){
            if(slot.currentItem == null) {
                return true;
            }
            i++;
        }   

        return false;
    }

    public void AddNewSlot(ItemData value, int slotNumber){
        //Add Item when you know inventory slot
        slots[slotNumber].Set(value, value.stackInfos[0].stackSize, slotNumber, null);
        
    }

    public ItemSlot AddNewSlot(ItemData value, int slotNumber, int amount){  
        StackInfo newStack = new StackInfo(amount, slotNumber);   
        value.stackInfos.Add(newStack); 

        slots[slotNumber].Set(value, newStack.stackSize, slotNumber, newStack);
        return slots[slotNumber];
    }

    public void UpdateInventory(int slot){
        slots[slot].UpdateSlot();
        activeItemSlot.UpdateActiveSlot();
    }

    public void DeleteSlot(int slot){
        slots[slot].DeleteSlot();
    }

    public void DeleteChestSlot(int slot){
        chestSlots[slot].DeleteSlot();
    }

    public void SetName(ItemData value, Transform pos){     
        if(value != null){
            popup.transform.position = pos.position;
            itemName.text = value.data.displayName;
            itemDesc.text = value.data.description;
            if(value.data.seed.growTime != 0 )itemYield.text = value.data.seed.growTime + " seconds = " + value.data.seed.ProduceAmount + " " + value.data.seed.Produce.displayName + "s";
            else itemYield.text = "";
            popup.SetActive(true);
        }     else{
            popup.SetActive(false);
        }
    }

    public List<ItemSaveData> getInventoryToSave(){
        List<ItemSaveData> listOfItems = new List<ItemSaveData>();

        foreach(ItemData data in inventory){
            
            ItemSaveData newData = new ItemSaveData();
            string tempName = "";
            
            foreach(KeyValuePair<string, Item> _item in ItemManager.Instance.itemList){
                    if(_item.Value.item.displayName == data.data.displayName)  tempName = _item.Value.name;
                }

            newData.itemName = tempName; //we need item.name
            Debug.Log("Saving " + newData.itemName);
            newData.stacks = data.stackInfos;
            listOfItems.Add(newData);       
        }
        return listOfItems;
    }

    public void ConvertInventory(List<ItemSaveData> listOfItems){
        foreach(ItemSaveData data in listOfItems){
            InventoryItem _data = SearchDictionary(data.itemName);
            foreach(StackInfo info in data.stacks){
                Add(_data, info.stackSize, info.InventorySlot);
            }          
        }
    }

    public InventoryItem SearchDictionary(string _value){
        if(_value == "") return null;
        if(ItemManager.Instance.itemList.TryGetValue(_value, out Item value)){
            return value.item;
        }

        return null;
    }

    public void SetChestInventory(Chest _chest){
        currentChest = _chest;
        ClearChest();
        ToggleChestUI();
        foreach(ItemData data in _chest.inventory){   
            foreach(StackInfo info in data.stackInfos){
                AddToChest(data.data, info.stackSize, info.InventorySlot, true);
            }          
        }
        AssignChestSlotIDS();
    }

    public void ClearChest(){
        foreach(ItemSlot _slot in chestSlots){
            _slot.DeleteSlot();
        }
        chestInventory.Clear();
        chestItemDictionary.Clear();
    }

    public void ToggleChestUI(){
        inChest = !inChest;
        if(inChest){
            chestUI.transform.parent = onLayout.transform;
        }
        else {
            chestUI.transform.parent = offLayout.transform;
        }
    }

    public ItemSlot AddToChest(InventoryItem _data, int amount, int slotNumber, bool loading){
        if(chestItemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slotNumber);
            if(_info == null){
                //new Inventory slot, old item
                return AddNewChestSlot(value, slotNumber, amount);
            }else{
                //old inventory slot, old item
                _info.AddToStack(amount);
                UpdateChestInventory(_info.InventorySlot);
            }
        }else{
            ItemData newItem = new ItemData(_data, amount);
            chestInventory.Add(newItem);
            chestItemDictionary.Add(_data, newItem);
            AddNewChestSlot(newItem, slotNumber);
        }       

        if(!loading)currentChest.Add(_data, amount, slotNumber);
        return null;
    }

    public void RemoveFromChestSlot(InventoryItem _data, int slot){
        if(chestItemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slot);
            if(value.RemoveFromList(_info)){
                chestInventory.Remove(value);
                chestItemDictionary.Remove(_data);
            }
             DeleteChestSlot(slot);
             currentChest.RemoveFromChestSlot(_data, slot);    
        }
    }

    public void RemoveFromChestSlot(InventoryItem _data, int slot, int amount){
        if(chestItemDictionary.TryGetValue(_data, out ItemData value)){
            StackInfo _info = value.SearchForStack(slot);
             _info.stackSize -= amount;
             UpdateChestInventory(_info.InventorySlot);
        }
    }

    public void RemoveFromChest(InventoryItem _data, int amount){
        if(chestItemDictionary.TryGetValue(_data, out ItemData value)){
            int amountLeft = amount;
             List<StackInfo> ItemsToRemove = new List<StackInfo>();

            foreach(StackInfo _info in value.stackInfos){
                if(amountLeft > 0){       
                    if(_info.stackSize - amountLeft >= 0){
                        //everything can come out of this stack
                        _info.stackSize -= amountLeft;
                        amountLeft = 0;
                        if(_info.stackSize == 0){
                            DeleteChestSlot(_info.InventorySlot);
                            ItemsToRemove.Add(_info);
                        }else UpdateChestInventory(_info.InventorySlot);     
                               
                    }else{
                        amountLeft -= _info.stackSize;
                        _info.stackSize = 0;
                        DeleteChestSlot(_info.InventorySlot);
                        ItemsToRemove.Add(_info);
                    }
                }else break;
            }

            //if the inventory for this item is all gone, clear from dictionary.
            if(ItemsToRemove.Count > 0){          
                if(value.RemoveFromList(ItemsToRemove)){
                    chestInventory.Remove(value);
                chestItemDictionary.Remove(_data);
                }
            }

        currentChest.Remove(_data, amount);     
        }
    }
    

    public void AddNewChestSlot(ItemData value, int slotNumber){  
        chestSlots[slotNumber].Set(value, value.stackInfos[0].stackSize, slotNumber, null);
    }

    public ItemSlot AddNewChestSlot(ItemData value, int slotNumber, int amount){  
 
        StackInfo newStack = new StackInfo(amount, slotNumber);   
        value.stackInfos.Add(newStack); 

        chestSlots[slotNumber].Set(value, newStack.stackSize,slotNumber, newStack);
        return chestSlots[slotNumber];
    }

    public void UpdateChestInventory(int slot){
        chestSlots[slot].UpdateSlot();
        activeItemSlot.UpdateActiveSlot();
    }

    public List<ChestData> SaveChests(){

        List<ChestData> newChestList = new List<ChestData>();


        //Each chest has a list of items (inventory)
        foreach(GameObject chestObject in BuildManager.Instance.chests){
            List<ItemSaveData> listOfItems = new List<ItemSaveData>();

            foreach(ItemData data in chestObject.GetComponent<Chest>().inventory){
               
            ItemSaveData newData = new ItemSaveData();
            string tempName = "";
    
            foreach(KeyValuePair<string, Item> _item in ItemManager.Instance.itemList){
                    if(_item.Value.item.displayName == data.data.displayName)  tempName = _item.Value.name;
                }

            newData.itemName = tempName;
            newData.stacks = data.stackInfos;
            listOfItems.Add(newData);   
                       
            }
            ChestData newChestData = new ChestData(listOfItems);
            newChestList.Add(newChestData);    
            listOfItems.Clear();
        }

        return newChestList;
    }

    public void SetPickedUpItem(ItemSlot _ItemInHand){
        ItemPickedUp = true;      
        ItemInHand = _ItemInHand;
         Debug.Log(ItemInHand.currentItem.stackInfos.Count);
    }

    public void DropItem(){
        ItemPickedUp = false;
        SwapItems();
    }

    public void MoveItem(){
        Vector2 pos;
         RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
         ItemInHand.MoveItem(canvas.transform.TransformPoint(pos));
    }

    public void HoveringOverItem(ItemSlot _ItemHovering){
        if(ItemPickedUp){
            if(_ItemHovering == null){
                ItemHovering = null;
                return;
            }

            if(_ItemHovering.currentItem == null){
                ItemHovering = _ItemHovering;
                return;
            }

            if(_ItemHovering.slotID != ItemInHand.slotID){
               
                ItemHovering = _ItemHovering;
                return;
            }
        }        
    }

    public void SwapItems(){
        if(ItemHovering == null){       
            ItemInHand = null;
            return;
        }


        ItemSlot tempHoldingSlot = new ItemSlot(ItemInHand);
        ItemSlot tempHoveringSlot = new ItemSlot(ItemHovering);
        
        int movingTo = new int();
        int movingTo2 = new int();
        movingTo = tempHoveringSlot.slotID;
        movingTo2= tempHoldingSlot.slotID;
        StackInfo fromInfo = new StackInfo(ItemInHand.AmountInSlot);
        StackInfo toInfo = null;
        if(ItemHovering.currentItem != null) toInfo =  new StackInfo(ItemHovering.AmountInSlot);

        if(ItemHovering.isChestSlot == ItemInHand.isChestSlot){
            //Both items slots are in the same inventory.
            if(!ItemInHand.isChestSlot){  
                RemoveFromSlot(ItemInHand.currentItem.data,movingTo2);             
                if(ItemHovering.currentItem != null){ 
                    if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName)RemoveFromSlot(ItemHovering.currentItem.data,movingTo); 
                }
                Add(tempHoldingSlot.currentItem.data, fromInfo.stackSize, movingTo);                
                if(tempHoveringSlot.currentItem != null){ 
                    if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName)Add(tempHoveringSlot.currentItem.data, toInfo.stackSize,  movingTo2);
                }
            }else{
               RemoveFromChest(ItemInHand.currentItem.data,fromInfo.stackSize);
                if(ItemHovering.currentItem != null) {
                    if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName)RemoveFromChest(ItemHovering.currentItem.data,toInfo.stackSize);
                }
                AddToChest(tempHoldingSlot.currentItem.data, fromInfo.stackSize, movingTo, false);
                if(tempHoveringSlot.currentItem != null){ 
                    if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName)AddToChest(tempHoveringSlot.currentItem.data, toInfo.stackSize,  movingTo2, false);
                }
            }
        }else{
            
            //Item slots are in different inventories.
            if(!ItemInHand.isChestSlot){
                RemoveFromSlot(ItemInHand.currentItem.data,movingTo2);
                if(ItemHovering.currentItem != null){
                    if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName) RemoveFromChestSlot(ItemHovering.currentItem.data,movingTo);
                }
                AddToChest(tempHoldingSlot.currentItem.data, fromInfo.stackSize, movingTo, false);
                if(tempHoveringSlot.currentItem != null){
                    if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName) Add(tempHoveringSlot.currentItem.data, toInfo.stackSize, movingTo2);
                }
            }else{           
                RemoveFromChestSlot(ItemInHand.currentItem.data,movingTo2);
                if(ItemHovering.currentItem != null){
                if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName) RemoveFromSlot(ItemHovering.currentItem.data,movingTo);
                }
                Add(tempHoldingSlot.currentItem.data, fromInfo.stackSize, movingTo);
                if(tempHoveringSlot.currentItem != null){
                    if(tempHoldingSlot.currentItem.data.displayName != ItemHovering.currentItem.data.displayName)AddToChest(tempHoveringSlot.currentItem.data, toInfo.stackSize,  movingTo2, false);
                }
            }
        }

        ItemHovering.UpdateSlot();
        ItemInHand.UpdateSlot();

        ItemHovering = null;
        ItemInHand = null;
        activeItemSlot.UpdateActiveSlot();
    }

    public void EatFood(ItemSlot _slot){
        player.GetComponent<PlayerController>().UpdatePlayerHunger(_slot.currentItem.data.foodValue);
        Remove(_slot.currentItem.data, 1);      
        activeItemSlot.UpdateActiveSlot();
    }
}

[SerializeField]
public class ItemData{
    [SerializeField]
    public InventoryItem data {get;private set;}
   public List<StackInfo> stackInfos = new List<StackInfo>();

    public ItemData(ItemData _item){
        data = _item.data;  
    }

    public ItemData(InventoryItem source, int amount){
        data = source;
        StackInfo _tempInfo = new StackInfo(amount);
        stackInfos.Add(_tempInfo);
    }


    public ItemData(InventoryItem source, int amount, int slot){
        data = source;
        StackInfo _tempInfo = SearchForStack(slot);
        if(SearchForStack(slot) != null) {
            _tempInfo.AddToStack(amount);
            _tempInfo.InventorySlot = slot;
        }else{
            _tempInfo = new StackInfo(amount, slot);
        }

        stackInfos.Add(_tempInfo);
    }

    public StackInfo SearchForStack(int _slotNumber){
        foreach(StackInfo info in stackInfos){
            if(info.InventorySlot == _slotNumber){
                return info;
            }
        }
        return null;
    }

//returns true if the item is no longer in inventory
    public bool RemoveFromList(List<StackInfo> _remove){
        List<StackInfo> stacksToKeep = new List<StackInfo>();
        foreach(StackInfo _info in _remove){
            foreach(StackInfo _stackInList in stackInfos){
                if(_info != _stackInList){
                    stacksToKeep.Add(_stackInList);
                }
            }
        }
        stackInfos.Clear();
        stackInfos = stacksToKeep;
        if(stackInfos.Count == 0){
            return true;
        }       
        return false;
    }

    public bool RemoveFromList(StackInfo _remove){
        List<StackInfo> stacksToKeep = new List<StackInfo>();
        foreach(StackInfo _stackInList in stackInfos){
            if(_remove != _stackInList){
                stacksToKeep.Add(_stackInList);
            }
        }
        stackInfos.Clear();
        stackInfos = stacksToKeep;

        if(stackInfos.Count == 0){
            return true;
        }       

        return false;
    }

    public int TotalAmount(){
        int amount = 0;
        foreach(StackInfo _stackInList in stackInfos){
            amount += _stackInList.stackSize;
        }
        return amount;
    }
}

[System.Serializable]
public class StackInfo{
    public int stackSize;
    public int InventorySlot;

    public StackInfo(int _size, int _slot){
        stackSize = _size;
        InventorySlot = _slot;
    }

    public StackInfo(int _size){
        stackSize = _size;
    }

    public void AddToStack(int amount){
        stackSize += amount;
    }

    public void RemoveFromStack(int amount){
        stackSize -= amount;
    }

}
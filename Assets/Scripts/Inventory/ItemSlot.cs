using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Sprite defaultIcon;
    public GameObject stack;
    public Text stackUI;
    public ItemData currentItem;
    public RectTransform rt;
    public GameObject Item;
    bool pickedUp = false;
    Vector3 originalPosition = new Vector3();
    public bool isChestSlot;
    public int slotID;
    public int AmountInSlot;
    public UnityEvent rightClick;
    private Transform originalLayout;
    public bool isActiveSlot = false;

    public ItemSlot(ItemSlot _slot){
        icon = _slot.icon;
        defaultIcon = _slot.defaultIcon;
        stack = _slot.stack;
        stackUI = _slot.stackUI;
        if(_slot.currentItem != null) Debug.Log(_slot.currentItem.stackInfos.Count);
        if(_slot.currentItem != null) currentItem = new ItemData(_slot.currentItem);
        rt = _slot.rt;
        Item = _slot.Item;
        originalLayout = Item.transform.parent;
        isChestSlot = _slot.isChestSlot;
        slotID = _slot.slotID;
    }


    public void Set(ItemData item, int amount, int slot, StackInfo _newStack){
        stack.SetActive(true);
        rt.localScale = new Vector3(4,4,1);
        currentItem = item;
        icon.sprite = currentItem.data.icon;
        stackUI.text = amount.ToString();
        AmountInSlot = amount;
        if(_newStack != null) _newStack.InventorySlot = slot;
        else currentItem.stackInfos[0].InventorySlot = slot;
        originalLayout = Item.transform.parent;
    }

    public void UpdateSlot(){
        if(currentItem != null){
            Debug.Log("Slot ID:" + slotID + " is being updated");
            StackInfo info = currentItem.SearchForStack(slotID);
            stackUI.text = info.stackSize.ToString();
            AmountInSlot = info.stackSize;
        }
        else stackUI.text = "";
    }

    public void DeleteSlot(){
        AmountInSlot = 0;
        rt.localScale = new Vector3(1,1,1);
        currentItem = null;
        stackUI.text = "";
        icon.sprite = defaultIcon;
    }

    public void OnPointerEnter()
    {
        if(currentItem != null && !pickedUp){ 
            InventoryManager.Instance.SetName(currentItem, this.gameObject.transform);
            if(InventoryManager.Instance.ItemPickedUp) InventoryManager.Instance.HoveringOverItem(this);
        }else{
            if(currentItem == null){
                if(InventoryManager.Instance.ItemPickedUp) InventoryManager.Instance.HoveringOverItem(this);
            }
        }
    }
    public void OnPointerExit()
    {
        InventoryManager.Instance.SetName(null, null);
        if(InventoryManager.Instance.ItemPickedUp) InventoryManager.Instance.HoveringOverItem(null);
    }

    public void SetItemSlot(){
        if( InventoryManager.Instance.ItemPickedUp){
            InventoryManager.Instance.ItemInHand.DropItem();
            return;
        }

        if(currentItem != null && !isChestSlot) {
            InventoryManager.Instance.ChangeCurrentItemSlot(slotID);
            isActiveSlot = true;
        }
    }

    public void SetUpActiveSlot(ItemSlot _slotToCopy){
        stack.SetActive(true);
        currentItem = _slotToCopy.currentItem;
        icon.sprite = currentItem.data.icon;
        stackUI.text =_slotToCopy.stackUI.text;
        AmountInSlot = _slotToCopy.AmountInSlot;
    }

    public void UpdateActiveSlot(){
        if(currentItem == null) {
            DeleteSlot();
            return;
        }

        StackInfo info = currentItem.SearchForStack(slotID);
        if(info == null){
            DeleteSlot();
            return;
        }

        stackUI.text = info.stackSize.ToString();
        AmountInSlot = info.stackSize;
    }

    public void PickUpItem(){
        if(currentItem != null){           
            if(!InventoryManager.Instance.ItemPickedUp && currentItem != null){    
                Item.transform.parent = InventoryManager.Instance.holdingItemLayout;
                Item.layer = 2;
                originalPosition = Item.transform.position;
                InventoryManager.Instance.SetPickedUpItem(this);  
                pickedUp = true; 
            }      
        }    
    }

    public void DropItem(){
        InventoryManager.Instance.DropItem(); 
        pickedUp = false;  
        ReturnItem();   
    }

    public void MoveItem(Vector3 pos){
        Item.transform.position = pos;
    }

    public void ReturnItem(){
        Item.transform.position = originalPosition;   
        Item.transform.parent = originalLayout;
        Item.layer = 5;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(eventData.button == PointerEventData.InputButton.Right && currentItem != null){
            if(AmountInSlot > 1) SplitStack();
        }
    }

    void SplitStack(){
        int amountToTake = AmountInSlot/2;
        bool slotFree = false;
        if(isChestSlot){
            int _slot = InventoryManager.Instance.FindFirstOpenChestSlot();
            if(_slot != -1){
                slotFree = true;
                 ItemSlot newItem = InventoryManager.Instance.AddToChest(currentItem.data, amountToTake, _slot, false);
                  newItem.PickUpItem();
            }   
            if(slotFree) InventoryManager.Instance.RemoveFromChestSlot(currentItem.data, slotID, amountToTake);       
        }else{
            int _slot = InventoryManager.Instance.FindFirstOpenSlot();
             if(_slot != -1){
                 slotFree = true;
                 ItemSlot newItem = InventoryManager.Instance.Add(currentItem.data, amountToTake, _slot);
                newItem.PickUpItem();
             }      
             if(slotFree) InventoryManager.Instance.RemoveFromSlot(currentItem.data, slotID, amountToTake);    
        }      
    }
}
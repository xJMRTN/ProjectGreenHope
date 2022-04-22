using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour
{
    public Image icon;
    public Buildable currentItem;

    
    
    public void Set(Buildable item){
        currentItem = item;
        icon.sprite = currentItem.BuildItem.icon;
    }

    public void SetBuildingItem(){
        BuildManager.Instance.SetNewItem(currentItem.BuildItem.displayName);
    }

    public void OnPointerEnter()
    {
       // BuildManager.Instance.SetCostText(currentItem, this.gameObject.transform);
    }
    public void OnPointerExit()
    {
        //BuildManager.Instance.SetCostText(null, null);
    }
}
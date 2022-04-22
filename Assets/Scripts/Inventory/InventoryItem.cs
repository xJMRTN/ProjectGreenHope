using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = ("Inventory Item"))]
public class InventoryItem : ScriptableObject
{
    public string id;
    public string displayName;
    public string description;
    public int sellPrice;
    public int buyPrice;
    public Sprite icon;
    public GameObject prefab;
     public Material itemMaterial;
     public Material goodMaterial;
     public Material blockedMaterial;
    public Seed seed;
    public bool isFood = false;
    public float foodValue;

    public InventoryItem(){
        if(prefab != null)itemMaterial = prefab.GetComponent<MeshRenderer>().material;
    }
}
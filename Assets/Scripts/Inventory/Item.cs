using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour
{
    public string name;
    public Texture2D cursor; 
    public GameObject player;
    public GameObject ItemObject;
    public InventoryItem item;
    public int itemAmount;
    private MeshRenderer meshRenderer;
    private Vector3 startLocation;
    public int sceneID = 0;
    public int spawnID = 0;
    AsyncOperation loadingOperation;
    private Canvas canvas;

    public enum SkillType{
        Mining,
        Lumbering,
        Foraging
    }

    public SkillType skills;

    public bool isActive = true;

    void Awake(){     
        player = GameObject.Find("Player");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        startLocation = transform.position;  
    }  


    void AddToInventory(){
        InventoryManager.Instance.Add(item, itemAmount);
    }
}
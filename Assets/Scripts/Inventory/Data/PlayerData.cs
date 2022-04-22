using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float[] pos;
    public float currentHunger;
    public float currentHealth;
    
   public void GetData(){
        pos = new float[3];
        Transform player = GameObject.Find("Player").transform;
        pos[0] = player.position.x;
        pos[1] = player.position.y;
        pos[2] = player.position.z;
      
        currentHealth = player.GetComponent<PlayerController>().playerHealth;
        currentHunger = player.GetComponent<PlayerController>().playerHunger;
    }
}
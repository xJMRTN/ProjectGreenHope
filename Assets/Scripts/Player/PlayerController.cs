using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerHunger;
    public float playerHealth;
    bool eDown = false;

    public void UpdatePlayerHunger(float _value){
        playerHunger += _value;
    }

    public void Update(){
        HandleInput();
    }

    void HandleInput(){
        if(Input.GetKeyDown(KeyCode.E) && !eDown){
            eDown = true;
            UtilityManager.Instance.RaycastFromPlayer();
        }
        if(Input.GetKeyUp(KeyCode.E)){
            eDown = false;
        }
    }
}
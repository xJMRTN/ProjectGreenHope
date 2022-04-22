using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SailControls : MonoBehaviour
{
    bool isTurning = false;
    public PlayerMovement playerMovement;
    [SerializeField] GameObject floatingText;
    [SerializeField] Transform leftControlText;
    [SerializeField] Transform rightControlText;
    [SerializeField] Transform canvas;
    [SerializeField] Transform bottomBar;
    [SerializeField] Transform Sail;
    [SerializeField] float steeringSpeed;
    [SerializeField] float barMovementSpeed;
    [SerializeField] float maxBarHeight;
    [SerializeField] float minBarHeight;

    [SerializeField] RaftController controller;

    float totalTurn = 5f;
    float turnAmount = 0f;
    bool hasText = false;

    GameObject t2;
    GameObject t1;

     void Interact(){          
        if(!hasText) SetupText();
        isTurning = !isTurning;
        playerMovement.isDriving = isTurning;                
        t2.SetActive(isTurning);  
        t1.SetActive(isTurning);  
    }

    void Update(){
        if(isTurning) GetInputs();
    }

    void GetInputs(){

        if(Input.GetKey(KeyCode.D)){
            turnAmount = Time.deltaTime * -steeringSpeed; 
            
        }

        if(Input.GetKey(KeyCode.A)){
            turnAmount = Time.deltaTime * steeringSpeed;   
        }

        transform.Rotate(-turnAmount * 10f, 0, 0);
        totalTurn += turnAmount;
        if(totalTurn > 90) totalTurn = 90f;
        if(totalTurn < 5f) totalTurn = 5f;
        bottomBar.Translate(0,turnAmount/48f,0);
        
        if(bottomBar.localPosition.y > maxBarHeight) bottomBar.localPosition = new Vector3( bottomBar.localPosition.x,maxBarHeight, bottomBar.localPosition.z);
        if(bottomBar.localPosition.y < minBarHeight) bottomBar.localPosition = new Vector3( bottomBar.localPosition.x,minBarHeight, bottomBar.localPosition.z);
        Sail.localScale = new Vector3(1f,1f - totalTurn/100f,1f);  
        turnAmount = 0;
    }


    void SetupText(){
        hasText = true;

        t2 = Instantiate(floatingText, canvas);
        t1 = Instantiate(floatingText, canvas);

        t2.GetComponent<TextMeshProUGUI>().SetText("D");
        t1.GetComponent<TextMeshProUGUI>().SetText("A");

        t2.GetComponent<FloatingName>().Target = rightControlText;
        t1.GetComponent<FloatingName>().Target = leftControlText;
    }

    public void IntroMovement(){
        transform.Rotate(3f, 0, 0);
    }
}
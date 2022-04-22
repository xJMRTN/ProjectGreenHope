using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class RaftSteering : MonoBehaviour
{
    bool isDriving = false;
    public RaftController controller;
    public PlayerMovement playerMovement;
    [SerializeField] GameObject floatingText;
    [SerializeField] Transform wheel;
    [SerializeField] Transform leftControlText;
    [SerializeField] Transform rightControlText;
    [SerializeField] Transform canvas;
    [SerializeField] float steeringSpeed;

    float totalTurn = 0f;
    float turnAmount = 0f;
    bool hasText = false;

    GameObject t1;
    GameObject t2;

     void Interact(){        
        if(!hasText) SetupText();
        isDriving = !isDriving;
        playerMovement.isDriving = isDriving;   
        t1.SetActive(isDriving);                
        t2.SetActive(isDriving);  
           
    }

    void Update(){
        if(isDriving) GetInputs();

        ChangeRotation();
    }

    void GetInputs(){
        if(Input.GetKey(KeyCode.A)){
            turnAmount = Time.deltaTime * -steeringSpeed;
        }

        if(Input.GetKey(KeyCode.D)){
            turnAmount = Time.deltaTime * steeringSpeed;           
        }

        wheel.Rotate(0, 0, turnAmount);
        totalTurn += turnAmount;
        turnAmount = 0;
    }

    void ChangeRotation(){      
        controller.ChangeRotation(totalTurn);
    }

    void SetupText(){
        hasText = true;
        t1 = Instantiate(floatingText, canvas);
        t2 = Instantiate(floatingText, canvas);
        t1.GetComponent<TextMeshProUGUI>().SetText("A");
        t2.GetComponent<TextMeshProUGUI>().SetText("D");
        t1.GetComponent<FloatingName>().Target = leftControlText;
        t2.GetComponent<FloatingName>().Target = rightControlText;
    }

    public void IntroMovement(){
        wheel.Rotate(0, 0, 2f);
    }
}
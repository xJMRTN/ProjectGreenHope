using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class AnchorWheel : MonoBehaviour
{
    bool isTurning = false;
    public PlayerMovement playerMovement;
    [SerializeField] GameObject floatingText;
    [SerializeField] Transform leftControlText;
    [SerializeField] Transform rightControlText;
    [SerializeField] Transform canvas;
    [SerializeField] float steeringSpeed;
    [SerializeField] float pullSpeed;
    [SerializeField] Transform raft;
    [SerializeField] Transform anchor;

    [SerializeField] RaftController controller;

    float totalTurn = 0f;
    float turnAmount = 0f;
    bool hasText = false;

    GameObject t2;

     void Interact(){          
        if(!hasText) SetupText();
        isTurning = !isTurning;
        playerMovement.isDriving = isTurning;                
        t2.SetActive(isTurning);  
    }

    void Update(){
        if(isTurning) GetInputs();
    }

    void GetInputs(){

        if(Input.GetKey(KeyCode.D)){

            if(Vector3.Distance(anchor.position, transform.position) < 6f) return;   

            turnAmount = Time.deltaTime * steeringSpeed;   
            controller.MoveTowardsAnchor(anchor, pullSpeed);
        }

        transform.Rotate(0, 0, turnAmount);
        turnAmount = 0;
    }

    void SetupText(){
        hasText = true;

        t2 = Instantiate(floatingText, canvas);

        t2.GetComponent<TextMeshProUGUI>().SetText("D");

        t2.GetComponent<FloatingName>().Target = rightControlText;
    }
}

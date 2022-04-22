using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Anchor : MonoBehaviour
{
    bool pickedUp = false;
    [SerializeField] GameObject anchor;
    [SerializeField] Transform pickupPosition;
    [SerializeField] Transform anchorPosition;
    [SerializeField] Transform Player;
    [SerializeField] float throwForce;

    [SerializeField] Transform anchorConnection;
    [SerializeField] Transform holderConnection;
    [SerializeField] RaftController controller;

    Rigidbody rb;
    LineRenderer lr;
    bool mouseDown = false;

    enum AnchorState{
        idle,
        pickedUp,
        Thrown,
        Anchored
    }

    AnchorState state;

    void Start(){
        state = AnchorState.idle;
        rb = anchor.GetComponent<Rigidbody>();
        lr = anchor.GetComponent<LineRenderer>();
    }

    void Interact(){    
        if(state == AnchorState.Thrown || state == AnchorState.Anchored){
            ReturnAnchor();
            return;
        }

        pickedUp = !pickedUp;  
        if(pickedUp){
            PickUpAnchor();
        }else{
            DropAnchor();
        }           
    }

    void ReturnAnchor(){
        controller.anchor = false;
        DropAnchor();
    }

    void Update(){
        if(pickedUp) HandleInput();
        UpdateLR();
    }

    void UpdateLR(){
        lr.SetPosition(0, anchorConnection.position);
        lr.SetPosition(1, holderConnection.position);
    }

    void HandleInput(){
        if(Input.GetMouseButtonDown(0)){
            ThrowAnchor();
        }
    }

    void PickUpAnchor(){
        rb.isKinematic = true;
        rb.useGravity = false;
        anchor.transform.position = pickupPosition.position;
        anchor.transform.parent = pickupPosition;
        anchor.transform.LookAt(Player);
        state = AnchorState.pickedUp;
    }

    void DropAnchor(){
        rb.isKinematic = true;
        rb.useGravity = false;
        anchor.transform.parent = transform;
        anchor.transform.position = anchorPosition.position;
        anchor.transform.LookAt(transform);
        state = AnchorState.idle;
    }

    void ThrowAnchor(){
        pickedUp = false;
        anchor.transform.parent = null;
        rb.isKinematic = false;
        rb.useGravity = true;
        Vector3 direction = Player.forward;
        rb.AddForce(direction * throwForce, ForceMode.Impulse);
        state = AnchorState.Thrown;
    }

    public bool AnchorHasLanded(){
        if(state == AnchorState.Thrown){
            rb.isKinematic = true;
            rb.useGravity = false;
            controller.anchor = true;
            state = AnchorState.Anchored;
            return true;
        }else return false;       
    }
}
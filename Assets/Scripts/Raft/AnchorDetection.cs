using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorDetection : MonoBehaviour
{
    [SerializeField] Anchor anchor;
    bool landed = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ground" && !landed){
            landed = anchor.AnchorHasLanded();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ground" && landed){
            landed = false;
        }
    }
}
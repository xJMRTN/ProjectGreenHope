using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] LineRenderer lr;
    public BalloonLever lever;

    void FixedUpdate(){
        UpdateLine();
    }

    void UpdateLine(){
        lr.SetPosition(0, startPoint.position);
        lr.SetPosition(1, endPoint.position);
    }
}